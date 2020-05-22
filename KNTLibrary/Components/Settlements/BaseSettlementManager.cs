using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Settlements
{
    public class BaseSettlementManager<InfoType> : IBaseManager<InfoType, Settlement> where InfoType : BaseSettlementInfo, new()
    {
        #region Singleton

        static BaseSettlementManager()
        {
            Instance = new BaseSettlementManager<InfoType>();
        }

        public static BaseSettlementManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Settlements.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Settlements)
            {
                this.Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Settlements.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Settlements.Any(go => go.StringId == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType Get(Settlement gameObject)
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

        public Settlement GetGameObject(string id)
        {
            return Campaign.Current.Settlements.FirstOrDefault(go => go.StringId == id);
        }

        public Settlement GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
        }

        #endregion
    }
}