using HarmonyLib;
using KNTLibrary.Helpers;
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

        private BaseClanManager() { }

        static BaseClanManager()
        {
            BaseClanManager<InfoType>.Instance = new BaseClanManager<InfoType>();
        }

        public static BaseClanManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public bool DebugMode { get; set; }

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Clans.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(Clan gameObject)
        {
            var infos = this.Infos.ToList().Where(i => i.ClanId == gameObject.StringId);
            if (this.DebugMode && infos.Count() > 1)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Multiple Clans with same Id. Using first one.", ColorHelper.Orange));
                foreach (var duplicatedInfo in infos)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Name: {duplicatedInfo.Clan.Name} | StringId: {duplicatedInfo.ClanId}", ColorHelper.Orange));
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
            var info = this.Infos.FirstOrDefault(i => i.ClanId == id);
            if (id == null)
            {
                return;
            }

            this.Infos.RemoveWhere(i => i.ClanId == id);
        }

        public Clan GetGameObject(string id)
        {
            return Campaign.Current.Clans.FirstOrDefault(go => go.StringId == id);
        }

        public Clan GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.ClanId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count == Campaign.Current.Clans.Count)
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Clans.Any(go => go.StringId == i.ClanId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Clans.Where(go => !this.Infos.Any(i => i.ClanId == go.StringId)))
            {
                this.GetInfo(gameObject);
            }
        }

        public void CleanupDuplicatedInfos()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.ClanId)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        #endregion

        public Clan CreateClan(Hero leader, TextObject name, TextObject informalName, Banner banner = null)
        {
            var clanName = NameGenerator.Current.GenerateClanName(leader.Culture, leader.HomeSettlement);

            var clan = MBObjectManager.Instance.CreateObject<Clan>();
            clan.InitializeClan(name ?? clanName, informalName ?? clanName, leader.Culture, banner ?? Banner.CreateRandomClanBanner(leader.StringId.GetDeterministicHashCode()));
            clan.SetLeader(leader);
            clan.AddRenown(1000, false);

            this.GetInfo(clan).IsCustomClan = true;
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
            this.ModifyClanList(gos =>
            {
                if (gos.RemoveAll(go => go.StringId == clan.StringId) > 0)
                {
                    return gos;
                }

                return null;
            });

            this.RemoveInfo(clan.StringId);
        }

        public void DestroyClan(Clan clan)
        {
            DestroyClanAction.Apply(clan);
            this.RemoveClan(clan);
        }
    }
}