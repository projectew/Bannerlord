using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Factions
{
    public class BaseFactionManager<TInfoType> /*: IBaseManager<InfoType, IFaction>*/ where TInfoType : BaseFactionInfo, new()
    {
        #region Singleton

        static BaseFactionManager()
        {
            Instance = new BaseFactionManager<TInfoType>();
        }

        public static BaseFactionManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Factions.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Factions)
            {
                Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Factions.Count())
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Factions.ToList().All(go => go.StringId != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType Get(IFaction gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.StringId);
            var baseFactionInfos = infos as TInfoType[] ?? infos.ToArray();
            if (baseFactionInfos.Length > 1)
            {
                RemoveDuplicates();
            }

            var info = baseFactionInfos.FirstOrDefault();
            if (info != null)
            {
                return info;
            }

            info = (TInfoType)Activator.CreateInstance(typeof(TInfoType), gameObject);
            Infos.Add(info);

            return info;
        }

        public TInfoType Get(string id)
        {
            var gameObject = GetGameObject(id);
            return gameObject == null ? null : Get(gameObject);
        }

        public void Remove(string id)
        {
            Infos.RemoveWhere(i => i.Id == id);
        }

        public IFaction GetGameObject(string id)
        {
            return Campaign.Current.Factions.FirstOrDefault(go => go.StringId == id);
        }

        public IFaction GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
        }

        #endregion

        public CharacterObject GetLordWithLeastFiefs(IFaction faction)
        {
            var validNobles = faction.Heroes.Where(hero => hero.StringId != Hero.MainHero.StringId && !hero.IsPrisoner && hero.IsAlive && !hero.IsOccupiedByAnEvent() && hero.PartyBelongedTo?.IsJoiningArmy == false && !hero.Noncombatant && hero.CharacterObject.Occupation == Occupation.Lord || hero.CharacterObject.Occupation == Occupation.Lady);
            var noble = validNobles.Aggregate((currentResult, current) => current.Clan.Settlements.Count() < currentResult.Clan.Settlements.Count() ? current : currentResult);
            return noble.Clan != null ? noble.CharacterObject : faction.Leader.CharacterObject;
        }
    }
}