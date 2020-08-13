using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Revolutions.Library.Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Revolutions.Library.Components.Kingdoms
{
    public class BaseKingdomManager<TInfoType> : IBaseManager<TInfoType, Kingdom> where TInfoType : BaseKingdomInfo, new()
    {
        #region Singleton

        static BaseKingdomManager()
        {
            Instance = new BaseKingdomManager<TInfoType>();
        }

        public static BaseKingdomManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Kingdoms)
            {
                Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Kingdoms.Count())
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Kingdoms.All(go => go.StringId != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType Get(Kingdom gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.StringId);
            var baseKingdomInfos = infos as TInfoType[] ?? infos.ToArray();
            if (baseKingdomInfos.Count() > 1)
            {
                RemoveDuplicates();
            }

            var info = baseKingdomInfos.FirstOrDefault();
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

        public Kingdom GetGameObject(string id)
        {
            return Campaign.Current.Kingdoms.FirstOrDefault(go => go.StringId == id);
        }

        public Kingdom GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
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
            ModifyKingdomList(kingdoms =>
            {
                if (kingdoms.Contains(kingdom))
                {
                    return null;
                }

                kingdoms.Add(kingdom);
                return kingdoms;
            });

            Get(kingdom).IsCustomKingdom = true;
        }

        public void RemoveKingdom(Kingdom kingdom)
        {
            ModifyKingdomList(kingdoms =>
            {
                return kingdoms.RemoveAll(go => go.StringId == kingdom.StringId) > 0 ? kingdoms : null;
            });

            Remove(kingdom.StringId);
        }

        public void DestroyKingdom(Kingdom kingdom)
        {
            DestroyKingdomAction.Apply(kingdom);
            RemoveKingdom(kingdom);
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

            AddKingdom(kingdom);
            return kingdom;
        }
    }
}