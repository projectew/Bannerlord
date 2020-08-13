using Revolutions.Library.Components.Banners;
using Revolutions.Library.Components.Characters;
using Revolutions.Library.Components.Clans;
using Revolutions.Library.Components.Factions;
using Revolutions.Library.Components.Kingdoms;
using Revolutions.Library.Components.Parties;
using Revolutions.Library.Components.Settlements;

namespace Revolutions.Library
{
    public static class BaseManagers
    {
        public static BaseBannerManager Banner { get; } = BaseBannerManager.Instance;

        public static BaseFactionManager<BaseFactionInfo> Faction { get; } = BaseFactionManager<BaseFactionInfo>.Instance;

        public static BaseKingdomManager<BaseKingdomInfo> Kingdom { get; } = BaseKingdomManager<BaseKingdomInfo>.Instance;

        public static BaseClanManager<BaseClanInfo> Clan { get; } = BaseClanManager<BaseClanInfo>.Instance;

        public static BasePartyManager<BasePartyInfo> Party { get; } = BasePartyManager<BasePartyInfo>.Instance;

        public static BaseCharacterManager<BaseCharacterInfo> Character { get; } = BaseCharacterManager<BaseCharacterInfo>.Instance;

        public static BaseSettlementManager<BaseSettlementInfo> Settlement { get; } = BaseSettlementManager<BaseSettlementInfo>.Instance;
    }
}