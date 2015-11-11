﻿using System.Linq;
using System.Xml.Linq;
using Orchard.ImportExport.Models;
using Orchard.ImportExport.Services;
using Orchard.Environment.Extensions;
using IDeliverable.ThemeSettings.Services;

namespace IDeliverable.ThemeSettings.ImportExport
{
    [OrchardFeature("IDeliverable.ThemeSettings.ImportExport")]
    public class ThemeSettingsExportHandler : IExportEventHandler
    {
        private readonly IThemeSettingsService mThemeSettingsService;

        public ThemeSettingsExportHandler(IThemeSettingsService themeSettingsService)
        {
            mThemeSettingsService = themeSettingsService;
        }

        public void Exporting(ExportContext context)
        {
        }

        public void Exported(ExportContext context)
        {
            if (!context.ExportOptions.CustomSteps.Contains("ThemeSettings"))
            {
                return;
            }

            var themes = mThemeSettingsService.GetAllProfiles().ToLookup(x => x.Theme);

            if (!themes.Any())
            {
                return;
            }

            var root = new XElement("ThemeSettings");
            context.Document.Element("Orchard").Add(root);

            foreach (var theme in themes)
            {
                root.Add(new XElement("Theme",
                    new XAttribute("Name", theme.Key),
                    theme.Select(profile => new XElement("Profile",
                        new XAttribute("Name", profile.Name),
                        new XAttribute("Description", profile.Description),
                        new XAttribute("IsCurrent", profile.IsCurrent),
                        new XCData(mThemeSettingsService.SerializeSettings(profile.Settings))))));
            }
        }
    }
}