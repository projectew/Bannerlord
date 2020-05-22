using KNTLibrary.Components.Settlements;
using Revolutions.Components.Factions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Settlements
{
    [Serializable]
    public class SettlementInfo : BaseSettlementInfo
    {
        public SettlementInfo() : base()
        {

        }

        public SettlementInfo(Settlement settlement) : base(settlement)
        {

        }

        #region Reference Properties

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction LoyalFaction
        {
            get
            {
                return this.Settlement.Notables
                    .Where(notable => notable.SupporterOf != null)
                    .GroupBy(notable => notable.SupporterOf.MapFaction, (key, notable) => new { MapFaction = key, Power = notable.Select(s => s.Power) })
                    .OrderByDescending(o => o.Power)
                    .FirstOrDefault().MapFaction;
            }
        }

        public FactionInfo LoyalFactionInfo => Managers.Faction.Get(this.LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfo InitialFactionInfo => Managers.Faction.Get(this.InitialFaction);

        public FactionInfo CurrentFactionInfo => Managers.Faction.Get(this.CurrentFaction);

        public FactionInfo PreviousFactionInfo => Managers.Faction.Get(this.PreviousFaction);

        #endregion

        #region Normal Properties



        #endregion

        public bool IsLoyalFactionOfImperialCulture => this.LoyalFaction.Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevoltProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}