using TaleWorlds.CampaignSystem;
using Revolutions.Settings;

namespace Revolutions.Components.Base.Settlements
{
    internal static class SettlementInfoExtension
    {
        internal static void UpdateOwnerRevolt(this SettlementInfo settlementInfo, IFaction faction)
        {
            settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
            settlementInfo.CurrentFactionId = faction.StringId;
            settlementInfo.DaysOwnedByOwner = 0;
        }

        internal static void DailyUpdate(this SettlementInfo settlementInfo)
        {
            settlementInfo.DaysOwnedByOwner++;

            if (settlementInfo.LoyalFactionId != settlementInfo.CurrentFactionId && settlementInfo.DaysOwnedByOwner > RevolutionsSettings.Instance.GeneralDaysUntilLoyaltyChange)
            {
                settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;
            }
        }
    }
}