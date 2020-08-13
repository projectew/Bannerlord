using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Settlements
{
    internal static class SettlementInfoExtension
    {
        internal static void UpdateOwnerRevolt(this SettlementInfo settlementInfo, IFaction faction)
        {
            settlementInfo.PreviousFactionId = settlementInfo.CurrentFactionId;
            settlementInfo.CurrentFactionId = faction.StringId;
            settlementInfo.DaysOwnedByOwner = 0;
        }
    }
}