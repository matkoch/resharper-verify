using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.VerifyTests
{
    [ZoneDefinition]
    public interface IVerifyTestsZone : IPsiLanguageZone,
        IRequire<IUnitTestingZone>
    {
    }
}