using System;
using System.Linq;
using DiffEngine;
using NUnit.Framework;

namespace ReSharperPlugin.Verify.Tests;

[TestFixture]
public class VerifyCompareActionTest
{
    static object[] AllDefinedLaunchResultValues =
        GetAllEnumValuesOf<LaunchResult>()
            .Select(e => new object[] { e })
            .ToArray();
    
    [TestCaseSource(nameof(AllDefinedLaunchResultValues))]
    public void IsError_WhenDefinedResultEnum_ShouldNotThrow(LaunchResult launchResult)
    {
        // Note: When this test fails you will have to adapt the implementation
        // to either consider the new enum value as success or error.
        VerifyCompareAction.IsError(launchResult);
    }

    static TEnum[] GetAllEnumValuesOf<TEnum>()
        where TEnum: Enum
        => (TEnum[])Enum.GetValues(typeof(TEnum));
}