using KNTLibrary.Components.Settlements;
using Revolutions.Components.Factions;
using System;
using System.Collections.Generic;
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
                var factions = new List<(IFaction MapFaction, int Power)>();

                var settlementFactions = this.Settlement.Notables
                    .Where(notable => notable.SupporterOf != null)
                    .Select(s => (s.SupporterOf.MapFaction, s.Power));
                factions.AddRange(settlementFactions);

                var villageFactions = this.Settlement.BoundVillages.SelectMany(village => village.Settlement.Notables)
                        .Where(notable => notable.SupporterOf != null)
                        .Select(s => (s.SupporterOf.MapFaction, s.Power));
                factions.AddRange(villageFactions);

                factions = factions.GroupBy(g => g.MapFaction, (key, g) => (MapFaction: key, Power: g.Select(s => s.Power).Aggregate((current, next) => current + next))).ToList();
                return factions.FirstOrDefault(w => w.Power == factions.Max(m => m.Power)).MapFaction ?? this.Settlement.MapFaction;
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