using System;
using TaleWorlds.CampaignSystem;
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

        public FactionInfo LoyalFactionInfo => RevolutionsManagers.Faction.GetInfo(this.LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfo InitialFactionInfo => RevolutionsManagers.Faction.GetInfo(this.InitialFaction);

        public FactionInfo CurrentFactionInfo => RevolutionsManagers.Faction.GetInfo(this.CurrentFaction);

        public FactionInfo PreviousFactionInfo => RevolutionsManagers.Faction.GetInfo(this.PreviousFaction);

        #endregion

        #region Normal Properties



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