using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace KNTLibrary.Components.Clans
{
    public static class BaseClanInfoExtension
    {
		public static Kingdom[] NearbyKingdoms(this Clan clan)
		{
			var clanPosition = Vec2.Zero;
			var relevantClanSettlements = clan.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D);

			foreach (var settlementPosition in clan.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D))
			{
				clanPosition += settlementPosition;
			}

			if (relevantClanSettlements.Count() == 0)
			{
				return Kingdom.All.ToArray();
			}

			clanPosition *= (float)1 / relevantClanSettlements.Count();

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