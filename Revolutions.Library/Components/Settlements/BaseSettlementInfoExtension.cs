using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Settlements
{
    public static class BaseSettlementInfoExtension
    {
        public static bool IsInitialCultureOf(this BaseSettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.InitialCultureId == cultureStringId;

        }
        public static bool IsInitialCultureOf(this BaseSettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsInitialCultureOf(culture.StringId);
        }

        public static bool IsOfCulture(this BaseSettlementInfo settlementInfo, string cultureStringId)
        {
            return settlementInfo.Settlement.Culture.StringId == cultureStringId;
        }

        public static bool IsOfCulture(this BaseSettlementInfo settlementInfo, CultureObject culture)
        {
            return settlementInfo.IsOfCulture(culture.StringId);
        }
    }
}