using KNTLibrary.Components.Banners;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.Base.Clans;
using Revolutions.Components.Base.Factions;
using Revolutions.Components.Base.Kingdoms;
using Revolutions.Components.Base.Parties;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.CivilWars;
using Revolutions.Components.Revolts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Revolutions
{
    internal class DataStorage
    {
        internal static string ActiveSaveSlotName { get; set; }

        internal static void InitializeBaseData()
        {
            Managers.Faction.InitializeInfos();
            Managers.Faction.CleanupDuplicatedInfos();

            Managers.Kingdom.InitializeInfos();
            Managers.Kingdom.CleanupDuplicatedInfos();

            Managers.Clan.InitializeInfos();
            Managers.Clan.CleanupDuplicatedInfos();

            Managers.Party.InitializeInfos();
            Managers.Party.CleanupDuplicatedInfos();

            Managers.Character.InitializeInfos();
            Managers.Character.CleanupDuplicatedInfos();

            Managers.Settlement.InitializeInfos();
            Managers.Settlement.CleanupDuplicatedInfos();

            Managers.Banner.InitializeInfos();
            Managers.Banner.CleanupDuplicatedInfos();
        }

        internal static void LoadBaseData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            Managers.Faction.Infos = FileHelper.Load<List<FactionInfo>>(saveDirectory, "Factions").ToHashSet();
            Managers.Faction.CleanupDuplicatedInfos();

            Managers.Kingdom.Infos = FileHelper.Load<List<KingdomInfo>>(saveDirectory, "Kingdoms").ToHashSet();
            Managers.Kingdom.CleanupDuplicatedInfos();

            Managers.Clan.Infos = FileHelper.Load<List<ClanInfo>>(saveDirectory, "Clans").ToHashSet();
            Managers.Clan.CleanupDuplicatedInfos();

            Managers.Party.Infos = FileHelper.Load<List<PartyInfo>>(saveDirectory, "Parties").ToHashSet();
            Managers.Party.CleanupDuplicatedInfos();

            Managers.Character.Infos = FileHelper.Load<List<CharacterInfo>>(saveDirectory, "Characters").ToHashSet();
            Managers.Character.CleanupDuplicatedInfos();

            Managers.Settlement.Infos = FileHelper.Load<List<SettlementInfo>>(saveDirectory, "Settlements").ToHashSet();
            Managers.Settlement.CleanupDuplicatedInfos();

            Managers.Banner.Infos = FileHelper.Load<List<BaseBannerInfo>>(saveDirectory, "Banners").ToHashSet();
            Managers.Banner.CleanupDuplicatedInfos();
        }

        internal static void SaveBaseData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            FileHelper.Save(Managers.Faction.Infos, saveDirectory, "Factions");
            FileHelper.Save(Managers.Kingdom.Infos, saveDirectory, "Kingdoms");
            FileHelper.Save(Managers.Clan.Infos, saveDirectory, "Clans");
            FileHelper.Save(Managers.Party.Infos, saveDirectory, "Parties");
            FileHelper.Save(Managers.Character.Infos, saveDirectory, "Characters");
            FileHelper.Save(Managers.Settlement.Infos, saveDirectory, "Settlements");
            FileHelper.Save(Managers.Banner.Infos, saveDirectory, "Banners");
        }

        internal static void LoadRevoltData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            Managers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(saveDirectory, "Revolts").ToHashSet();
        }

        internal static void SaveRevoltData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            FileHelper.Save(Managers.Revolt.Revolts, saveDirectory, "Revolts");
        }

        internal static void LoadCivilWarData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            Managers.CivilWar.CivilWars = FileHelper.Load<List<CivilWar>>(saveDirectory, "CivilWars").ToHashSet();
        }

        internal static void SaveCivilWarData()
        {
            var saveDirectory = DataStorage.GetSaveDirectory();

            FileHelper.Save(Managers.CivilWar.CivilWars, saveDirectory, "CivilWars");
        }

        private static string GetSaveDirectory()
        {
            return Path.Combine(SubModule.BaseSavePath, DataStorage.ActiveSaveSlotName);
        }
    }
}