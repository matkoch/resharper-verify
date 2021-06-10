using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Common;
using JetBrains.Util;
#if RIDER
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features;
#endif
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.Session.Actions;
#endif

namespace ReSharperPlugin.Verify
{
    [Action("UnitTestSession.VerifyCompare", "Compare Received & Verified",
        Icon = typeof(VerifyThemedIcons.Verify), Id = 222011)]
    public class VerifyCompareAction :
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
            var elements = context.GetData(UnitTestDataConstants.UnitTestElements.SELECTED)
                               ?.Evaluate(criterionEvaluator).ToList() ??
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
            var elements = context.GetData(UnitTestDataConstants.UnitTestElements.SELECTED)
                               ?.Evaluate(criterionEvaluator).ToList() ??
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

#if RIDER
                var verifyTestsModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
                verifyTestsModel.Compare.Fire(new CompareData(element.GetPresentation(element.Parent), receivedFile, verifiedFile));
#endif
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