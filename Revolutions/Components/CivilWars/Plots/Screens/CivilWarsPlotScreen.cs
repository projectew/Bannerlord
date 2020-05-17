using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace Revolutions.Components.CivilWars.Plots.Screens
{
    public class CivilWarsPlotScreen : ScreenBase
    {
        private readonly string _movieName;
        private CivilWarsPlotViewModel _dataSource;
        private GauntletLayer _gauntletLayer;

        public CivilWarsPlotScreen()
        {
            this._movieName = "CivilWarsPlotScreen";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new CivilWarsPlotViewModel();
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);
            this._gauntletLayer.LoadMovie(this._movieName, this._dataSource);
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