using Revolts.Screens.ViewModels;
using Revolutions.Components.BaseComponents.Factions;
using Revolutions.Components.BaseComponents.Settlements;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;

namespace Revolts.Screens
{
    public class TownRevoltsScreen : ScreenBase
    {
        private readonly SettlementInfo _settlementInfo;
        private readonly FactionInfo _factionInfo;
        private TownRevoltViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private bool _firstRender;

        protected override void OnFinalize()
        {
            base.OnFinalize();

            this._gauntletLayer = null;
            this._dataSource = null;
            this._movie = null;
        }

        public TownRevoltsScreen(SettlementInfo settlementInfo, FactionInfo factionInfo)
        {
            this._settlementInfo = settlementInfo;
            this._factionInfo = factionInfo;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new TownRevoltViewModel(this._settlementInfo, this._factionInfo);
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };

            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);

            this._movie = this._gauntletLayer.LoadMovie("TownRevoltScreen", this._dataSource);
            this._firstRender = true;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);

            _ = this._gauntletLayer.Input;
        }
    }
}