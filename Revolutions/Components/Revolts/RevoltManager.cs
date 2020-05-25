using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Revolts
{
    internal class RevoltManager
    {
        #region Singleton

        static RevoltManager()
        {
            Instance = new RevoltManager();
        }

        internal static RevoltManager Instance { get; private set; }

        #endregion

        internal HashSet<Revolt> Revolts = new HashSet<Revolt>();

        internal Revolt GetRevoltByParty(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.PartyId == id);
        }

        internal Revolt GetRevoltByParty(PartyBase party)
        {
            return this.GetRevoltByParty(party.Id);
        }

        internal Revolt GetRevoltBySettlement(string id)
        {
            return this.Revolts.FirstOrDefault(r => r.Id == id);
        }

        internal Revolt GetRevoltBySettlement(Settlement settlement)
        {
            return this.GetRevoltBySettlement(settlement.StringId);
        }

        internal List<Settlement> GetSettlements()
        {
            return this.Revolts.Select(r => r.Settlement).ToList();
        }

        internal List<PartyBase> GetParties()
        {
            return this.Revolts.Select(r => r.Party).ToList();
        }
    }
}