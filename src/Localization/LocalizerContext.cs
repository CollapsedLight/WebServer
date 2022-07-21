using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Query;
using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CollapsedLight.WebApp.Localization
{
    public record TextExport
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }

    public sealed class TranslateTextMap : ClassMap<TextExport>
    {
        public TranslateTextMap(string source, string target)
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Tag).Name("Tag");
            Map(m => m.Source).Name(source);
            Map(m => m.Target).Name(target);
        }
    }

    public record Language
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Language() { }

        public Language(CultureInfo culture)
        {
            Id = culture.LCID;
            Name = culture.Name;
        }
    }

    public record TextLocalized
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Id { get; set; }

        public int TextId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual TextTag Text { get; set; }
    }

    public record TextTag
    {
        [Key]
        public int Id { get; set; }

        public string Name { set; get; }

        public virtual ICollection<TextLocalized> Descriptions { get; set; }
    }

    public class LocalizerContext : DbContext
    {
        public DbSet<Language> Languages { get; private set; }
        public DbSet<TextLocalized> Descriptions { get; private set; }
        public DbSet<TextTag> Texts { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=localization.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>().HasData(
                new List<Language>
                {
                    new Language(new CultureInfo("de-DE")),
                    new Language(new CultureInfo("en-US")),
                }
            );


            var desc = new List<TextLocalized>
            {
                new TextLocalized { Id = 1, LanguageId  = 1031, TextId = 1, Name = "Localisierung" },
                new TextLocalized { Id = 2, LanguageId  = 1033, TextId = 1, Name = "Localizer" },
            };

            modelBuilder.Entity<TextLocalized>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<TextLocalized>().HasData(desc);
            modelBuilder.Entity<TextLocalized>().HasOne(x => x.Text).WithMany(x => x.Descriptions).HasForeignKey(x => x.TextId).IsRequired(true);

            var texts = new List<TextTag>
            {
                new TextTag { Id = 1, Name = "localization" },
            };

            modelBuilder.Entity<TextTag>().HasData(texts);
            modelBuilder.Entity<TextTag>().HasMany(x => x.Descriptions).WithOne(x => x.Text).HasForeignKey(x => x.TextId);

            base.OnModelCreating(modelBuilder);
        }
    }
}