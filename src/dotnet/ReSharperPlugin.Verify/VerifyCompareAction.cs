using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.UnitTestFramework.Execution;
#if RESHARPER
using DiffEngine;
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#elif RIDER
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features;
#endif

namespace ReSharperPlugin.Verify
{
    [Action("UnitTestSession.VerifyCompare", "Compare Received and Verified",
        Icon = typeof(VerifyThemedIcons.Verify))]
    public class VerifyCompareAction :
#if RESHARPER
        IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
        IExecutableAction,
        IActionWithUpdateRequirement
    {
        public IActionRequirement GetRequirement(IDataContext context)
        {
            return context.VerifyRequirement();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate next)
        {
            return context.IsVerifyException();
        }

        public void Execute(IDataContext context, DelegateExecute next)
        {
            var manager = context.ResultManager();
            if (!context.TryGetElements(out var session, out var elements))
                return;

            foreach (var element in elements)
            {
                var result = manager.GetResultData(element, session);
                if (!result.TryGetVerifyFiles(out var received, out var verified))
                    continue;

#if RIDER
                var verifyModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
                verifyModel.Compare.Fire(new CompareData(element.GetPresentation(), received, verified));
#else
                DiffRunner.Launch(received, verified);
#endif
            }
        }
    }
}