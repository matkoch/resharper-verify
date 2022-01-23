using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
#if RESHARPER
using DiffEngine;
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#elif RIDER
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features;
#endif

namespace ReSharperPlugin.Verify;

[Action("UnitTestSession.VerifyCompare", "Compare Received and Verified", Icon = typeof(VerifyThemedIcons.Verify))]
public class VerifyCompareAction :
#if RESHARPER
    IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
    IExecutableAction,
    IActionWithUpdateRequirement
{
    public IActionRequirement GetRequirement(IDataContext dataContext)
    {
        return dataContext.GetRequirement();
    }

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        var session = context.GetTestSession();
        var elements = context.GetElements();
        if (session == null || elements == null)
        {
            return false;
        }

        var resultManager = context.GetComponent<IUnitTestResultManager>();

        foreach (var element in elements)
        {
            var result = resultManager.GetResultData(element, session);
            if (!result.HasVerifyException())
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var session = context.GetTestSession();
        var elements = context.GetElements();
        if (session == null || elements == null)
        {
            return;
        }

        var resultManager = context.GetComponent<IUnitTestResultManager>();

        foreach (var element in elements)
        {
            var result = resultManager.GetResultData(element, session);
            if (!result.HasVerifyException())
            {
                continue;
            }

            var projectFile = element.GetProjectFiles().NotNull().SingleItem().NotNull();
            var exceptionLines = result.GetExceptionChunk(2).SplitByNewLine();
            var (receivedFileName, verifiedFileName) = (
                exceptionLines.FirstOrDefault(x => x.StartsWith("Received"))?.TrimFromStart("Received: "),
                exceptionLines.FirstOrDefault(x => x.StartsWith("Verified"))?.TrimFromStart("Verified: "));
            var receivedFile = (projectFile.Location.Directory / receivedFileName.NotNull("receivedFileName")).FullPath;
            var verifiedFile = (projectFile.Location.Directory / verifiedFileName.NotNull("verifiedFileName")).FullPath;

#if RIDER
            var verifyTestsModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
            verifyTestsModel.Compare.Fire(new CompareData(element.GetPresentation(), receivedFile, verifiedFile));
#else
            DiffRunner.Launch(receivedFile, verifiedFile);
#endif
        }
    }
}