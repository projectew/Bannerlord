using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolutions.Library.Components.Kingdoms
{
    public static class BaseKingdomInfoExtension
    {
        public static bool IsInsideTerritoryOf(this Kingdom kingdom, Kingdom kingdomToCheck)
        {
            var kingdomBasePositions = kingdom.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D);
            var kingdomToCheckPositions = kingdomToCheck.Settlements.Where(go => go.IsTown || go.IsCastle).Select(go => go.Position2D);

            var kingdomPositions = kingdomBasePositions as Vec2[] ?? kingdomBasePositions.ToArray();
            var toCheckPositions = kingdomToCheckPositions as Vec2[] ?? kingdomToCheckPositions.ToArray();

            return kingdomPositions.Max(p => p.X) <= toCheckPositions.Max(p => p.X) &&
                   kingdomPositions.Max(p => p.Y) <= toCheckPositions.Max(p => p.Y) &&
                   kingdomPositions.Min(p => p.X) >= toCheckPositions.Min(p => p.X) &&
                   kingdomPositions.Min(p => p.Y) >= toCheckPositions.Min(p => p.Y);
        }
    }
}