using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using System.IO;
using DiffEngine;
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#elif RIDER
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Protocol;
#endif

namespace ReSharperPlugin.Verify;

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.VerifyCompareActionText),
    Icon = typeof(Icons.VerifyThemedIcons.VerifyCompare))]
public class VerifyCompareAction :
#if RESHARPER
    IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
    IExecutableAction,
    IActionWithUpdateRequirement
{
    public IActionRequirement GetRequirement(IDataContext dataContext) =>
        dataContext.GetRequirement();

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) =>
        context.HasPendingCompare();

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        foreach (var (result, element) in context.GetVerifyResults())
        {
            var files = result.New.Concat(result.NotEqual);
#if RIDER
            var verifyTestsModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
            var presentation = element.GetPresentation();
            foreach (var file in files)
            {
                if (!File.Exists(file.Received))
                {
                    continue;
                }

                if (EmptyFiles.FileExtensions.IsText(file.Received))
                {
                    if (!File.Exists(file.Verified))
                    {
                        File.WriteAllText(file.Verified, "");
                    }

                    verifyTestsModel.Compare.Fire(new CompareData(presentation, file.Received, file.Verified));
                }
                else
                {
                    DiffRunner.Launch(file.Received, file.Verified);
                }
            }
#else
            foreach (var file in files)
            {
                if (!File.Exists(file.Received))
                {
                    continue;
                }

                DiffRunner.Launch(file.Received, file.Verified);
            }
#endif
        }
    }
}
