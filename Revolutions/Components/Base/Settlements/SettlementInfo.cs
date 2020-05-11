﻿using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Settlements;
using Revolutions.Components.Base.Factions;

namespace Revolutions.Components.Base.Settlements
{
    [Serializable]
    public class SettlementInfo : BaseSettlementInfo
    {
        public SettlementInfo() : base()
        {
            this.LoyalFactionId = this.InitialFactionId;
        }

        public SettlementInfo(Settlement settlement) : base(settlement)
        {
            this.LoyalFactionId = this.InitialFactionId;
        }

        #region Reference Properties

        public string LoyalFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction LoyalFaction => RevolutionsManagers.Faction.GetGameObject(this.LoyalFactionId);

        public BaseFactionInfo LoyalFactionInfo => RevolutionsManagers.Faction.GetInfo(this.LoyalFaction);

        public FactionInfo LoyalFactionInfoRevolts => RevolutionsManagers.Faction.GetInfo(this.LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfo InitialFactionInfoRevolts => RevolutionsManagers.Faction.GetInfo(this.InitialFaction);

        public FactionInfo CurrentFactionInfoRevolts => RevolutionsManagers.Faction.GetInfo(this.CurrentFaction);

        public FactionInfo PreviousFactionInfoRevolts => RevolutionsManagers.Faction.GetInfo(this.PreviousFaction);

        #endregion

        public bool IsLoyalFactionOfImperialCulture => RevolutionsManagers.Faction.GetGameObject(this.LoyalFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevoltProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}