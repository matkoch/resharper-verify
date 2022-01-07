using System.IO;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.UnitTestFramework.Execution;
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

            if (File.Exists(verified))
                File.Delete(verified);
            File.Move(received, verified);

            manager.MarkOutdated(element);
        }
    }
}
