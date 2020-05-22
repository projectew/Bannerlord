using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace KNTLibrary.Components.Parties
{
    public class BasePartyManager<InfoType> /*: IBaseManager<InfoType, PartyBase>*/ where InfoType : BasePartyInfo, new()
    {
        #region Singleton

        static BasePartyManager()
        {
            Instance = new BasePartyManager<InfoType>();
        }

        public static BasePartyManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Parties.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Parties)
            {
                this.GetInfo(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Parties.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Parties.Any(go => go.Id == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType GetInfo(PartyBase gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = this.Infos.Where(i => i.Id == gameObject.Id);
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

        public InfoType GetInfo(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void Remove(string id)
        {
            this.Infos.RemoveWhere(i => i.Id == id);
        }

        public PartyBase GetGameObject(string id)
        {
            return Campaign.Current.Parties.FirstOrDefault(go => go.Id == id);
        }

        public PartyBase GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
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

            this.GetInfo(mobileParty.Party).IsCustomParty = true;
            return mobileParty;
        }
    }
}