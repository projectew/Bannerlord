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

namespace Revolts
{
    public class DataStorage
    {
        internal string SaveId { get; set; } = string.Empty;

        internal void InitializeData()
        {
            RevoltsManagers.Kingdom.InitializeInfos();
            RevoltsManagers.Faction.InitializeInfos();
            RevoltsManagers.Clan.InitializeInfos();
            RevoltsManagers.Party.InitializeInfos();
            RevoltsManagers.Character.InitializeInfos();
            RevoltsManagers.Settlement.InitializeInfos();
        }

        internal void LoadData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            RevoltsManagers.Faction.Infos = FileHelper.Load<List<FactionInfo>>(directoryPath, "Factions").ToHashSet();
            RevoltsManagers.Faction.CleanupDuplicatedInfos();

            RevoltsManagers.Kingdom.Infos = FileHelper.Load<List<KingdomInfo>>(directoryPath, "Kingdoms").ToHashSet();
            RevoltsManagers.Kingdom.CleanupDuplicatedInfos();

            RevoltsManagers.Clan.Infos = FileHelper.Load<List<ClanInfo>>(directoryPath, "Clans").ToHashSet();
            RevoltsManagers.Clan.CleanupDuplicatedInfos();

            RevoltsManagers.Party.Infos = FileHelper.Load<List<PartyInfo>>(directoryPath, "Parties").ToHashSet();
            RevoltsManagers.Party.CleanupDuplicatedInfos();

            RevoltsManagers.Character.Infos = FileHelper.Load<List<CharacterInfo>>(directoryPath, "Characters").ToHashSet();
            RevoltsManagers.Character.CleanupDuplicatedInfos();

            RevoltsManagers.Settlement.Infos = FileHelper.Load<List<SettlementInfo>>(directoryPath, "Settlements").ToHashSet();
            RevoltsManagers.Settlement.CleanupDuplicatedInfos();

            RevoltsManagers.Revolt.Revolts = FileHelper.Load<List<Revolt>>(directoryPath, "Revolts").ToHashSet();
        }

        internal void SaveData()
        {
            var directoryPath = Path.Combine(SubModule.BaseSavePath, this.SaveId);

            FileHelper.Save(RevoltsManagers.Faction.Infos, directoryPath, "Factions");
            FileHelper.Save(RevoltsManagers.Kingdom.Infos, directoryPath, "Kingdoms");
            FileHelper.Save(RevoltsManagers.Clan.Infos, directoryPath, "Clans");
            FileHelper.Save(RevoltsManagers.Party.Infos, directoryPath, "Parties");
            FileHelper.Save(RevoltsManagers.Character.Infos, directoryPath, "Characters");
            FileHelper.Save(RevoltsManagers.Settlement.Infos, directoryPath, "Settlements");
            FileHelper.Save(RevoltsManagers.Revolt.Revolts, directoryPath, "Revolts");
        }
    }
}