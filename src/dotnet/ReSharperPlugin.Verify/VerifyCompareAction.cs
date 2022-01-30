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

            var files = parsed.New.Concat(parsed.NotEqual);
#if RIDER
            var verifyTestsModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
            var presentation = element.GetPresentation();
            foreach (var file in files)
            {
                verifyTestsModel.Compare.Fire(new CompareData(presentation, file.Received, file.Verified));
            }
#else
            foreach (var file in files)
            {
                DiffRunner.Launch(file.Received, file.Verified);
            }
#endif
        }
    }
}
