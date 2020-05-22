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

namespace KNTLibrary.Components.Kingdoms
{
    public class BaseKingdomManager<InfoType> : IBaseManager<InfoType, Kingdom> where InfoType : BaseKingdomInfo, new()
    {
        #region Singleton

        static BaseKingdomManager()
        {
            Instance = new BaseKingdomManager<InfoType>();
        }

        public static BaseKingdomManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms)
            {
                this.Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Kingdoms.Any(go => go.StringId == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType Get(Kingdom gameObject)
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

        public Kingdom GetGameObject(string id)
        {
            return Campaign.Current.Kingdoms.FirstOrDefault(go => go.StringId == id);
        }

        public Kingdom GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
        }

        #endregion

        public void ModifyKingdomList(Func<List<Kingdom>, List<Kingdom>> modificator)
        {
            var kingdoms = modificator(Campaign.Current.Kingdoms.ToList());
            if (kingdoms != null)
            {
                AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
            }
        }

        public void AddKingdom(Kingdom kingdom)
        {
            this.ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.Contains(kingdom))
                {
                    return null;
                }

                kingdoms.Add(kingdom);
                return kingdoms;
            });

            this.Get(kingdom).IsCustomKingdom = true;
        }

        public void RemoveKingdom(Kingdom kingdom)
        {
            this.ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.RemoveAll(go => go.StringId == kingdom.StringId) > 0)
                {
                    return kingdoms;
                }

                return null;
            });

            this.Remove(kingdom.StringId);
        }

        public void DestroyKingdom(Kingdom kingdom)
        {
            DestroyKingdomAction.Apply(kingdom);
            this.RemoveKingdom(kingdom);
        }

        public Kingdom CreateKingdom(Hero leader, TextObject name, TextObject informalName, Banner banner = null, bool showNotification = false)
        {
            var kingdom = MBObjectManager.Instance.CreateObject<Kingdom>();
            kingdom.InitializeKingdom(name, informalName, leader.Culture, banner ?? Banner.CreateRandomClanBanner(leader.StringId.GetDeterministicHashCode()), leader.Clan.Color, leader.Clan.Color2, leader.Clan.InitialPosition);

            ChangeKingdomAction.ApplyByJoinToKingdom(leader.Clan, kingdom, showNotification);
            kingdom.RulingClan = leader.Clan;

            AccessTools.Property(typeof(Kingdom), "AlternativeColor").SetValue(kingdom, leader.Clan.Color);
            AccessTools.Property(typeof(Kingdom), "AlternativeColor2").SetValue(kingdom, leader.Clan.Color2);
            AccessTools.Property(typeof(Kingdom), "LabelColor").SetValue(kingdom, ColorHelper.Black.ToUnsignedInteger());

            kingdom.AddPolicy(DefaultPolicies.RoyalGuard);
            kingdom.AddPolicy(DefaultPolicies.NobleRetinues);

            this.AddKingdom(kingdom);
            return kingdom;
        }
    }
}