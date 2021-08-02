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
                "DiffEngine_Disabled",
                bool.TrueString,
                EnvironmentVariableTarget.Process);
        }
    }
}