using System.IO;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#endif

namespace ReSharperPlugin.Verify;

[Action("UnitTestSession.VerifyAccept", "Accept Received",
    Icon = typeof(VerifyThemedIcons.Verify))]
public class VerifyAcceptAction :
#if RESHARPER
    IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
    IExecutableAction,
    IActionWithUpdateRequirement
{
    public IActionRequirement GetRequirement(IDataContext dataContext)
    {
        return dataContext.GetData(DocumentModelDataConstants.DOCUMENT) != null
            ? CurrentPsiFileRequirement.FromDataContext(dataContext)
            : CommitAllDocumentsRequirement.TryGetInstance(dataContext);
    }

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        var resultManager = context.GetComponent<IUnitTestResultManager>();
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        var elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
        if (session == null || elements == null)
            return false;

        foreach (var element in elements)
        {
            var result = resultManager.GetResultData(element, session);
            if (!HasVerifyException(result))
                continue;

            return true;
        }

        return false;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var resultManager = context.GetComponent<IUnitTestResultManager>();
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        var elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
        if (session == null || elements == null)
            return;

        foreach (var element in elements)
        {
            var result = resultManager.GetResultData(element, session);
            if (!HasVerifyException(result))
                continue;

            var projectFile = element.GetProjectFiles().NotNull().SingleItem().NotNull();
            var exceptionLines = result.GetExceptionChunk(2).SplitByNewLine();
            var (receivedFileName, verifiedFileName) = (
                exceptionLines.FirstOrDefault(x => x.StartsWith("Received"))?.TrimFromStart("Received: "),
                exceptionLines.FirstOrDefault(x => x.StartsWith("Verified"))?.TrimFromStart("Verified: "));
            var receivedFile = (projectFile.Location.Directory / receivedFileName.NotNull("receivedFileName")).FullPath;
            var verifiedFile = (projectFile.Location.Directory / verifiedFileName.NotNull("verifiedFileName")).FullPath;

            if (File.Exists(verifiedFile))
                File.Delete(verifiedFile);
            File.Move(receivedFile, verifiedFile);

            resultManager.MarkOutdated(element);
        }
    }

    static bool HasVerifyException(UnitTestResultData result)
    {
        return result.ExceptionChunks > 2 &&
               result.GetExceptionChunk(0) == "VerifyException" &&
               result.GetExceptionChunk(2).StartsWith("Results do not match");
    }
}