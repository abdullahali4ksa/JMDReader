// MIT License - see LICENSE file in the repository root.

using System.Linq;
using MDReader.Core.Localization;
using Xunit;

namespace MDReader.Core.Tests;

public class LocalizationServiceTests
{
    [Fact]
    public void SupportedLanguages_ContainsAll10WorldLanguages()
    {
        var langs = LocalizationService.SupportedLanguages;
        Assert.Equal(10, langs.Count);

        var codes = langs.Select(l => l.Code).ToList();
        Assert.Contains("ar", codes);
        Assert.Contains("en", codes);
        Assert.Contains("zh", codes);
        Assert.Contains("hi", codes);
        Assert.Contains("es", codes);
        Assert.Contains("fr", codes);
        Assert.Contains("de", codes);
        Assert.Contains("ru", codes);
        Assert.Contains("ja", codes);
        Assert.Contains("pt", codes);
    }

    [Fact]
    public void SetLanguage_ValidCodes_UpdatesLanguageAndRtl()
    {
        var service = LocalizationService.Instance;

        service.SetLanguage("en");
        Assert.Equal("en", service.CurrentLanguage);
        Assert.False(service.IsRtl);
        Assert.Equal("File", service.MenuFile);

        service.SetLanguage("ar");
        Assert.Equal("ar", service.CurrentLanguage);
        Assert.True(service.IsRtl);
        Assert.Equal("ملف", service.MenuFile);

        service.SetLanguage("zh");
        Assert.Equal("zh", service.CurrentLanguage);
        Assert.Equal("文件", service.MenuFile);

        service.SetLanguage("hi");
        Assert.Equal("hi", service.CurrentLanguage);
        Assert.Equal("फ़ाइल", service.MenuFile);

        service.SetLanguage("es");
        Assert.Equal("es", service.CurrentLanguage);
        Assert.Equal("Archivo", service.MenuFile);

        service.SetLanguage("fr");
        Assert.Equal("fr", service.CurrentLanguage);
        Assert.Equal("Fichier", service.MenuFile);

        service.SetLanguage("de");
        Assert.Equal("de", service.CurrentLanguage);
        Assert.Equal("Datei", service.MenuFile);

        service.SetLanguage("ru");
        Assert.Equal("ru", service.CurrentLanguage);
        Assert.Equal("Файл", service.MenuFile);

        service.SetLanguage("ja");
        Assert.Equal("ja", service.CurrentLanguage);
        Assert.Equal("ファイル", service.MenuFile);

        service.SetLanguage("pt");
        Assert.Equal("pt", service.CurrentLanguage);
        Assert.Equal("Arquivo", service.MenuFile);
    }

    [Fact]
    public void SetLanguage_InvalidCode_FallsBackToEnglish()
    {
        var service = LocalizationService.Instance;
        service.SetLanguage("non-existent-code");
        Assert.Equal("en", service.CurrentLanguage);
        Assert.Equal("File", service.MenuFile);
    }

    [Fact]
    public void AllSupportedLanguages_HaveNonEmptyEssentialKeys()
    {
        var service = LocalizationService.Instance;
        var essentialKeys = new[]
        {
            "MenuFile", "MenuNew", "MenuSave", "MenuView", "MenuSettings",
            "TooltipSplit", "TooltipRead", "TooltipOutline", "TooltipAutoScroll",
            "StatusWords", "StatusLines", "TextPathCopied"
        };

        foreach (var (code, _, _) in LocalizationService.SupportedLanguages)
        {
            service.SetLanguage(code);
            foreach (var key in essentialKeys)
            {
                var val = service.Get(key);
                Assert.False(string.IsNullOrWhiteSpace(val), $"Language '{code}' missing key '{key}'");
            }
        }
    }

    [Fact]
    public void PropertyChanged_FiresWhenLanguageChanges()
    {
        var service = LocalizationService.Instance;
        service.SetLanguage("en");

        bool eventFired = false;
        service.PropertyChanged += (_, _) => eventFired = true;

        service.SetLanguage("ar");
        Assert.True(eventFired);
        Assert.Equal("ar", service.CurrentLanguage);
    }
}
