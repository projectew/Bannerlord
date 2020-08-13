using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolutions.Library.Components.Clans
{
    public static class BaseClanInfoExtension
    {
        public static Kingdom[] NearbyKingdoms(this Clan clan)
        {
            var clanPosition = Vec2.Zero;
            var relevantClanSettlements = clan.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D);

            clanPosition = clan.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D).Aggregate(clanPosition, (current, settlementPosition) => current + settlementPosition);

            var clanSettlements = relevantClanSettlements as Vec2[] ?? relevantClanSettlements.ToArray();
            if (!clanSettlements.Any())
            {
                return Kingdom.All.ToArray();
            }

            clanPosition *= (float)1 / clanSettlements.Count();

            var kingdomDistance = Kingdom.All
                .Select(go => (Kingdom: go, FactionMidPoint: go.FactionMidPoint.Distance(clanPosition)))
                .ToArray();

            var averageKingdomDistance = kingdomDistance.Average(go => go.FactionMidPoint);

            return kingdomDistance
                .Where(x => x.FactionMidPoint <= averageKingdomDistance)
                .OrderBy(x => x.FactionMidPoint)
                .Select(x => x.Kingdom)
                .ToArray();
        }
    }
}