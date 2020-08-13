using Revolutions.Library.Components.Characters;
using Revolutions.Library.Components.Clans;
using Revolutions.Library.Components.Factions;
using Revolutions.Library.Components.Kingdoms;
using Revolutions.Library.Components.Parties;
using Revolutions.Library.Components.Settlements;
using Revolutions.Module.Components.Banners;
using Revolutions.Module.Components.Characters;
using Revolutions.Module.Components.CivilWars;
using Revolutions.Module.Components.Clans;
using Revolutions.Module.Components.Factions;
using Revolutions.Module.Components.Kingdoms;
using Revolutions.Module.Components.Parties;
using Revolutions.Module.Components.Revolts;
using Revolutions.Module.Components.Settlements;

namespace Revolutions.Module
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