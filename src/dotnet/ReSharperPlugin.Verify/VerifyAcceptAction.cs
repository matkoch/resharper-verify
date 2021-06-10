using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.Icons;
using JetBrains.Application.Icons.CompiledIconsCs;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Application.UI.Icons.CompiledIcons;
using JetBrains.Application.UI.Icons.ComposedIcons;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel.DataContext;
using JetBrains.IDE.UI.Extensions;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Common;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.UI.Icons;
using JetBrains.UI.ThemedIcons;
using JetBrains.Util;
using JetBrains.Util.Icons;
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.Session.Actions;
#endif

namespace ReSharperPlugin.Verify
{
    [Action("UnitTestSession.VerifyAccept", "Accept Received",
        Icon = typeof(VerifyThemedIcons.Verify), Id = 222011)]
    public class VerifyAcceptAction :
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
            var resultManager = context.GetComponent<IUnitTestResultManager>();
            var criterionEvaluator = context.GetComponent<IUnitTestElementCriterionEvaluator>();
            var session = context.GetData(UnitTestDataConstants.UnitTestSession.CURRENT);
            var elements = context.GetData(UnitTestDataConstants.UnitTestElements.SELECTED)?.Evaluate(criterionEvaluator).ToList() ??
                           new List<IUnitTestElement>();

            foreach (var element in elements)
            {
                var result = resultManager.GetResultData(element, session);
                if (!HasVerifyException(result))
                    continue;

                return true;
            }

            return false;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var resultManager = context.GetComponent<IUnitTestResultManager>();
            var criterionEvaluator = context.GetComponent<IUnitTestElementCriterionEvaluator>();
            var session = context.GetData(UnitTestDataConstants.UnitTestSession.CURRENT);
            var elements = context.GetData(UnitTestDataConstants.UnitTestElements.SELECTED)?.Evaluate(criterionEvaluator).ToList() ??
                           new List<IUnitTestElement>();

            foreach (var element in elements)
            {
                var result = resultManager.GetResultData(element, session);
                if (!HasVerifyException(result))
                    continue;

                var projectFile = element.GetProjectFiles().NotNull().SingleItem().NotNull();
                var exceptionLines = result.GetExceptionChunk(2).SplitByNewLine();
                // TODO: Consider "Verify command placed in clipboard."
                var (receivedFileName, verifiedFileName) = exceptionLines[2] == "Pending verification:"
                    ? (exceptionLines[4], exceptionLines[3].Split(':').First())
                    : (exceptionLines[3].TrimFromStart("Received: "), exceptionLines[4].TrimFromStart("Verified: "));
                var receivedFile = (projectFile.Location.Directory / receivedFileName).FullPath;
                var verifiedFile = (projectFile.Location.Directory / verifiedFileName).FullPath;

                if (File.Exists(verifiedFile))
                    File.Delete(verifiedFile);
                File.Move(receivedFile, verifiedFile);

                resultManager.MarkOutdated(element);
            }
        }

        private static bool HasVerifyException(UnitTestResultData result)
        {
            return result.ExceptionChunks > 2 &&
                   result.GetExceptionChunk(0) == "VerifyException" &&
                   result.GetExceptionChunk(2).StartsWith("Results do not match");
        }
    }
}