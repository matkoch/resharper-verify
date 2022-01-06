using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.UnitTestFramework.Execution;
using JetBrains.Util;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Persistence;
using JetBrains.ReSharper.UnitTestFramework.Session;

namespace ReSharperPlugin.Verify
{
    static class VerifyExceptionReader
    {
        public static bool TryGetVerifyFiles(this UnitTestResultData result, out string received, out string verified)
        {
            received = null;
            verified = null;

            if (!result.IsVerifyException())
            {
                return false;
            }

            //Received and Verified paths are at the end of the exception message
            foreach (var chuck in result.ExceptionChuckValues().Reverse())
            {
                var lines = chuck
                    .SplitByNewLine()
                    .Reverse()
                    .ToArray();

                if (received == null)
                {
                    received = lines.FirstOrDefault(x => x.StartsWith("Received path: "))?.TrimFromStart("Received path: ");
                }

                if (verified == null)
                {
                    verified = lines.FirstOrDefault(x => x.StartsWith("Verified path: "))?.TrimFromStart("Verified path: ");
                }

                if (received != null && verified != null)
                {
                    return true;
                }
            }

            throw new Exception("Received or Verified file path not found in exception. Update to at least version 14.13.1 of Verify. ");
        }

        public static bool IsVerifyException(this IDataContext context)
        {
            var manager = context.ResultManager();
            if (!context.TryGetElements(out var session, out var elements))
                return false;

            return elements
                .Select(element => manager.GetResultData(element, session))
                .Any(result => result.IsVerifyException());
        }

        public static IUnitTestResultManager ResultManager(this IDataContext context)
        {
            return context.GetComponent<IUnitTestResultManager>();
        }

        public static bool TryGetElements(this IDataContext context, out IUnitTestSession session, out IQueryResult elements)
        {
            session = context.GetData(UnitTestDataConstants.Session.CURRENT);
            elements = context.GetData(UnitTestDataConstants.Elements.SELECTED)?.Criterion.Evaluate();
            return session != null && elements != null;
        }

        static IEnumerable<string> ExceptionChuckValues(this UnitTestResultData result)
        {
            for (var i = 0; i < result.ExceptionChunks; i++)
            {
                var exceptionChunk = result.GetExceptionChunk(i);
                yield return exceptionChunk;
            }
        }

        static bool IsVerifyException(this UnitTestResultData result)
        {
            var exceptionChunk = result.GetExceptionChunk(0);
            return exceptionChunk.StartsWith("VerifyException");
        }
    }
}