using System.IO;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using VerifyTests.ExceptionParsing;
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#endif

namespace ReSharperPlugin.Verify;

[Action("UnitTestSession.VerifyAccept", "Accept Received", Icon = typeof(VerifyThemedIcons.Verify))]
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
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        var elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
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
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        var elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
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

            var parsed = result.GetParseResult();
            if (parsed.Equals(default(Result)))
                return;

            foreach (var file in parsed.New.Concat(parsed.NotEqual))
            {
                if (File.Exists(file.Verified))
                {
                    File.Delete(file.Verified);
                }

                File.Move(file.Received, file.Verified);
            }

            foreach (var file in parsed.Delete)
            {
                File.Delete(file);
            }

            resultManager.MarkOutdated(element);
        }
    }
}
