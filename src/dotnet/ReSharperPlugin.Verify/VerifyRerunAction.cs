using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Execution.Hosting;
using JetBrains.ReSharper.UnitTestFramework.Execution.Launch;

namespace ReSharperPlugin.Verify;

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.VerifyRerunActionText),
    Icon = typeof(Icons.VerifyThemedIcons.VerifyRerun))]
public class VerifyRerunAction : VerifyAcceptActionBase
{
    public override void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        base.Execute(context, nextExecute);

        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        var criterion = context.GetData(UnitTestDataConstants.Elements.IN_CONTEXT)?.Criterion;
        if (session == null || criterion == null)
            return;

        var launchManager = context.GetComponent<IUnitTestLaunchManager>();
        var hostProvider = UnitTestHost.Instance.GetProviderDescriptor(WellKnownHostProvidersIds.RunProviderId).Provider;
        var unitTestElements = new UnitTestElements(criterion);
        launchManager.BuildAndRunSession(
            session,
            unitTestElements,
            hostProvider);
    }
}
