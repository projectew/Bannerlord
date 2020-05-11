using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Settlements;
using Revolts;
using Revolutions.Components.BaseComponents.Factions;

namespace Revolutions.Components.BaseComponents.Settlements
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

        public IFaction LoyalFaction => RevoltsManagers.Faction.GetGameObject(this.LoyalFactionId);

        public BaseFactionInfo LoyalFactionInfo => RevoltsManagers.Faction.GetInfo(this.LoyalFaction);

        public FactionInfo LoyalFactionInfoRevolts => RevoltsManagers.Faction.GetInfo(this.LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfo InitialFactionInfoRevolts => RevoltsManagers.Faction.GetInfo(this.InitialFaction);

        public FactionInfo CurrentFactionInfoRevolts => RevoltsManagers.Faction.GetInfo(this.CurrentFaction);

        public FactionInfo PreviousFactionInfoRevolts => RevoltsManagers.Faction.GetInfo(this.PreviousFaction);

        #endregion

        public bool IsLoyalFactionOfImperialCulture => RevoltsManagers.Faction.GetGameObject(this.LoyalFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevoltProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}