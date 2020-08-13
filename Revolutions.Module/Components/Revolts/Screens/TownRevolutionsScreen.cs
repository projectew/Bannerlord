using System;
using Revolutions.Library.Helpers;
using Revolutions.Module.Components.Revolts.ViewModels;
using Revolutions.Module.Components.Settlements;
using Revolutions.Module.Settings;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace Revolutions.Module.Components.Revolts.Screens
{
    public class TownRevoltsScreen : ScreenBase
    {
        private readonly SettlementInfo _settlementInfo;
        private TownRevoltViewModel _dataSource;
        private GauntletLayer _gauntletLayer;

        public TownRevoltsScreen(SettlementInfo settlementInfo)
        {
            _settlementInfo = settlementInfo;
        }

        protected override void OnInitialize()
        {
            try
            {
                base.OnInitialize();

                _dataSource = new TownRevoltViewModel(_settlementInfo);
                _gauntletLayer = new GauntletLayer(100)
                {
                    IsFocusLayer = true
                };

                AddLayer(_gauntletLayer);
                _gauntletLayer.InputRestrictions.SetInputRestrictions();
                ScreenManager.TrySetFocus(_gauntletLayer);
                _gauntletLayer.LoadMovie("TownRevoltScreen", _dataSource);
            }
            catch (Exception exception)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Error while initializing screen. Turn on debug mode for details.", ColorHelper.Red));

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"{exception.Message}{Environment.NewLine}{exception.StackTrace}", ColorHelper.Red));
                }
            }
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();

            _gauntletLayer = null;
            _dataSource = null;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            RemoveLayer(_gauntletLayer);
            _gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_gauntletLayer);
        }
    }
}