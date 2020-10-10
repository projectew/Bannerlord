using System;
using System.Collections.Generic;
using System.Linq;
using Revolutions.Library.Components.Settlements;
using Revolutions.Module.Components.Factions;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Settlements
{
    [Serializable]
    public class SettlementInfo : BaseSettlementInfo
    {
        public SettlementInfo()
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
                var factions = new List<(IFaction MapFaction, float Power)>();

                //  gets list of (supported faction, notable power) from all notables in this settlement
                var settlementFactions = Settlement.Notables
                    .Where(notable => notable.SupporterOf != null)
                    .Select(s => (s.SupporterOf.MapFaction, s.Power));
                factions.AddRange(settlementFactions);

				//  gets list of (supported faction, notable power) from all notables in this settlement's bound villages

                var villageFactions = Settlement.BoundVillages.SelectMany(village => village.Settlement.Notables)
                        .Where(notable => notable.SupporterOf != null)
                        .Select(s => (s.SupporterOf.MapFaction, s.Power));
                factions.AddRange(villageFactions);

                //  groups list by faction and aggregates the total notable power supporting each faction

                factions = factions.GroupBy(g => g.MapFaction, (key, g) => (MapFaction: key, Power: g.Select(s => s.Power).Aggregate((current, next) => current + next))).ToList();

                // returns (faction, total power) tuple with highest power, or this settlement's own faction if no such faction matching the faction with the most power exists
                // faction must exist?, since the highest power faction was found by LINQing the same list
                
                return factions.FirstOrDefault(w => w.Power == factions.Max(m => m.Power)).MapFaction ?? Settlement.MapFaction;
            }
        }

        public FactionInfo LoyalFactionInfo => Managers.Faction.Get(LoyalFaction);

        #endregion

        #region Reference Properties Inherited

        public FactionInfo InitialFactionInfo => Managers.Faction.Get(InitialFaction);

        public FactionInfo CurrentFactionInfo => Managers.Faction.Get(CurrentFaction);

        public FactionInfo PreviousFactionInfo => Managers.Faction.Get(PreviousFaction);

        #endregion

        #region Normal Properties



        #endregion

        public bool IsLoyalFactionOfImperialCulture => LoyalFaction.Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties

        public bool IsOwnerInSettlement { get; set; } = false;

        public float RevoltProgress { get; set; } = 0;

        public bool HasRebellionEvent { get; set; } = false;

        public int DaysOwnedByOwner { get; set; } = 0;

        #endregion
    }
}