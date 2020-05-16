using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace KNTLibrary.Components.Plots
{
    public class PlotScreen : ScreenBase
    {
        private readonly string _movieName;
        private PlotViewModel _dataSource;
        private GauntletLayer _gauntletLayer;

        public PlotScreen()
        {
            this._movieName = "PlotScreen";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new PlotViewModel();
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