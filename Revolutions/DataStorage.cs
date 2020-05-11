using System.IO;
using System.Collections.Generic;
using System.Linq;
using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.Base.Parties;
using Revolutions.Components.Base.Kingdoms;
using Revolutions.Components.Base.Factions;
using Revolutions.Components.Base.Clans;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.Revolts;
using Revolutions;

namespace Revolts
{
    public class DataStorage
    {
        internal string SaveId { get; set; } = string.Empty;

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
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            RevolutionsManagers.Faction.Infos = FileHelper.Load<List<FactionInfo>>(directoryPath, "Factions").ToHashSet();
            RevolutionsManagers.Faction.CleanupDuplicatedInfos();

            RevolutionsManagers.Kingdom.Infos = FileHelper.Load<List<KingdomInfo>>(directoryPath, "Kingdoms").ToHashSet();
            RevolutionsManagers.Kingdom.CleanupDuplicatedInfos();

            RevolutionsManagers.Clan.Infos = FileHelper.Load<List<ClanInfo>>(directoryPath, "Clans").ToHashSet();
            RevolutionsManagers.Clan.CleanupDuplicatedInfos();

            RevolutionsManagers.Party.Infos = FileHelper.Load<List<PartyInfo>>(directoryPath, "Parties").ToHashSet();
            RevolutionsManagers.Party.CleanupDuplicatedInfos();

            RevolutionsManagers.Character.Infos = FileHelper.Load<List<CharacterInfo>>(directoryPath, "Characters").ToHashSet();
            RevolutionsManagers.Character.CleanupDuplicatedInfos();

            RevolutionsManagers.Settlement.Infos = FileHelper.Load<List<SettlementInfo>>(directoryPath, "Settlements").ToHashSet();
            RevolutionsManagers.Settlement.CleanupDuplicatedInfos();
        }

        internal void LoadRevoltData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            RevolutionsManagers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(directoryPath, "Revolts").ToHashSet();
        }

        internal void LoadCivilWarData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            RevolutionsManagers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(directoryPath, "CivilWars").ToHashSet();
        }

        internal void SaveBaseData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            FileHelper.Save(RevolutionsManagers.Faction.Infos, directoryPath, "Factions");
            FileHelper.Save(RevolutionsManagers.Kingdom.Infos, directoryPath, "Kingdoms");
            FileHelper.Save(RevolutionsManagers.Clan.Infos, directoryPath, "Clans");
            FileHelper.Save(RevolutionsManagers.Party.Infos, directoryPath, "Parties");
            FileHelper.Save(RevolutionsManagers.Character.Infos, directoryPath, "Characters");
            FileHelper.Save(RevolutionsManagers.Settlement.Infos, directoryPath, "Settlements");
        }

        internal void SaveRevoltData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            FileHelper.Save(RevolutionsManagers.CivilWar.CivilWars, directoryPath, "Revolts");
        }

        internal void SaveCivilWarData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            FileHelper.Save(RevolutionsManagers.CivilWar.CivilWars, directoryPath, "CivilWars");
        }
    }
}