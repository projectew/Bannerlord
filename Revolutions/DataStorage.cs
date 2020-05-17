using HarmonyLib;
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
using Revolutions.Settings;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core;

namespace Revolutions
{
    internal class DataStorage
    {
        internal void LoadBaseData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

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

        internal void LoadRevoltData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            Managers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(saveDirectory, "Revolts").ToHashSet();
        }

        internal void LoadCivilWarData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            Managers.CivilWar.CivilWars = FileHelper.Load<List<CivilWar>>(saveDirectory, "CivilWars").ToHashSet();
        }

        internal void SaveBaseData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            FileHelper.Save(Managers.Faction.Infos, saveDirectory, "Factions");
            FileHelper.Save(Managers.Kingdom.Infos, saveDirectory, "Kingdoms");
            FileHelper.Save(Managers.Clan.Infos, saveDirectory, "Clans");
            FileHelper.Save(Managers.Party.Infos, saveDirectory, "Parties");
            FileHelper.Save(Managers.Character.Infos, saveDirectory, "Characters");
            FileHelper.Save(Managers.Settlement.Infos, saveDirectory, "Settlements");
            FileHelper.Save(Managers.Banner.Infos, saveDirectory, "Banners");
        }

        internal void SaveRevoltData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            FileHelper.Save(Managers.Revolt.Revolts, saveDirectory, "Revolts");
        }

        internal void SaveCivilWarData()
        {
            var saveDirectory = this.GetSaveDirectory();
            if (string.IsNullOrEmpty(saveDirectory))
            {
                return;
            }

            FileHelper.Save(Managers.CivilWar.CivilWars, saveDirectory, "CivilWars");
        }

        private string GetSaveDirectory()
        {
            var activeSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName")?.GetValue(null)?.ToString();
            if (activeSaveSlotName == null)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.DataStorage: The SaveSlot does not exists yet. Please save again!", ColorHelper.Red));
                return string.Empty;
            }

            return Path.Combine(SubModule.BaseSavePath, activeSaveSlotName);
        }
    }
}