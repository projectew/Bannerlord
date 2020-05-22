using KNTLibrary.Components.Banners;
using KNTLibrary.Helpers;
using Revolutions.Components.Characters;
using Revolutions.Components.CivilWars;
using Revolutions.Components.Clans;
using Revolutions.Components.Factions;
using Revolutions.Components.Kingdoms;
using Revolutions.Components.Parties;
using Revolutions.Components.Revolts;
using Revolutions.Components.Settlements;
using Revolutions.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Revolutions
{
    internal class DataStorage
    {
        internal static string ActiveSaveSlotName { get; set; }

        internal static void LoadData()
        {
            LoadBaseData();

            if (RevolutionsSettings.Instance.EnableRevolts)
            {
                LoadRevoltData();
            }

            if (RevolutionsSettings.Instance.EnableCivilWars)
            {
                LoadCivilWarData();
            }
        }

        internal static void SaveData()
        {
            SaveBaseData();

            if (RevolutionsSettings.Instance.EnableRevolts)
            {
                SaveRevoltData();
            }

            if (RevolutionsSettings.Instance.EnableCivilWars)
            {
                SaveCivilWarData();
            }
        }

        private static void LoadBaseData()
        {
            var saveDirectory = GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                Managers.Faction.Infos.Clear();
                Managers.Kingdom.Infos.Clear();
                Managers.Clan.Infos.Clear();
                Managers.Party.Infos.Clear();
                Managers.Character.Infos.Clear();
                Managers.Settlement.Infos.Clear();
            }

            Managers.Faction.Infos = FileHelper.Load<List<FactionInfo>>(saveDirectory, "Factions").ToHashSet();
            if (Managers.Faction.Infos.Count == 0)
            {
                Managers.Faction.Initialize();
            }
            Managers.Faction.RemoveDuplicates();

            Managers.Kingdom.Infos = FileHelper.Load<List<KingdomInfo>>(saveDirectory, "Kingdoms").ToHashSet();
            if (Managers.Kingdom.Infos.Count == 0)
            {
                Managers.Kingdom.Initialize();
            }
            Managers.Kingdom.RemoveDuplicates();

            Managers.Clan.Infos = FileHelper.Load<List<ClanInfo>>(saveDirectory, "Clans").ToHashSet();
            if (Managers.Clan.Infos.Count == 0)
            {
                Managers.Clan.Initialize();
            }
            Managers.Clan.RemoveDuplicates();

            Managers.Party.Infos = FileHelper.Load<List<PartyInfo>>(saveDirectory, "Parties").ToHashSet();
            if (Managers.Party.Infos.Count == 0)
            {
                Managers.Party.Initialize();
            }
            Managers.Party.RemoveDuplicates();

            Managers.Character.Infos = FileHelper.Load<List<CharacterInfo>>(saveDirectory, "Characters").ToHashSet();
            if (Managers.Character.Infos.Count == 0)
            {
                Managers.Character.Initialize();
            }
            Managers.Character.RemoveDuplicates();

            Managers.Settlement.Infos = FileHelper.Load<List<SettlementInfo>>(saveDirectory, "Settlements").ToHashSet();
            if (Managers.Settlement.Infos.Count == 0)
            {
                Managers.Settlement.Initialize();
            }
            Managers.Settlement.RemoveDuplicates();

            Managers.Banner.Infos = FileHelper.Load<List<BaseBannerInfo>>(saveDirectory, "Banners").ToHashSet();
            if (Managers.Banner.Infos.Count == 0)
            {
                Managers.Banner.InitializeInfos();
            }
            Managers.Banner.CleanupDuplicatedInfos();
        }

        private static void SaveBaseData()
        {
            var saveDirectory = GetSaveDirectory();

            FileHelper.Save(Managers.Faction.Infos, saveDirectory, "Factions");
            FileHelper.Save(Managers.Kingdom.Infos, saveDirectory, "Kingdoms");
            FileHelper.Save(Managers.Clan.Infos, saveDirectory, "Clans");
            FileHelper.Save(Managers.Party.Infos, saveDirectory, "Parties");
            FileHelper.Save(Managers.Character.Infos, saveDirectory, "Characters");
            FileHelper.Save(Managers.Settlement.Infos, saveDirectory, "Settlements");
            FileHelper.Save(Managers.Banner.Infos, saveDirectory, "Banners");
        }

        private static void LoadRevoltData()
        {
            var saveDirectory = GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                Managers.Revolt.Revolts.Clear();
            }

            Managers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(saveDirectory, "Revolts").ToHashSet();
        }

        private static void SaveRevoltData()
        {
            var saveDirectory = GetSaveDirectory();

            FileHelper.Save(Managers.Revolt.Revolts, saveDirectory, "Revolts");
        }

        private static void LoadCivilWarData()
        {
            var saveDirectory = GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                Managers.CivilWar.CivilWars.Clear();
            }

            Managers.CivilWar.CivilWars = FileHelper.Load<List<CivilWar>>(saveDirectory, "CivilWars").ToHashSet();
        }

        private static void SaveCivilWarData()
        {
            var saveDirectory = GetSaveDirectory();

            FileHelper.Save(Managers.CivilWar.CivilWars, saveDirectory, "CivilWars");
        }

        private static string GetSaveDirectory()
        {
            if (string.IsNullOrEmpty(ActiveSaveSlotName))
            {
                return string.Empty;
            }

            return Path.Combine(SubModule.BaseSavePath, ActiveSaveSlotName);
        }
    }
}