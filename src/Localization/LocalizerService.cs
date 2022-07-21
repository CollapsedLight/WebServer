using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Localization;
using System.Collections.Immutable;
using System.Data;
using System.Globalization;

namespace CollapsedLight.WebApp.Localization;

public class LocalizerService : IDisposable
{
    private LocalizerContext _localizer = new();

    public LocalizerService()
    {
        _localizer.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _localizer.Dispose();
    }

    public IEnumerable<Language> GetLanguages()
    {
        return _localizer.Languages.OrderBy(x => x.Id);
    }

    public DataTable CreateCompleteTable()
    {
        var language = _localizer.Languages.ToList();

        var dt = new DataTable();
        var pkc = new DataColumn[1];
        pkc[0] = new DataColumn("Id", typeof(int));
        dt.Columns.Add(pkc[0]);
        dt.Columns.Add(new DataColumn("Tag", typeof(TextTag)));
        dt.PrimaryKey = pkc;

        dt.Columns.AddRange(language.Select(x => new DataColumn(x.Name, typeof(TextLocalized))).ToArray());
        var texts = _localizer.Texts.Include(x => x.Descriptions).ToList();
        foreach (var text in texts)
        {
            var row = !dt.Rows.Contains(text.Id) ? dt.NewRow() : dt.Rows.Find(text.Id);
            if (!row["Id"].Equals(text.Id))
            {
                row["Id"] = text.Id;
                row["Tag"] = text;
            }

            foreach (var lcid in language)
            {
                var entry = text.Descriptions.FirstOrDefault(x => x.LanguageId == lcid.Id)
                         ?? CreateNewEntry(text.Id, lcid.Id, string.Empty, text).Entity;

                row[lcid.Name] = entry;

                if (!dt.Rows.Contains(text.Id))
                    dt.Rows.Add(row);
            }
        }

        return dt;
    }

    public IReadOnlyDictionary<string, LocalizedString> GetTagsFor(int lcid)
    {
        return _localizer.Texts.Include(x => x.Descriptions.Where(l => l.LanguageId == lcid)).ToImmutableDictionary(x => x.Name, d => new LocalizedString(d.Name, d.Descriptions.First().Name));
    }

    public void ExportDescriptions(int lcid, string path)
    {
        var lcids = new HashSet<int> { 2057, lcid };
        var texts = _localizer.Texts.Include(
            x => x.Descriptions
               .Where(d => lcids.Contains(d.LanguageId))
        ).Select(
            c => new TextExport
            {
                Id = c.Id,
                Source = c.Descriptions.FirstOrDefault(d => d.LanguageId == 1033).Name ?? "---",
                Target = c.Descriptions.FirstOrDefault(d => d.LanguageId == lcid).Name ?? "---"
            }
        ).ToList();

        using var stream = new StreamWriter(path);
        using var writer = new CsvWriter(stream, new CultureInfo("de-DE", false));
        writer.Context.RegisterClassMap(new TranslateTextMap("en-US", new CultureInfo(lcid).Name));
        writer.WriteRecords(texts);
    }

    public void ImportDescriptions(string path)
    {
        using var stream = new StreamReader(path);
        using var reader = new CsvReader(stream, new CultureInfo("de-DE"));
        reader.Read();
        reader.ReadHeader();
        reader.Context.RegisterClassMap(new TranslateTextMap("en-US", reader.HeaderRecord[3]));

        var records = reader.GetRecords<TextExport>();

        var lcids = new HashSet<int> { 2057, new CultureInfo(reader.HeaderRecord[3]).LCID };
        var texts = _localizer.Texts.Include(
            x => x.Descriptions
               .Where(d => lcids.Contains(d.LanguageId))
        ).ToList();

        foreach (var record in records)
        {
            var text = texts.FirstOrDefault(x => x.Id == record.Id);
            if (text != null)
            {
                text.Descriptions.FirstOrDefault(x => x.LanguageId == 1033).Name = record.Source;
                text.Descriptions.FirstOrDefault(x => x.LanguageId == lcids.Last()).Name = record.Target;
                continue;
            }

            var entry = AddNewTextRow(record.Tag, record.Id);

            CreateNewEntry(record.Id, 2057, record.Source, entry);
            CreateNewEntry(record.Id, 1031, record.Target, entry);
        }

        _localizer.SaveChanges();
    }

    public TextLocalized GetDescription(int languageId, string textName)
    {
        return _localizer.Descriptions.FirstOrDefault(x => x.Text.Name == textName && x.LanguageId == languageId);
    }

    public EntityEntry<TextLocalized> SaveDescription(TextLocalized desc)
    {
        EntityEntry<TextLocalized> data;
        if (_localizer.Descriptions.Contains(desc))
            data = _localizer.Descriptions.Update(desc);
        else
            data = _localizer.Descriptions.Add(desc);

        return data;
    }

    public EntityEntry<TextLocalized> CreateNewEntry(int textId, int languageId, string description, TextTag? text = null)
    {
        var newValue = new TextLocalized() { Name = description, LanguageId = languageId, TextId = textId, Text = text };
        var entity = _localizer.Entry(newValue);
        entity.State = EntityState.Added;
        _localizer.SaveChanges();
        return entity;
    }

    public TextTag AddNewTextRow(string name, int? id = null, ICollection<TextLocalized>? descriptions = null)
    {
        var newValue = id.HasValue
            ? new TextTag() { Name = name, Id = id.Value }
            : new TextTag() { Name = name };

        ;
        if (descriptions != null)
            newValue.Descriptions = new List<TextLocalized>(descriptions);

        _localizer.Entry(newValue).State = EntityState.Added;
        _localizer.SaveChanges();
        return newValue;
    }

    public async Task<int> SaveDatabase()
    {
        return await _localizer.SaveChangesAsync();
    }
}