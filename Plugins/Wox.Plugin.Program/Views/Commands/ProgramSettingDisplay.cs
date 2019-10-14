﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wox.Plugin.Program.Views.Models;

namespace Wox.Plugin.Program.Views.Commands
{
    internal static class ProgramSettingDisplay
    {
        internal static List<ProgramSource> LoadProgramSources(this List<Settings.ProgramSource> programSources)
        {
            var list = new List<ProgramSource>();

            programSources.ForEach(x => list
                                            .Add(
                                                    new ProgramSource {
                                                                        Enabled = x.Enabled,
                                                                        Location = x.Location,
                                                                        Name = x.Name,
                                                                        UniqueIdentifier = x.UniqueIdentifier
                                                    }
                                                ));

            return list;
        }

        internal static void LoadAllApplications(this List<ProgramSource> list)
        {
            Main._win32s
                .Where(t1 => !ProgramSetting.ProgramSettingDisplayList.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier))
                .ToList()
                .ForEach(t1 => ProgramSetting.ProgramSettingDisplayList
                                                .Add(
                                                        new ProgramSource {
                                                                            Name = t1.Name,
                                                                            Location = t1.ParentDirectory,
                                                                            UniqueIdentifier = t1.UniqueIdentifier,
                                                                            Enabled = t1.Enabled
                                                        }
                                                    ));

            Main._uwps
                .Where(t1 => !ProgramSetting.ProgramSettingDisplayList.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier))
                .ToList()
                .ForEach(t1 => ProgramSetting.ProgramSettingDisplayList
                                                .Add(
                                                        new ProgramSource {
                                                                            Name = t1.DisplayName,
                                                                            Location = t1.Package.Location,
                                                                            UniqueIdentifier = t1.UniqueIdentifier,
                                                                            Enabled = t1.Enabled
                                                        })
                                                    );
        }

        internal static void SetProgramSourcesStatus(this List<ProgramSource> list, List<ProgramSource> selectedProgramSourcesToDisable, bool status)
        {
            ProgramSetting.ProgramSettingDisplayList
                .Where(t1 => selectedProgramSourcesToDisable.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && t1.Enabled != status))
                .ToList()
                .ForEach(t1 => t1.Enabled = status);

            Main._win32s
                .Where(t1 => selectedProgramSourcesToDisable.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && t1.Enabled != status))
                .ToList()
                .ForEach(t1 => t1.Enabled = status);

            Main._uwps
                .Where(t1 => selectedProgramSourcesToDisable.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && t1.Enabled != status))
                .ToList()
                .ForEach(t1 => t1.Enabled = status);
        }

        internal static void StoreDisabledInSettings(this List<ProgramSource> list)
        {
            Main._settings.ProgramSources
               .Where(t1 => ProgramSetting.ProgramSettingDisplayList.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && !x.Enabled))
               .ToList()
               .ForEach(t1 => t1.Enabled = false);

            ProgramSetting.ProgramSettingDisplayList
                .Where(t1 => !t1.Enabled
                                && !Main._settings.DisabledProgramSources.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier))
                .ToList()
                .ForEach(x => Main._settings.DisabledProgramSources
                                            .Add(
                                                    new Settings.DisabledProgramSource
                                                    {
                                                        Name = x.Name,
                                                        Location = x.Location,
                                                        UniqueIdentifier = x.UniqueIdentifier,
                                                        Enabled = false
                                                    }
                                                ));            
        }

        internal static void RemoveDisabledFromSettings(this List<ProgramSource> list)
        {
            Main._settings.ProgramSources
               .Where(t1 => ProgramSetting.ProgramSettingDisplayList.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && x.Enabled))
               .ToList()
               .ForEach(t1 => t1.Enabled = true);

            Main._settings.DisabledProgramSources
                .Where(t1 => ProgramSetting.ProgramSettingDisplayList.Any(x => x.UniqueIdentifier == t1.UniqueIdentifier && x.Enabled))
                .ToList()
                .ForEach(x => Main._settings.DisabledProgramSources.Remove(x));
        }
    }
}
