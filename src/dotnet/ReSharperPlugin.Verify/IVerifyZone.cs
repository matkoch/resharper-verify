using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.Verify;

[ZoneDefinition]
public interface IVerifyZone : IPsiLanguageZone,
    IRequire<IUnitTestingZone>
{
}