using Revolts;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Base.Settlements
{
    public static class SettlementInfoExtension
    {
        public static void UpdateOwnerRevolt(this SettlementInfo settlementInfo, IFaction faction)
        {
            settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
            settlementInfo.CurrentFactionId = faction.StringId;
            settlementInfo.DaysOwnedByOwner = 0;
        }

        public static void DailyUpdate(this SettlementInfo settlementInfo)
        {
            settlementInfo.DaysOwnedByOwner++;

            if (settlementInfo.LoyalFactionId != settlementInfo.CurrentFactionId && settlementInfo.DaysOwnedByOwner > Settings.Instance.DaysUntilLoyaltyChange)
            {
                settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;
            }
        }
    }
}