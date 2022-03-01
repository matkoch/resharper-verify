using System;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using VerifyTests.ExceptionParsing;

public static class Extensions
{
    public static bool HasVerifyException(this UnitTestResultData result)
    {
        return result.GetExceptionInfo(0).Type == "VerifyException";
    }

    public static Result GetParseResult(this UnitTestResultData result)
    {
        var exceptionLines = result.GetExceptionInfo(0).Message.NotNull().SplitByNewLine();
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
}
