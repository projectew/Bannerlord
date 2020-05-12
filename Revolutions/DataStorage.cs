using System.IO;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.Core;
using KNTLibrary.Components.Banners;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.Base.Parties;
using Revolutions.Components.Base.Kingdoms;
using Revolutions.Components.Base.Factions;
using Revolutions.Components.Base.Clans;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.Revolts;
using Revolutions.Components.CivilWars;

namespace Revolutions
{
    internal class DataStorage
    {
        internal void InitializeBaseData()
        {
            RevolutionsManagers.Faction.DebugMode = Settings.Instance.DebugMode;
            RevolutionsManagers.Kingdom.DebugMode = Settings.Instance.DebugMode;
            RevolutionsManagers.Clan.DebugMode = Settings.Instance.DebugMode;
            RevolutionsManagers.Party.DebugMode = Settings.Instance.DebugMode;
            RevolutionsManagers.Character.DebugMode = Settings.Instance.DebugMode;
            RevolutionsManagers.Settlement.DebugMode = Settings.Instance.DebugMode;

            RevolutionsManagers.Kingdom.InitializeInfos();
            RevolutionsManagers.Faction.InitializeInfos();
            RevolutionsManagers.Clan.InitializeInfos();
            RevolutionsManagers.Party.InitializeInfos();
            RevolutionsManagers.Character.InitializeInfos();
            RevolutionsManagers.Settlement.InitializeInfos();
        }

        internal void LoadBaseData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            RevolutionsManagers.Faction.Infos = FileHelper.Load<List<FactionInfo>>(saveDirectory, "Factions").ToHashSet();
            RevolutionsManagers.Faction.CleanupDuplicatedInfos();

            RevolutionsManagers.Kingdom.Infos = FileHelper.Load<List<KingdomInfo>>(saveDirectory, "Kingdoms").ToHashSet();
            RevolutionsManagers.Kingdom.CleanupDuplicatedInfos();

            RevolutionsManagers.Clan.Infos = FileHelper.Load<List<ClanInfo>>(saveDirectory, "Clans").ToHashSet();
            RevolutionsManagers.Clan.CleanupDuplicatedInfos();

            RevolutionsManagers.Party.Infos = FileHelper.Load<List<PartyInfo>>(saveDirectory, "Parties").ToHashSet();
            RevolutionsManagers.Party.CleanupDuplicatedInfos();

            RevolutionsManagers.Character.Infos = FileHelper.Load<List<CharacterInfo>>(saveDirectory, "Characters").ToHashSet();
            RevolutionsManagers.Character.CleanupDuplicatedInfos();

            RevolutionsManagers.Settlement.Infos = FileHelper.Load<List<SettlementInfo>>(saveDirectory, "Settlements").ToHashSet();
            RevolutionsManagers.Settlement.CleanupDuplicatedInfos();

            RevolutionsManagers.Banner.Infos = FileHelper.Load<List<BaseBannerInfo>>(saveDirectory, "Banners").ToHashSet();
            RevolutionsManagers.Banner.CleanupDuplicatedInfos();
        }

        internal void LoadRevoltData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            RevolutionsManagers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(saveDirectory, "Revolts").ToHashSet();
        }

        internal void LoadCivilWarData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            RevolutionsManagers.CivilWar.CivilWars = FileHelper.Load<List<CivilWar>>(saveDirectory, "CivilWars").ToHashSet();
        }

        internal void SaveBaseData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if(string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            FileHelper.Save(RevolutionsManagers.Faction.Infos, saveDirectory, "Factions");
            FileHelper.Save(RevolutionsManagers.Kingdom.Infos, saveDirectory, "Kingdoms");
            FileHelper.Save(RevolutionsManagers.Clan.Infos, saveDirectory, "Clans");
            FileHelper.Save(RevolutionsManagers.Party.Infos, saveDirectory, "Parties");
            FileHelper.Save(RevolutionsManagers.Character.Infos, saveDirectory, "Characters");
            FileHelper.Save(RevolutionsManagers.Settlement.Infos, saveDirectory, "Settlements");
			FileHelper.Save(RevolutionsManagers.Banner.Infos, saveDirectory, "Banners");
        }

        internal void SaveRevoltData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.GetSaveDirectory());

            FileHelper.Save(RevolutionsManagers.Revolt.Revolts, directoryPath, "Revolts");
        }

        internal void SaveCivilWarData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.GetSaveDirectory());

            FileHelper.Save(RevolutionsManagers.CivilWar.CivilWars, directoryPath, "CivilWars");
        }

        private string GetSaveDirectory()
        {
            var activeSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName")?.GetValue(null)?.ToString();
            if(activeSaveSlotName == null)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.DataStorage: The SaveSlot does not exists yet. Please save again!", ColorHelper.Red));
                return string.Empty;
            }

            return Path.Combine(SubModule.BaseSavePath, activeSaveSlotName);
        }
    }
}