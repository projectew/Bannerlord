using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Revolts
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
            return Revolts.FirstOrDefault(r => r.PartyId == id);
        }

        internal Revolt GetRevoltByParty(PartyBase party)
        {
            return GetRevoltByParty(party.Id);
        }

        internal Revolt GetRevoltBySettlement(string id)
        {
            return Revolts.FirstOrDefault(r => r.Id == id);
        }

        internal Revolt GetRevoltBySettlement(Settlement settlement)
        {
            return GetRevoltBySettlement(settlement.StringId);
        }

        internal List<Settlement> GetSettlements()
        {
            return Revolts.Select(r => r.Settlement).ToList();
        }

        internal List<PartyBase> GetParties()
        {
            return Revolts.Select(r => r.Party).ToList();
        }
    }
}