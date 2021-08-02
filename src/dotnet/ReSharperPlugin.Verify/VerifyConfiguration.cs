using System;
using JetBrains.Application;

namespace ReSharperPlugin.Verify
{
    [ShellComponent]
    public class VerifyConfiguration
    {
        public VerifyConfiguration()
        {
            Environment.SetEnvironmentVariable(
                "Verify_DisableClipboard",
                bool.TrueString,
                EnvironmentVariableTarget.Process);
        }
    }
}