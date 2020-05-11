using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace KNTLibrary.Components.Clans
{
    public static class ClanInfoExtension
    {
		public static Kingdom[] NearbyKingdoms(this Clan clan)
		{
			var clanPosition = Vec2.Zero;


			var relevantClanSettlements = clan.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D);
			var amountOfRelevantClanSettlements = 0;

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
				.Select(k => (k, k.FactionMidPoint.Distance(clanPosition)))
				.ToArray();
			var average = kingdomDistance.Average(x => x.Item2);
			return kingdomDistance
				.Where(x => x.Item2 <= average)
				.OrderBy(x => x.Item2)
				.Select(x => x.k)
				.ToArray();
		}
	}
}