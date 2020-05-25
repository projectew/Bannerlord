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

        static BaseFactionManager()
        {
            Instance = new BaseFactionManager<InfoType>();
        }

        public static BaseFactionManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Factions.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Factions)
            {
                this.Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Factions.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Factions.ToList().Any(go => go.StringId == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType Get(IFaction gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = this.Infos.Where(i => i.Id == gameObject.StringId);
            if (infos.Count() > 1)
            {
                this.RemoveDuplicates();
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

        public InfoType Get(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.Get(gameObject);
        }

        public void Remove(string id)
        {
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

        #endregion

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            var validNobles = faction.Heroes.Where(hero => hero.StringId != Hero.MainHero.StringId && !hero.IsPrisoner && hero.IsAlive && !hero.IsOccupiedByAnEvent() && !hero.PartyBelongedTo.IsJoiningArmy && hero.PartyBelongedTo.Army != null && !hero.Noncombatant && hero.CharacterObject.Occupation == Occupation.Lord || hero.CharacterObject.Occupation == Occupation.Lady);
            var noble = validNobles.Aggregate((currentResult, current) => current.Clan.Settlements.Count() < currentResult.Clan.Settlements.Count() ? current : currentResult);
            return noble.Clan != null ? noble.CharacterObject : faction.Leader.CharacterObject;
        }
    }
}