using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolutions.Settings;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Revolutions.Models
{
    public class SettlementLoyaltyModel : DefaultSettlementLoyaltyModel
    {

        public SettlementLoyaltyModel()
        {
        }

        public override float CalculateLoyaltyChange(Town town, StatExplainer statExplainer = null)
        {
            if (!town.IsTown)
            {
                return base.CalculateLoyaltyChange(town, statExplainer);
            }

            try
            {
                var explainedNumber = new ExplainedNumber(0.0f, statExplainer, null);
                var settlementInfo = Managers.Settlement.GetInfo(town.Settlement);

                if (settlementInfo.CurrentFaction?.Leader == Hero.MainHero)
                {
                    explainedNumber.Add(RevolutionsSettings.Instance.GeneralBasePlayerLoyalty, new TextObject("{=q2tbqP0z}Bannerlord Settlement"));

                    if (RevolutionsSettings.Instance.RevoltsOverextensionAffectsPlayer && RevolutionsSettings.Instance.RevoltsOverextensionMechanics)
                    {
                        this.Overextension(settlementInfo, ref explainedNumber);
                    }
                }
                else
                {
                    this.BaseLoyalty(settlementInfo, ref explainedNumber);

                    if (RevolutionsSettings.Instance.RevoltsOverextensionMechanics)
                    {
                        this.Overextension(settlementInfo, ref explainedNumber);
                    }
                }

                return explainedNumber.ResultNumber + base.CalculateLoyaltyChange(town, statExplainer);
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed to calculate loyalty change! Using TaleWorld logic now.", ColorHelper.Red));
                InformationManager.DisplayMessage(new InformationMessage($"Town: {town?.Name} | StringId: {town?.StringId}", ColorHelper.Red));

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                    InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    InformationManager.DisplayMessage(new InformationMessage($"InnerException: {exception.InnerException.Message}", ColorHelper.Red));
                    InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.InnerException.StackTrace}", ColorHelper.Red));
                }

                return base.CalculateLoyaltyChange(town, statExplainer);
            }
        }

        private void Overextension(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfo.CurrentFaction?.StringId == settlementInfo.LoyalFaction?.StringId)
            {
                return;
            }

            if (RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic)
            {
                if (settlementInfo?.IsOfImperialCulture == true && settlementInfo?.IsCurrentFactionOfImperialCulture == true)
                {
                    return;
                }
            }

            var factionInfo = settlementInfo.CurrentFactionBaseInfo;
            var overExtension = factionInfo?.InitialTownsCount - factionInfo?.CurrentTownsCount;

            explainedNumber.Add(overExtension.Value * RevolutionsSettings.Instance.RevoltsOverextensionMultiplier, new TextObject("{=YnRmNltF}Overextension"));
        }

        private void BaseLoyalty(SettlementInfo settlementInfo, ref ExplainedNumber explainedNumber)
        {
            if (settlementInfo.CurrentFaction?.IsKingdomFaction == true)
            {
                if (Managers.Kingdom.GetInfo((Kingdom)settlementInfo.CurrentFaction)?.LuckyNation == true)
                {
                    explainedNumber.Add(10, new TextObject("{=glCo42fD}Loyal population)"));
                    return;
                }
            }

            if (RevolutionsSettings.Instance.RevoltsImperialLoyaltyMechanic)
            {
                if (settlementInfo?.IsOfImperialCulture == true)
                {
                    if (settlementInfo?.IsCurrentFactionOfImperialCulture == true)
                    {
                        explainedNumber.Add(10, new TextObject("{=3fQwNP5z}Imperial Loyalty"));
                    }
                    else
                    {
                        explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                    }
                }
                else
                {
                    if (settlementInfo?.IsCurrentFactionOfImperialCulture == true)
                    {
                        explainedNumber.Add(-5, new TextObject("{=qNWmNN8d}Imperial Aversion"));
                    }

                    if (settlementInfo?.LoyalFactionId != settlementInfo?.CurrentFactionId)
                    {
                        explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                    }
                }
            }
            else
            {
                if (settlementInfo?.LoyalFactionId != settlementInfo?.CurrentFactionId)
                {
                    explainedNumber.Add(-5, new TextObject("{=7LzQNP0z}Foreign Rule"));
                }
            }
        }
    }
}