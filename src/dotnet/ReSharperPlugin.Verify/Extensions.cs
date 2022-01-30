using System;
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
using JetBrains.Util;
using VerifyTests.ExceptionParsing;

public static class Extensions
{
    public static bool HasVerifyException(this UnitTestResultData result)
    {
        return result.ExceptionChunks > 1 &&
               result.GetExceptionChunk(0) == "VerifyException";
    }

    public static Result GetParseResult(this UnitTestResultData result)
    {
        var exceptionLines = result.GetExceptionChunk(2).SplitByNewLine();
        try
        {
            return Parser.Parse(exceptionLines);
        }
        catch (Exception exception)
        {
            MessageBox.ShowError(
                exception.Message +
                "\n\nNote that you might need to rerun tests before your changes take effect.");
            return default;
        }
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
