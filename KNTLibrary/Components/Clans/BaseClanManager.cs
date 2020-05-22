using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace KNTLibrary.Components.Clans
{
    public class BaseClanManager<InfoType> : IBaseManager<InfoType, Clan> where InfoType : BaseClanInfo, new()
    {
        #region Singleton

        static BaseClanManager()
        {
            Instance = new BaseClanManager<InfoType>();
        }

        public static BaseClanManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Clans.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans)
            {
                this.Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Clans.Count)
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Clans.Any(go => go.StringId == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType Get(Clan gameObject)
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

        public Clan GetGameObject(string id)
        {
            return Campaign.Current.Clans.FirstOrDefault(go => go.StringId == id);
        }

        public Clan GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
        }

        #endregion

        public Clan CreateClan(Hero leader, TextObject name, TextObject informalName, Banner banner = null)
        {
            var clanName = NameGenerator.Current.GenerateClanName(leader.Culture, leader.HomeSettlement);

            var clan = MBObjectManager.Instance.CreateObject<Clan>();
            clan.InitializeClan(name ?? clanName, informalName ?? clanName, leader.Culture, banner ?? Banner.CreateRandomClanBanner(leader.StringId.GetDeterministicHashCode()));
            clan.SetLeader(leader);
            clan.AddRenown(1000, false);

            leader.Clan = clan;

            this.Get(clan).IsCustomClan = true;
            return clan;
        }

        public void ModifyClanList(Func<List<Clan>, List<Clan>> modificator)
        {
            var clans = modificator(Campaign.Current.Clans.ToList());
            if (clans != null)
            {
                AccessTools.Field(Campaign.Current.GetType(), "_clans").SetValue(Campaign.Current, new MBReadOnlyList<Clan>(clans));
            }
        }

        public void RemoveClan(Clan clan)
        {
            this.ModifyClanList(clans =>
            {
                if (clans.RemoveAll(go => go.StringId == go.StringId) > 0)
                {
                    return clans;
                }

                return null;
            });

            this.Remove(clan.StringId);
        }

        public void DestroyClan(Clan clan)
        {
            DestroyClanAction.Apply(clan);
            this.RemoveClan(clan);
        }
    }
}