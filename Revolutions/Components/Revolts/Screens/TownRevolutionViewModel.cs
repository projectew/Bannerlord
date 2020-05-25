using KNTLibrary.Helpers;
using Revolutions.Components.Settlements;
using Revolutions.Settings;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Components.Revolts.Screens
{
    public class TownRevoltViewModel : ViewModel
    {
        private readonly SettlementInfo SettlementInfo;

        public TownRevoltViewModel(SettlementInfo settlementInfo)
        {
            this.SettlementInfo = settlementInfo;
        }

        [DataSourceProperty]
        public string TownDescription
        {
            get
            {
                try
                {
                    if (this.SettlementInfo.RevoltProgress < 10)
                    {
                        var textObject = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuDescriptionPositive);
                        textObject.SetTextVariable("SETTLEMENT", this.SettlementInfo.Settlement.Name);
                        return textObject.ToString();
                    }
                    else
                    {
                        var textObject = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuDescriptionNegative);
                        textObject.SetTextVariable("SETTLEMENT", this.SettlementInfo.Settlement.Name);
                        return textObject.ToString();
                    }
                }
                catch (Exception exception)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}", ColorHelper.Red));
                        return $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
                    }

                    return "An error occurred. Pleas enable Debug Mode for more information!";
                }
            }
        }

        [DataSourceProperty]
        public string PopulationLoyalty
        {
            get
            {
                try
                {
                    var textObject = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuPopulationLoyaltyFaction);
                    textObject.SetTextVariable("FACTION", this.SettlementInfo.LoyalFaction.Name);
                    return textObject.ToString();
                }
                catch (Exception exception)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}", ColorHelper.Red));
                        return $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
                    }

                    return "An error occurred. Pleas enable Debug Mode for more information!";
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM PopulationLoyaltyVisual => new ImageIdentifierVM(BannerCode.CreateFrom(this.SettlementInfo.LoyalFaction.Banner), true);

        [DataSourceProperty]
        public string RevoltProgress
        {
            get
            {
                try
                {
                    var textObject = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltProgress);
                    textObject.SetTextVariable("PROGRESS", new TextObject($"{(int)this.SettlementInfo.RevoltProgress}"));
                    return textObject.ToString();
                }
                catch (Exception exception)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}", ColorHelper.Red));
                        return $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
                    }

                    return "An error occurred. Pleas enable Debug Mode for more information!";
                }
            }
        }

        [DataSourceProperty]
        public string RevoltMood
        {
            get
            {
                try
                {
                    var currentSettlementId = this.SettlementInfo.Id;
                    var currentFactionInfo = this.SettlementInfo.CurrentFactionInfo;
                    var previousFactionInfo = this.SettlementInfo.PreviousFactionInfo;

                    if (currentFactionInfo.Id == this.SettlementInfo.LoyalFaction.StringId)
                    {
                        return $"{new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodPositive)}";
                    }

                    var revoltedInPreviousFaction = previousFactionInfo?.RevoltedSettlementId == currentSettlementId;

                    if (currentFactionInfo.CanRevolt)
                    {
                        var inspiration = new TextObject("");

                        if (currentFactionInfo.SuccessfullyRevolted || revoltedInPreviousFaction && previousFactionInfo?.SuccessfullyRevolted == true)
                        {
                            if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                            {
                                inspiration = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeKingdom);
                            }
                            else
                            {
                                inspiration = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeSettlement);
                                inspiration.SetTextVariable("SETTLEMENT", currentFactionInfo.RevoltedSettlement.Name);
                            }
                        }

                        return $"{new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeBase)} {inspiration}";
                    }
                    else
                    {
                        if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                        {
                            return $"{new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodNeutralCurrent)}";
                        }

                        if (currentFactionInfo.RevoltedSettlement == null)
                        {
                            return string.Empty;
                        }

                        var textObject = new TextObject(Localization.GameTexts.RevoltsLoyaltyMenuRevoltMoodNeutralSettlement);
                        textObject.SetTextVariable("SETTLEMENT", currentFactionInfo.RevoltedSettlement.Name);
                        return textObject.ToString();
                    }
                }
                catch (Exception exception)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}", ColorHelper.Red));
                        return $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
                    }

                    return "An error occurred. Pleas enable Debug Mode for more information!";
                }
            }
        }

        [DataSourceProperty]
        public string CloseMenu => new TextObject("{=3fQwWiDZ}Close").ToString();

        private void ExitMenu()
        {
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged("FactionVisual");
            this.OnPropertyChanged("TownDescription");
            this.OnPropertyChanged("TownOwnership");
            this.OnPropertyChanged("RevoltProgress");
            this.OnPropertyChanged("RevoltMood");
        }
    }
}