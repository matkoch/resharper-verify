using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Psi.Files;
using System.IO;
using VerifyTests.ExceptionParsing;
#if RESHARPER
using DiffEngine;
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#elif RIDER
using DiffEngine;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features;
#endif

namespace ReSharperPlugin.Verify;

[Action("UnitTestSession.VerifyCompare", "Compare Received and Verified", Icon = typeof(VerifyThemedIcons.Verify))]
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
        return context.HasPendingCompare();
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        using var diffRunnerWithResultCollection = new DiffRunnerWithResultCollection();
        
        foreach (var (result, element) in context.GetVerifyResults())
        {
            var files = result.New.Concat(result.NotEqual);
#if RIDER
            var verifyTestsModel = context.GetComponent<ISolution>().GetProtocolSolution().GetVerifyModel();
            var presentation = element.GetPresentation();
            foreach (var file in files)
            {
                if (!File.Exists(file.Received))
                {
                    continue;
                }

                if (EmptyFiles.Extensions.IsText(file.Received))
                {
                    if (!File.Exists(file.Verified))
                    {
                        File.WriteAllText(file.Verified, "");
                    }

                    verifyTestsModel.Compare.Fire(new CompareData(presentation, file.Received, file.Verified));
                }
                else
                {
                    diffRunnerWithResultCollection.LaunchDiffAndRecordResult(file);
                }
            }
#else
            foreach (var file in files)
            {
                if (!File.Exists(file.Received))
                {
                    continue;
                }

                diffRunnerWithResultCollection.LaunchDiffAndRecordResult(file);
            }
#endif
        }
    }

    class DiffRunnerWithResultCollection : IDisposable
    {
        readonly List<Result> results = new List<Result>();

        public void LaunchDiffAndRecordResult(FilePair filePair)
        {
            results.Add(
                new Result(
                    filePair,
                    DiffRunner.Launch(filePair.Received, filePair.Verified)));
        }

        public void Dispose()
        {
            AssertDiffsLaunchedSuccessfully(results);
        }
        
        static void AssertDiffsLaunchedSuccessfully(IEnumerable<Result> results)
        {
            var errors = results
                .Where(x => x.IsError)
                .ToList();

            if (errors.Any())
            {
                throw new Exception(
                    CreateExceptionMessage(
                        errors));
            }
        }

        private static string CreateExceptionMessage(IEnumerable<Result> errors)
        {
            var errorMessages = errors.Select(error =>
                FormattableString.Invariant(
                    $"Failed to launch diff viewer. Error: {error.LaunchResult}. File: {error.FilePair.Received}"));
            return string.Join(
                Environment.NewLine,
                errorMessages);
        }

        private record Result(
            FilePair FilePair,
            LaunchResult LaunchResult)
        {
            public bool IsError { get; } = IsError(LaunchResult);
        }
    }

    public static bool IsError(LaunchResult launchResult)
    {
        var successResults = new[]
        {
            LaunchResult.Disabled,
            LaunchResult.StartedNewInstance,
            LaunchResult.AlreadyRunningAndSupportsRefresh
        };

        var errorResults = new[]
        {
            LaunchResult.NoEmptyFileForExtension,
            LaunchResult.NoDiffToolFound,
            LaunchResult.TooManyRunningDiffTools
        };

        if (successResults.Contains(launchResult))
        {
            return false;
        }

        if (errorResults.Contains(launchResult))
        {
            return true;
        }

        throw new Exception(
            FormattableString.Invariant($"Unknown {nameof(LaunchResult)} with value {launchResult}"));
    }
}
