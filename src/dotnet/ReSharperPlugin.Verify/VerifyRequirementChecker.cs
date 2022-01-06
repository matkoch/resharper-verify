using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;

namespace ReSharperPlugin.Verify
{
    public static class VerifyRequirementChecker
    {
        public static IActionRequirement VerifyRequirement(this IDataContext context)
        {
            if (context.GetData(DocumentModelDataConstants.DOCUMENT) == null)
            {
                return CommitAllDocumentsRequirement.TryGetInstance(context);
            }

            return CurrentPsiFileRequirement.FromDataContext(context);
        }
    }
}