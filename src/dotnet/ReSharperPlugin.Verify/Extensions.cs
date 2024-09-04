using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using VerifyTests.ExceptionParsing;

public static class Extensions
{
    public static IActionRequirement GetRequirement(this IDataContext dataContext)
    {
        if (dataContext.GetData(DocumentModelDataConstants.DOCUMENT) == null)
        {
            return CommitAllDocumentsRequirement.TryGetInstance(dataContext);
        }

        return CurrentPsiFileRequirement.FromDataContext(dataContext);
    }

    public static bool HasPendingCompare(this IDataContext context)
    {
        foreach (var (result, _) in context.GetVerifyResults())
        {
            foreach (var file in result.New.Concat(result.NotEqual))
            {
                if (File.Exists(file.Received))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool HasPendingAccept(this IDataContext context)
    {
        foreach (var (result, _) in context.GetVerifyResults())
        {
            foreach (var file in result.New.Concat(result.NotEqual))
            {
                if (File.Exists(file.Received))
                {
                    return true;
                }
            }

            foreach (var file in result.Delete)
            {
                if (File.Exists(file))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static IEnumerable<VirtualFileSystemPath> GetVerifiedFiles(this IDataContext context)
    {
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        if (session == null)
        {
            yield break;
        }

        var elements = context.GetData(UnitTestDataConstants.Elements.IN_CONTEXT)?.Criterion.Evaluate();
        if (elements == null)
        {
            yield break;
        }

        foreach (var element in elements)
        {
            var directory = element.GetProjectFiles()?.FirstOrDefault()?.Location.Parent;
            if (directory == null)
                continue;

            var parent = element.TraverseAcross(x => x.Parent).Last().ShortName;
            var name = element.ShortName
                .Replace("(", "_")
                .Replace(": ", "=")
                .Replace(", ", "_")
                .Replace("\"", string.Empty)
                .Replace(")", string.Empty);
            var verifiedFileName = $"{parent}.{name}.verified";

            foreach (var file in directory.GetChildFiles(verifiedFileName + "*"))
            {
                yield return file;
            }
        }
    }

    public static IEnumerable<(Result, IUnitTestElement)> GetVerifyResults(this IDataContext context)
    {
        var session = context.GetData(UnitTestDataConstants.Session.CURRENT);
        if (session == null)
        {
            yield break;
        }

        var elements = context.GetData(UnitTestDataConstants.Elements.IN_CONTEXT)?.Criterion.Evaluate();
        if (elements == null)
        {
            yield break;
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
            if (!parsed.Equals(default(Result)))
            {
                yield return (parsed, element);
            }
        }
    }

    private static bool HasVerifyException(this UnitTestResultData result)
    {
        if (result.ExceptionCount == 0)
            return false;

        var info = result.GetExceptionInfo(0);
        return info.Type == "VerifyException" ||
               (info.Message?.StartsWith("VerifyException") ?? false) ||
               // MSTest
               (info.Message?.Substring(info.Message.IndexOf('\n') + 1, 15) == "VerifyException");
    }

    private static Result GetParseResult(this UnitTestResultData result)
    {
        var message = result.GetExceptionInfo(0).Message!;
        try
        {
            return Parser.Parse(message);
        }
        catch (Exception exception)
        {
            MessageBox.ShowError(
                exception.Message +
                "\n\nNote that you might need to rerun tests before your changes take effect.");
            return default;
        }
    }
}
