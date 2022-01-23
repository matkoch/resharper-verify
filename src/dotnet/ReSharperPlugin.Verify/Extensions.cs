using JetBrains.ReSharper.UnitTestFramework.Execution;

public static class Extensions
{
    public static bool HasVerifyException(this UnitTestResultData result)
    {
        return result.ExceptionChunks > 2 &&
               result.GetExceptionChunk(0) == "VerifyException" &&
               result.GetExceptionChunk(2).StartsWith("Results do not match");
    }
}