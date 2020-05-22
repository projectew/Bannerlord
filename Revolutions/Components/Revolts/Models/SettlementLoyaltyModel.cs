using Revolutions.Components.Settlements;
using Revolutions.Settings;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Localization;

namespace Revolutions.Components.Revolts.Models
{
    public class SettlementLoyaltyModel : DefaultSettlementLoyaltyModel
    {
        public SettlementLoyaltyModel()
        {
        }

        public override float CalculateLoyaltyChange(Town town, StatExplainer statExplainer = null)
        {
            if (!town.Settlement.IsFortification)
            {
                return base.CalculateLoyaltyChange(town, statExplainer);
            }

            var explainedNumber = new ExplainedNumber(0, statExplainer, null);
            var settlementInfo = Managers.Settlement.Get(town.Settlement);

            if (settlementInfo.CurrentFaction.Leader == Hero.MainHero)
            {
                explainedNumber.Add(RevolutionsSettings.Instance.GeneralPlayerBaseLoyalty, new TextObject("Player Settlement"));
            }

            this.NotablesChange(settlementInfo, ref explainedNumber);
            this.ImperialChange(settlementInfo, ref explainedNumber);
            this.OverextensionChange(settlementInfo, ref explainedNumber);
            this.LuckyNationChange(settlementInfo, ref explainedNumber);

            return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
        }

        private void NotablesChange(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            var notablesLoyaltyChange = 0f;

            var settlement = settlementInfo.Settlement;
            foreach (var notable in settlement.Notables.Where(notable => notable.SupporterOf != null))
            {
                if (settlement.OwnerClan.MapFaction.StringId == notable.SupporterOf.MapFaction.StringId)
                {
                    notablesLoyaltyChange += 1;
                }
                else
                {
                    notablesLoyaltyChange -= 1;
                }
            }

            explainedNumber.Add(notablesLoyaltyChange, new TextObject($"Notables: {settlement.Name}"));

            foreach (var village in settlement.BoundVillages.Select(v => v.Settlement))
            {
                notablesLoyaltyChange = 0;

                foreach (var noteable in village.Notables.Where(notable => notable.SupporterOf != null))
                {
                    if (village.OwnerClan.MapFaction.StringId == noteable.SupporterOf.MapFaction.StringId)
                    {
                        notablesLoyaltyChange += 1;
                    }
                    else
                    {
                        notablesLoyaltyChange -= 1;
                    }
                }

                explainedNumber.Add(notablesLoyaltyChange, new TextObject($"Notables: {village.Name}"));
            }
        }

        private void OverextensionChange(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (!RevolutionsSettings.Instance.RevoltsOverextensionMechanics
                || !RevolutionsSettings.Instance.RevoltsOverextensionAffectsPlayer && settlementInfo.CurrentFaction.Leader == Hero.MainHero
                || settlementInfo.CurrentFaction.StringId == settlementInfo.LoyalFaction.StringId)
            {
                return;
            }

            var overextension = settlementInfo.CurrentFaction.Settlements.Where(s => settlementInfo.CurrentFactionId != Managers.Settlement.Get(s).LoyalFaction.StringId).Count();
            explainedNumber.Add(overextension * RevolutionsSettings.Instance.RevoltsOverextensionMultiplier, new TextObject("Overextension"));
        }

        private void ImperialChange(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (!RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic)
            {
                return;
            }

            if (settlementInfo.IsOfImperialCulture)
            {
                if (settlementInfo.IsCurrentFactionOfImperialCulture)
                {
                    explainedNumber.Add(1, new TextObject("Imperial Loyalty"));
                }
                else
                {
                    explainedNumber.Add(-1, new TextObject("Foreign Rule"));
                }
            }
            else
            {
                if (settlementInfo.IsCurrentFactionOfImperialCulture)
                {
                    explainedNumber.Add(-1, new TextObject("Imperial Aversion"));
                }

                if (settlementInfo.LoyalFaction.StringId != settlementInfo.CurrentFactionId)
                {
                    explainedNumber.Add(1, new TextObject("Foreign Rule"));
                }
            }
        }

        private void LuckyNationChange(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfo.CurrentFaction.IsKingdomFaction)
            {
                if (Managers.Kingdom.Get(settlementInfo.Settlement.OwnerClan.Kingdom)?.LuckyNation == true)
                {
                    explainedNumber.Add(3, new TextObject("Lucky Nation)"));
                    return;
                }
            }
        }
    }
}