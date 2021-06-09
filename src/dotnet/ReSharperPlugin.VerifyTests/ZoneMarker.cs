using JetBrains.Application.BuildScript.Application.Zones;

namespace ReSharperPlugin.VerifyTests
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IVerifyTestsZone>
    {
    }
}