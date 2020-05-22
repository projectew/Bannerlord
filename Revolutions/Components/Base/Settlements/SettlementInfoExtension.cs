using Revolutions.Components.Base.Kingdoms;
using Revolutions.Settings;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Base.Settlements
{
    internal static class SettlementInfoExtension
    {
        internal static void UpdateOwnerRevolt(this SettlementInfo settlementInfo, IFaction faction)
        {
            if (faction.IsKingdomFaction)
            {
                var kingdomInfo = Revolutions.Managers.Kingdom.GetInfo(faction.Leader.Clan.Kingdom);
                if (kingdomInfo != null && kingdomInfo.IsRevoltKingdom)
                {
                    settlementInfo.LoyalFactionId = faction.StringId;
                }
            }

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