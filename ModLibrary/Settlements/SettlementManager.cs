﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Settlements
{
    public class SettlementManager<T> where T : SettlementInfo, new()
    {
        #region Singleton

        private SettlementManager() { }

        static SettlementManager()
        {
            SettlementManager<T>.Instance = new SettlementManager<T>();
        }

        public static SettlementManager<T> Instance { get; private set; }
        public object PartyInfos { get; private set; }

        #endregion

        public List<T> SettlementInfos = new List<T>();

        public void InitializeSettlementInfos()
        {
            foreach (var settlement in Settlement.All)
            {
                this.GetSettlementInfo(settlement);
            }
        }

        public T GetSettlementInfo(string settlementId)
        {
            var settlementInfo = this.SettlementInfos.FirstOrDefault(info => info.SettlementId == settlementId);

            if (settlementInfo != null)
            {
                return settlementInfo;
            }

            Settlement missingSettlement = Settlement.All.FirstOrDefault(settlement => settlement.StringId == settlementId);
            return missingSettlement != null ? this.AddSettlement(missingSettlement) : null;
        }

        public T GetSettlementInfo(Settlement settlement)
        {
            return this.GetSettlementInfo(settlement.StringId);
        }

        public T AddSettlement(Settlement settlement)
        {
            var settlementInfo = (T)Activator.CreateInstance(typeof(T), settlement);
            this.SettlementInfos.Add(settlementInfo);

            return settlementInfo;
        }

        private void RemoveSettlementInfo(string settlementId)
        {
            this.SettlementInfos.RemoveAll(info => info.SettlementId == settlementId);
        }

        public Settlement GetSettlement(string settlementId)
        {
            return Settlement.All.FirstOrDefault(settlement => settlement.StringId == settlementId);
        }

        public Settlement GetSettlement(T settlementInfo)
        {
            return this.GetSettlement(settlementInfo.SettlementId);
        }

        public void WatchSettlements()
        {
            if (this.SettlementInfos.Count() == Campaign.Current.Settlements.Count())
            {
                return;
            }

            this.SettlementInfos.RemoveAll(info => !Settlement.All.Any(settlement => settlement.StringId == info.SettlementId));

            foreach (var settlement in Campaign.Current.Settlements)
            {
                this.GetSettlementInfo(settlement);
            }
        }
    }
}