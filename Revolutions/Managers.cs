using KNTLibrary.Components.Characters;
using KNTLibrary.Components.Clans;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Kingdoms;
using KNTLibrary.Components.Parties;
using KNTLibrary.Components.Settlements;
using Revolutions.Components.Banners;
using Revolutions.Components.Characters;
using Revolutions.Components.CivilWars;
using Revolutions.Components.Clans;
using Revolutions.Components.Factions;
using Revolutions.Components.Kingdoms;
using Revolutions.Components.Parties;
using Revolutions.Components.Revolts;
using Revolutions.Components.Settlements;

namespace Revolutions
{
    internal static class Managers
    {
        internal static BannerManager Banner { get; } = BannerManager.Instance;

        internal static BaseKingdomManager<KingdomInfo> Kingdom { get; } = BaseKingdomManager<KingdomInfo>.Instance;

        internal static BaseFactionManager<FactionInfo> Faction { get; } = BaseFactionManager<FactionInfo>.Instance;

        internal static BaseClanManager<ClanInfo> Clan { get; } = BaseClanManager<ClanInfo>.Instance;

        internal static BasePartyManager<PartyInfo> Party { get; } = BasePartyManager<PartyInfo>.Instance;

        internal static BaseCharacterManager<CharacterInfo> Character { get; } = BaseCharacterManager<CharacterInfo>.Instance;

        internal static BaseSettlementManager<SettlementInfo> Settlement { get; } = BaseSettlementManager<SettlementInfo>.Instance;

        internal static RevoltManager Revolt { get; } = RevoltManager.Instance;

        internal static CivilWarManager CivilWar { get; } = CivilWarManager.Instance;
    }
}