using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using VerifyTests.ExceptionParsing;

public static class Extensions
{
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
        var info = result.GetExceptionInfo(0);
        return info.Type == "VerifyException" ||
               (info.Message?.StartsWith("VerifyException") ?? false);
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
