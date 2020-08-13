using System;
using Revolutions.Library.Helpers;
using Revolutions.Module.Components.Settlements;
using Revolutions.Module.Settings;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using GameTexts = Revolutions.Module.Components.Revolts.Localization.GameTexts;

namespace Revolutions.Module.Components.Revolts.ViewModels
{
    public class TownRevoltViewModel : ViewModel
    {
        private readonly SettlementInfo _settlementInfo;

        public TownRevoltViewModel(SettlementInfo settlementInfo)
        {
            _settlementInfo = settlementInfo;
        }

        [DataSourceProperty]
        public string TownDescription
        {
            get
            {
                try
                {
                    if (_settlementInfo.RevoltProgress < 10)
                    {
                        var textObject = new TextObject(GameTexts.RevoltsLoyaltyMenuDescriptionPositive);
                        textObject.SetTextVariable("SETTLEMENT", _settlementInfo.Settlement.Name);
                        return textObject.ToString();
                    }
                    else
                    {
                        var textObject = new TextObject(GameTexts.RevoltsLoyaltyMenuDescriptionNegative);
                        textObject.SetTextVariable("SETTLEMENT", _settlementInfo.Settlement.Name);
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
                    var textObject = new TextObject(GameTexts.RevoltsLoyaltyMenuPopulationLoyaltyFaction);
                    textObject.SetTextVariable("FACTION", _settlementInfo.LoyalFaction.Name);
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
        public ImageIdentifierVM PopulationLoyaltyVisual => new ImageIdentifierVM(BannerCode.CreateFrom(_settlementInfo.LoyalFaction.Banner), true);

        [DataSourceProperty]
        public string RevoltProgress
        {
            get
            {
                try
                {
                    var textObject = new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltProgress);
                    textObject.SetTextVariable("PROGRESS", new TextObject($"{(int)_settlementInfo.RevoltProgress}"));
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
                    var currentSettlementId = _settlementInfo.Id;
                    var currentFactionInfo = _settlementInfo.CurrentFactionInfo;
                    var previousFactionInfo = _settlementInfo.PreviousFactionInfo;

                    if (currentFactionInfo.Id == _settlementInfo.LoyalFaction.StringId)
                    {
                        return $"{new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodPositive)}";
                    }

                    var revoltedInPreviousFaction = previousFactionInfo?.RevoltedSettlementId == currentSettlementId;

                    if (currentFactionInfo.CanRevolt)
                    {
                        var inspiration = new TextObject();

                        if (currentFactionInfo.SuccessfullyRevolted || revoltedInPreviousFaction && previousFactionInfo?.SuccessfullyRevolted == true)
                        {
                            if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                            {
                                inspiration = new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeKingdom);
                            }
                            else
                            {
                                inspiration = new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeSettlement);
                                inspiration.SetTextVariable("SETTLEMENT", currentFactionInfo.RevoltedSettlement.Name);
                            }
                        }

                        return $"{new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodNegativeBase)} {inspiration}";
                    }

                    if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                    {
                        return $"{new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodNeutralCurrent)}";
                    }

                    if (currentFactionInfo.RevoltedSettlement == null)
                    {
                        return string.Empty;
                    }

                    var textObject = new TextObject(GameTexts.RevoltsLoyaltyMenuRevoltMoodNeutralSettlement);
                    textObject.SetTextVariable("SETTLEMENT", currentFactionInfo.RevoltedSettlement.Name);
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
        public string CloseMenu => new TextObject("{=3fQwWiDZ}Close").ToString();

        private void ExitMenu()
        {
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged("FactionVisual");
            OnPropertyChanged("TownDescription");
            OnPropertyChanged("TownOwnership");
            OnPropertyChanged("RevoltProgress");
            OnPropertyChanged("RevoltMood");
        }
    }
}