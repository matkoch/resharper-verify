using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using System.IO;
#if RESHARPER
using DiffEngine;
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#elif RIDER
using DiffEngine;
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
        return context.HasPendingCompare();
    }

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
