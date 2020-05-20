using KNTLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace KNTLibrary.Components.Factions
{
    public class BaseFactionManager<InfoType> /*: IBaseManager<InfoType, IFaction>*/ where InfoType : BaseFactionInfo, new()
    {
        #region Singleton

        private BaseFactionManager() { }

        static BaseFactionManager()
        {
            BaseFactionManager<InfoType>.Instance = new BaseFactionManager<InfoType>();
        }

        public static BaseFactionManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public bool DebugMode { get; set; }

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count == Campaign.Current?.Factions?.ToList()?.Count)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Factions)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(IFaction gameObject)
        {
            var infos = this.Infos.Where(i => i.Id == gameObject.StringId);
            if (this.DebugMode && infos.Count() > 1)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Multiple Factions with same Id. Using first one.", ColorHelper.Orange));
                foreach (var duplicatedInfo in infos)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Name: {duplicatedInfo.Faction.Name} | StringId: {duplicatedInfo.Id}", ColorHelper.Orange));
                }
            }

            var info = infos.FirstOrDefault();
            if (info != null)
            {
                return info;
            }

            info = (InfoType)Activator.CreateInstance(typeof(InfoType), gameObject);
            this.Infos.Add(info);

            return info;
        }

        public InfoType GetInfo(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void RemoveInfo(string id)
        {
            var info = this.Infos.FirstOrDefault(i => i.Id == id);
            if (info == null)
            {
                return;
            }

            this.Infos.RemoveWhere(i => i.Id == id);
        }

        public IFaction GetGameObject(string id)
        {
            return Campaign.Current.Factions.FirstOrDefault(go => go.StringId == id);
        }

        public IFaction GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count == Campaign.Current.Factions.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Factions.ToList().Any(go => go.StringId == i.Id));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var faction in Campaign.Current.Factions.Where(go => !this.Infos.Any(i => i.Id == go.StringId)))
            {
                this.GetInfo(faction);
            }
        }

        public void CleanupDuplicatedInfos()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        #endregion

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            var noble = faction.Nobles.Aggregate((currentResult, current) => current.Clan.Settlements.Count() < currentResult.Clan.Settlements.Count() ? current : currentResult);
            return noble.Clan != null ? noble.Clan.Nobles.GetRandomElement().CharacterObject : faction.Leader.CharacterObject;
        }
    }
}