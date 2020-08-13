using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Revolutions.Library.Components.Parties
{
    public class BasePartyManager<TInfoType> /*: IBaseManager<InfoType, PartyBase>*/ where TInfoType : BasePartyInfo, new()
    {
        #region Singleton

        static BasePartyManager()
        {
            Instance = new BasePartyManager<TInfoType>();
        }

        public static BasePartyManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Parties.Count)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Parties)
            {
                GetInfo(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Parties.Count)
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Parties.All(go => go.Id != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType GetInfo(PartyBase gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.Id);
            var basePartyInfos = infos as TInfoType[] ?? infos.ToArray();
            if (basePartyInfos.Length > 1)
            {
                RemoveDuplicates();
            }

            var info = basePartyInfos.FirstOrDefault();
            if (info != null)
            {
                return info;
            }

            info = (TInfoType)Activator.CreateInstance(typeof(TInfoType), gameObject);
            Infos.Add(info);

            return info;
        }

        public TInfoType GetInfo(string id)
        {
            var gameObject = GetGameObject(id);
            return gameObject == null ? null : GetInfo(gameObject);
        }

        public void Remove(string id)
        {
            Infos.RemoveWhere(i => i.Id == id);
        }

        public PartyBase GetGameObject(string id)
        {
            return Campaign.Current.Parties.FirstOrDefault(go => go.Id == id);
        }

        public PartyBase GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
        }

        #endregion

        public MobileParty CreateMobileParty(Hero leader, TextObject name, Vec2 spawnPosition, Settlement homeSettlement, bool addLeaderToRoster, bool addInitialFood = true)
        {
            var mobileParty = MBObjectManager.Instance.CreateObject<MobileParty>(leader.CharacterObject.StringId + "_" + leader.NumberOfCreatedParties);
            ++leader.NumberOfCreatedParties;

            if (addLeaderToRoster)
            {
                mobileParty.AddElementToMemberRoster(leader.CharacterObject, 1, true);
            }

            name = name ?? MobilePartyHelper.GeneratePartyName(leader.CharacterObject);
            mobileParty.InitializeMobileParty(name, leader.Clan.DefaultPartyTemplate, spawnPosition, 5f, 0.0f, MobileParty.PartyTypeEnum.Default, 0);
            mobileParty.Party.Owner = leader;
            mobileParty.IsLordParty = true;
            mobileParty.HomeSettlement = homeSettlement;
            mobileParty.Quartermaster = leader;
            mobileParty.Party.Visuals.SetMapIconAsDirty();

            if (addInitialFood)
            {
                var foodItem = Campaign.Current.Items.Where(item => item.IsFood).GetRandomElement();
                mobileParty.ItemRoster.AddToCounts(foodItem, new Random().Next(100, 300));
            }

            GetInfo(mobileParty.Party).IsCustomParty = true;
            return mobileParty;
        }
    }
}