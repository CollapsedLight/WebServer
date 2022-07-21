using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace CollapsedLight.WebApp.Localization;

public class LocalizerFactory : IStringLocalizerFactory
{
    private LocalizerService _localizerService;

    public LocalizerFactory(LocalizerService localizerService)
    {
        _localizerService = localizerService;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return CreateImpl();
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return CreateImpl();
    }

    private IStringLocalizer CreateImpl()
    {
        var culture = CultureInfo.CurrentUICulture;

        var texts = _localizerService.GetTagsFor(culture.LCID);

        return new LocalizedText(texts);
    }
}

public class LocalizedText : IStringLocalizer
{
    private readonly IReadOnlyDictionary<string, LocalizedString> _tags;

    public LocalizedText(IReadOnlyDictionary<string, LocalizedString> tags)
    {
        _tags = tags;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _tags.Values;
    }

    public LocalizedString this[string name] => _tags.ContainsKey(name)
        ? _tags[name]
        : _tags.ContainsKey(name.ToLower())
            ? _tags[name.ToLower()]
            : new LocalizedString(name, $"_NF_{name}");

    public LocalizedString this[string name, params object[] arguments] => _tags[name];
}