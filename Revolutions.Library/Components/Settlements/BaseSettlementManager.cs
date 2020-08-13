using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Settlements
{
    public class BaseSettlementManager<TInfoType> : IBaseManager<TInfoType, Settlement> where TInfoType : BaseSettlementInfo, new()
    {
        #region Singleton

        static BaseSettlementManager()
        {
            Instance = new BaseSettlementManager<TInfoType>();
        }

        public static BaseSettlementManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Settlements.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Settlements)
            {
                Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Settlements.Count())
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Settlements.All(go => go.StringId != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType Get(Settlement gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.StringId);
            var baseSettlementInfos = infos as TInfoType[] ?? infos.ToArray();
            if (baseSettlementInfos.Count() > 1)
            {
                RemoveDuplicates();
            }

            var info = baseSettlementInfos.FirstOrDefault();
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

        public Settlement GetGameObject(string id)
        {
            return Campaign.Current.Settlements.FirstOrDefault(go => go.StringId == id);
        }

        public Settlement GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
        }

        #endregion
    }
}