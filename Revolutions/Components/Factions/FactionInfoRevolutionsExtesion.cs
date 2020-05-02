﻿using ModLibrary.Characters;
using Revolutions.Components.Factions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Revolutions.Factions
{
    public static class FactionInfoRevolutionsExtesion
    {
        public static void CityRevoltionFailed(this FactionInfoRevolutions factionInfoRevolutions, Settlement settlement)
        {
            factionInfoRevolutions.CanRevolt = false;
            factionInfoRevolutions.RevoltedSettlementId = settlement.StringId;
            factionInfoRevolutions.DaysSinceLastRevolt = 0;
            factionInfoRevolutions.SuccessfullyRevolted = false;
            RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement).RevolutionProgress = 0;
        }

        public static void CityRevoltionSucceeded(this FactionInfoRevolutions factionInfoRevolutions, Settlement settlement)
        {
            factionInfoRevolutions.CanRevolt = false;
            factionInfoRevolutions.RevoltedSettlementId = settlement.StringId;
            factionInfoRevolutions.DaysSinceLastRevolt = 0;
            factionInfoRevolutions.SuccessfullyRevolted = true;
            RevolutionsManagers.SettlementManager.GetSettlementInfo(settlement).RevolutionProgress = 0;
        }

        public static void DailyUpdate(this FactionInfoRevolutions factionInfo)
        {
            factionInfo.DaysSinceLastRevolt++;

            if (factionInfo.DaysSinceLastRevolt > SubModule.Configuration.RevoltCooldownTime)
            {
                factionInfo.CanRevolt = true;
            }
        }
    }
}