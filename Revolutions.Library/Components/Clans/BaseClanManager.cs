using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Revolutions.Library.Components.Clans
{
    public class BaseClanManager<TInfoType> : IBaseManager<TInfoType, Clan> where TInfoType : BaseClanInfo, new()
    {
        #region Singleton

        static BaseClanManager()
        {
            Instance = new BaseClanManager<TInfoType>();
        }

        public static BaseClanManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Clans.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans)
            {
                Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Clans.Count)
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Clans.All(go => go.StringId != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType Get(Clan gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.StringId);
            var baseClanInfos = infos as TInfoType[] ?? infos.ToArray();
            if (baseClanInfos.Count() > 1)
            {
                RemoveDuplicates();
            }

            var info = baseClanInfos.FirstOrDefault();
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

        public Clan GetGameObject(string id)
        {
            return Campaign.Current.Clans.FirstOrDefault(go => go.StringId == id);
        }

        public Clan GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
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

            Get(clan).IsCustomClan = true;
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
            ModifyClanList(clans =>
            {
                return clans.RemoveAll(go => go.StringId == clan.StringId) > 0 ? clans : null;
            });

            Remove(clan.StringId);
        }

        public void DestroyClan(Clan clan)
        {
            DestroyClanAction.Apply(clan);
            RemoveClan(clan);
        }
    }
}