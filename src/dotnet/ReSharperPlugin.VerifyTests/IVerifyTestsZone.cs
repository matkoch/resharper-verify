using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.VerifyTests
{
    [ZoneDefinition]
    public interface IVerifyTestsZone : IPsiLanguageZone,
        IRequire<IUnitTestingZone>
    {
    }
}