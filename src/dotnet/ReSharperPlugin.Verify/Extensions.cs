using System;
using JetBrains.ReSharper.UnitTestFramework.Execution;
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
}
