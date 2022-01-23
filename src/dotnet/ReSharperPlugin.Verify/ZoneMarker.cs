using JetBrains.Application.BuildScript.Application.Zones;

namespace ReSharperPlugin.Verify;

[ZoneMarker]
public class ZoneMarker : IRequire<IVerifyZone>
{
}