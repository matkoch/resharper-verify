using System.IO;
using System.Linq;
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

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.VerifyAcceptActionText),
    Icon = typeof(Icons.VerifyThemedIcons.VerifyAccept))]
public class VerifyAcceptAction : VerifyAcceptActionBase
{
}

public abstract class VerifyAcceptActionBase :
#if RESHARPER
    IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
    IExecutableAction,
    IActionWithUpdateRequirement
{
    public IActionRequirement GetRequirement(IDataContext dataContext) =>
        dataContext.GetRequirement();

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) =>
        context.HasPendingAccept();

    public virtual void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var resultManager = context.GetComponent<IUnitTestResultManager>();
        foreach (var (result, element) in context.GetVerifyResults())
        {
            foreach (var file in result.New.Concat(result.NotEqual))
            {
                if (File.Exists(file.Verified))
                {
                    File.Delete(file.Verified);
                }

                File.Move(file.Received, file.Verified);
            }

            foreach (var file in result.Delete)
            {
                File.Delete(file);
            }

            resultManager.MarkOutdated(element);
        }
    }
}
