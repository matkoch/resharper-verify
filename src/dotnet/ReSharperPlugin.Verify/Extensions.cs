using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using JetBrains.ReSharper.UnitTestFramework.Session;

public static class Extensions
{
    public static bool HasVerifyException(this UnitTestResultData result)
    {
        return result.ExceptionChunks > 2 &&
               result.GetExceptionChunk(0) == "VerifyException" &&
               result.GetExceptionChunk(2).StartsWith("Results do not match");
    }

    public static IUnitTestSession GetTestSession(this IDataContext context)
    {
        return context.GetData(UnitTestDataConstants.Session.CURRENT);
    }

    public static IQueryResult GetElements(this IDataContext context)
    {
        var elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
        return elements;
    }

    public static IActionRequirement GetRequirement(this IDataContext dataContext)
    {
        if (dataContext.GetData(DocumentModelDataConstants.DOCUMENT) != null)
        {
            return CurrentPsiFileRequirement.FromDataContext(dataContext);
        }

        return CommitAllDocumentsRequirement.TryGetInstance(dataContext);
    }

}