using KNTLibrary.Helpers;
using Revolutions.Components.Base.Settlements;
using Revolutions.Settings;
using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Components.General.Screens.TownRevolution
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
                        var textObject = new TextObject("{=3fBkqk4u}The people of {SETTLEMENT} seem to be content.");
                        textObject.SetTextVariable("SETTLEMENT", this.SettlementInfo.Settlement.Name);
                        return textObject.ToString();
                    }
                    else
                    {
                        var textObject = new TextObject("{=dRoS0zTD}Flames of Revolt are slowly stirring in {SETTLEMENT}.");
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
                    var textObject = new TextObject("{=MYu8szGz}Population is loyal to {FACTION}.");
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
                    var textObject = new TextObject("{=q2tbSs8d}Current revolt progress is {PROGRESS}%");
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
                        return $"{new TextObject("{=zQNPQz3q}People are content with the current rule.")}";
                    }

                    var revoltedInPreviousFaction = previousFactionInfo?.RevoltedSettlementId == currentSettlementId;

                    if (currentFactionInfo.CanRevolt)
                    {
                        var inspiration = new TextObject("");

                        if (currentFactionInfo.SuccessfullyRevolted || revoltedInPreviousFaction && previousFactionInfo?.SuccessfullyRevolted == true)
                        {
                            if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                            {
                                inspiration = new TextObject("{=qZS0ma0z}Many are inspired by tales of revolts in the kingdom.");
                            }
                            else
                            {
                                inspiration = new TextObject("{=7LzQWiDZ}Many are inspired by events at {SETTLEMENT}.");
                                inspiration.SetTextVariable("SETTLEMENT", currentFactionInfo.RevoltedSettlement.Name);
                            }
                        }

                        return $"{new TextObject("{=HWiDqN8d}Some talk of raising banners of their homeland.")} {inspiration}";
                    }
                    else
                    {
                        if (currentFactionInfo.RevoltedSettlementId == currentSettlementId || revoltedInPreviousFaction)
                        {
                            return $"{new TextObject("{=q2tbH41e}The people of this town had revolted recently and don't wish to spill blood again.")}";
                        }

                        if (currentFactionInfo.RevoltedSettlement == null)
                        {
                            return string.Empty;
                        }

                        var textObject = new TextObject("{=6m7Ss8fW}After hearing of blood spilled in {SETTLEMENT} citizens are afraid of revolting.");
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