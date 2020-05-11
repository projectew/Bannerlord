using Revolts;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Base.Factions
{
    public static class FactionInfoExtesion
    {
        public static void CityRevoltionFailed(this FactionInfo factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.StringId;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = false;

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevoltProgress = 0;
        }

        public static void CityRevoltionSucceeded(this FactionInfo factionInfo, Settlement settlement)
        {
            factionInfo.CanRevolt = false;
            factionInfo.RevoltedSettlementId = settlement.StringId;
            factionInfo.DaysSinceLastRevolt = 0;
            factionInfo.SuccessfullyRevolted = true;

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.HasRebellionEvent = false;
            settlementInfo.RevoltProgress = 0;
        }

        public static void DailyUpdate(this FactionInfo factionInfo)
        {
            factionInfo.DaysSinceLastRevolt++;

            if (factionInfo.DaysSinceLastRevolt > Settings.Instance.RevoltCooldownTime)
            {
                factionInfo.CanRevolt = true;
            }
        }
    }
}