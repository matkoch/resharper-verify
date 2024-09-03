using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.IDE;
using JetBrains.Interop.WinApi;
using JetBrains.Util;
#if RESHARPER
using JetBrains.ReSharper.UnitTestExplorer.Session.Actions;
using JetBrains.ReSharper.UnitTestFramework.UI.Session.Actions;
#endif

namespace ReSharperPlugin.Verify;

[Action(
    ResourceType: typeof(Resources),
    TextResourceName: nameof(Resources.VerifyOpenActionText),
    Icon = typeof(Icons.VerifyThemedIcons.VerifyOpen))]
public class VerifyOpenAction :
#if RESHARPER
    IInsertBefore<UnitTestSessionContextMenuActionGroup, UnitTestSessionAppendChildren>,
#endif
    IExecutableAction,
    IActionWithUpdateRequirement
{
    public IActionRequirement GetRequirement(IDataContext dataContext) =>
        dataContext.GetRequirement();

    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) =>
        context.GetVerifiedFiles().Any();

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var editorManager = context.GetComponent<IEditorManager>();
        var files = context.GetVerifiedFiles().ToList();
        if (files.Count > 1 && !ShouldOpenFiles())
        {
            return;
        }

        foreach (var file in files)
        {
            editorManager.OpenFileAsync(file, OpenFileOptions.DefaultActivate);
        }

        bool ShouldOpenFiles()
            => MessageBox.ShowMessageBox(
                   $"Open {files.Count} verified files?",
                   MbButton.MB_YESNO,
                   MbIcon.MB_ICONQUESTION)
               == DialogBoxCommandId.IDYES;
    }
}
