using KNTLibrary.Components.Characters;
using KNTLibrary.Components.Clans;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Kingdoms;
using KNTLibrary.Components.Parties;
using KNTLibrary.Components.Settlements;
using Revolutions.Components.Base.Banner;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.Base.Clans;
using Revolutions.Components.Base.Factions;
using Revolutions.Components.Base.Kingdoms;
using Revolutions.Components.Base.Parties;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.CivilWars;
using Revolutions.Components.Revolts;

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