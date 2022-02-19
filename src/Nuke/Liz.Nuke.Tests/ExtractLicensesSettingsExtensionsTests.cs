using FluentAssertions;
using Nuke.Common.IO;
using System.Reflection;
using Xunit;

namespace Liz.Nuke.Tests;

public class ExtractLicensesSettingsExtensionsTests
{
    [Fact]
    public void ExtractLicensesSettingsExtensions_Has_An_Extensions_For_Each_Settings_Property()
    {
        var properties = GetExtractLicensesSettingsProperties();

        foreach (var property in properties)
        {
            typeof(ExtractLicensesSettingsExtensions)
                .Should()
                .HaveMethod($"Set{property.Name}", new[] { typeof(ExtractLicensesSettings), property.PropertyType });
            
            // boolean properties have some more corresponding methods...
            if (property.PropertyType != typeof(bool)) continue;

            typeof(ExtractLicensesSettingsExtensions)
                .Should()
                .HaveMethod($"Enable{property.Name}", new[] { typeof(ExtractLicensesSettings) });

            typeof(ExtractLicensesSettingsExtensions)
                .Should()
                .HaveMethod($"Disable{property.Name}", new[] { typeof(ExtractLicensesSettings) });

            typeof(ExtractLicensesSettingsExtensions)
                .Should()
                .HaveMethod($"Toggle{property.Name}", new[] { typeof(ExtractLicensesSettings) });

            typeof(ExtractLicensesSettingsExtensions)
                .Should()
                .HaveMethod($"Reset{property.Name}", new[] { typeof(ExtractLicensesSettings) });
        }
    }

    [Fact]
    public void ExtractLicensesSettings_Sets_AbsolutePath_Settings()
    {
        var absolutePathInstance = (AbsolutePath)"C:/something";
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(AbsolutePath));

        foreach (var property in properties)
        {
            var setMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Set{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                setMethod?.Invoke(null, new object?[] { null, absolutePathInstance }));
            Assert.Throws<TargetInvocationException>(() =>
                setMethod?.Invoke(null, new object?[] { settingsInstance, null }));
                
            setMethod?.Invoke(null, new object?[] { settingsInstance, absolutePathInstance });

            property
                .GetValue(settingsInstance)
                .Should()
                .Be(absolutePathInstance);
        }
    }

    [Fact]
    public void ExtractLicensesSettings_Sets_Bool_Settings()
    {
        const bool boolInstance = true;
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(bool));

        foreach (var property in properties)
        {
            var setMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Set{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                setMethod?.Invoke(null, new object?[] { null, boolInstance }));
                
            setMethod?.Invoke(null, new object?[] { settingsInstance, boolInstance });

            property
                .GetValue(settingsInstance)
                .Should()
                .Be(boolInstance);
        }
    }
    
    [Fact]
    public void ExtractLicensesSettings_Enables_Bool_Settings()
    {
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(bool));

        foreach (var property in properties)
        {
            var enableMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Enable{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                enableMethod?.Invoke(null, new object?[] { null }));
                
            enableMethod?.Invoke(null, new object?[] { settingsInstance });

            property
                .GetValue(settingsInstance)
                .As<bool>()
                .Should()
                .BeTrue();
        }
    }
    
    [Fact]
    public void ExtractLicensesSettings_Disables_Bool_Settings()
    {
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(bool));

        foreach (var property in properties)
        {
            var disableMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Disable{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                disableMethod?.Invoke(null, new object?[] { null }));
                
            disableMethod?.Invoke(null, new object?[] { settingsInstance });

            property
                .GetValue(settingsInstance)
                .As<bool>()
                .Should()
                .BeFalse();
        }
    }
    
    [Fact]
    public void ExtractLicensesSettings_Toggles_Bool_Settings()
    {
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(bool));

        foreach (var property in properties)
        {
            var toggleMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Toggle{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                toggleMethod?.Invoke(null, new object?[] { null }));
            
            // true should toggle to false
            property.SetValue(settingsInstance, true);
                
            toggleMethod?.Invoke(null, new object?[] { settingsInstance });

            property
                .GetValue(settingsInstance)
                .As<bool>()
                .Should()
                .BeFalse();
            
            // false should toggle to true
            property.SetValue(settingsInstance, false);
                
            toggleMethod?.Invoke(null, new object?[] { settingsInstance });

            property
                .GetValue(settingsInstance)
                .As<bool>()
                .Should()
                .BeTrue();
        }
    }
    
    [Fact]
    public void ExtractLicensesSettings_Resets_Bool_Settings()
    {
        var properties = GetExtractLicensesSettingsProperties()
            .Where(property => property.PropertyType == typeof(bool));
        var settingsInstanceWithDefaults = new ExtractLicensesSettings();

        foreach (var property in properties)
        {
            var resetMethod = typeof(ExtractLicensesSettingsExtensions).GetMethod($"Reset{property.Name}");

            var settingsInstance = new ExtractLicensesSettings();
            
            Assert.Throws<TargetInvocationException>(() =>
                resetMethod?.Invoke(null, new object?[] { null }));

            resetMethod?.Invoke(null, new object?[] { settingsInstance });

            property
                .GetValue(settingsInstance)
                .Should()
                .Be(property.GetValue(settingsInstanceWithDefaults));
        }
    }

    private static IEnumerable<PropertyInfo> GetExtractLicensesSettingsProperties()
    {
        var properties = typeof(ExtractLicensesSettings).GetProperties();
        return properties;
    }
}
