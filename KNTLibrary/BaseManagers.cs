using KNTLibrary.Components;
using KNTLibrary.Components.Banners;
using KNTLibrary.Components.Characters;
using KNTLibrary.Components.Clans;
using KNTLibrary.Components.Factions;
using KNTLibrary.Components.Kingdoms;
using KNTLibrary.Components.Parties;
using KNTLibrary.Components.Plots;
using KNTLibrary.Components.Settlements;

namespace KNTLibrary
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

        public static BasePlotManager<BasePlotInfo<IBaseInfoType, IBaseInfoType>> Plot { get; } = BasePlotManager<BasePlotInfo<IBaseInfoType, IBaseInfoType>>.Instance;
    }
}