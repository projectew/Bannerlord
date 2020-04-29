﻿using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace ModLibrary.Factions
{
    [Serializable]
    public class FactionInfo
    {
        public FactionInfo()
        {

        }

        public FactionInfo(IFaction faction)
        {
            this.FactionId = faction.StringId;
            this.InitialTownsCount = this.CurrentTownsCount;
        }

        public string FactionId { get; set; }

        public int InitialTownsCount { get; set; } = 0;

        public int CurrentTownsCount
        {
            get
            {
                return FactionManager<FactionInfo>.Instance.GetFaction(FactionId).Settlements.Where(n => n.IsTown).Count();
            }
        }
    }
}