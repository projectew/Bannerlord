using TaleWorlds.CampaignSystem;
using Revolutions.Settings;

namespace Revolutions.Components.Base.Factions
{
    internal static class FactionInfoExtesion
    {
        internal static void CityRevoltionFailed(this FactionInfo factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.StringId;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = false;

            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevoltProgress = 0;
        }

        internal static void CityRevoltionSucceeded(this FactionInfo factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.StringId;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = true;

            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevoltProgress = 0;
        }

        internal static void DailyUpdate(this FactionInfo factionInfo)
        {
            factionInfo.DaysSinceLastRevolt++;

            if (factionInfo.DaysSinceLastRevolt > RevolutionsSettings.Instance.RevoltsGeneralCooldownTime)
            {
                factionInfo.CanRevolt = true;
            }
        }
    }
}