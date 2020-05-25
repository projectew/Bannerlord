using Revolutions.Components.Revolts.Localization;
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
                explainedNumber.Add(RevolutionsSettings.Instance.RevoltsGeneralPlayerBaseLoyalty, new TextObject(GameTexts.RevoltsLoyaltyBannerlordSettlement));
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

            var textObject = new TextObject(GameTexts.RevoltsLoyaltyNotables);
            textObject.SetTextVariable("SETTLEMENT", settlement.Name);
            explainedNumber.Add(notablesLoyaltyChange, textObject);

            foreach (var village in settlement.BoundVillages.Select(village => village.Settlement))
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

                textObject = new TextObject(GameTexts.RevoltsLoyaltyNotables);
                textObject.SetTextVariable("SETTLEMENT", settlement.Name);
                explainedNumber.Add(notablesLoyaltyChange, textObject);
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
            explainedNumber.Add(overextension * RevolutionsSettings.Instance.RevoltsOverextensionMultiplier, new TextObject(GameTexts.RevoltsLoyaltyOverextension));
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
                    explainedNumber.Add(3, new TextObject(GameTexts.RevoltsLoyaltyImperialLoyalty));
                }
                else
                {
                    explainedNumber.Add(-1, new TextObject(GameTexts.RevoltsLoyaltyForeignRule));
                }
            }
            else
            {
                if (settlementInfo.IsCurrentFactionOfImperialCulture)
                {
                    explainedNumber.Add(-1, new TextObject(GameTexts.RevoltsLoyaltyImperialAversion));
                }

                if (settlementInfo.LoyalFaction.StringId != settlementInfo.CurrentFactionId)
                {
                    explainedNumber.Add(1, new TextObject(GameTexts.RevoltsLoyaltyForeignRule));
                }
            }
        }

        private void LuckyNationChange(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfo.CurrentFaction.IsKingdomFaction)
            {
                if (Managers.Kingdom.Get(settlementInfo.Settlement.OwnerClan.Kingdom)?.LuckyNation == true)
                {
                    explainedNumber.Add(5, new TextObject(GameTexts.RevoltsLoyaltyLuckyNation));
                    return;
                }
            }
        }
    }
}