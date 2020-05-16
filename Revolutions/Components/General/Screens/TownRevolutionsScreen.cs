using Revolutions.Components.Base.Settlements;
using Revolutions.Components.General.Screens.ViewModels;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace Revolutions.Components.General.Screens
{
    public class TownRevoltsScreen : ScreenBase
    {
        private readonly SettlementInfo _settlementInfo;
        private TownRevoltViewModel _dataSource;
        private GauntletLayer _gauntletLayer;

        public TownRevoltsScreen(SettlementInfo settlementInfo)
        {
            this._settlementInfo = settlementInfo;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new TownRevoltViewModel(this._settlementInfo);
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };

            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);
            this._gauntletLayer.LoadMovie("TownRevoltScreen", this._dataSource);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();

            this._gauntletLayer = null;
            this._dataSource = null;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            this.RemoveLayer(this._gauntletLayer);
            this._gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(this._gauntletLayer);
        }
    }
}