using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Localization;

namespace KNTLibrary.Components.Events
{
    public class EventScreen : ScreenBase
    {
        private EventViewModel _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;

        private string _movieName;
        private List<Option> _options;
        private TextObject _description;

        protected override void OnFinalize()
        {
            base.OnFinalize();

            this._gauntletLayer = null;
            this._dataSource = null;
            this._movie = null;
        }

        public EventScreen(List<Option> options, TextObject description)
        {
            _options = options;
            _description = description;

            if (options.Count() == 1)
            {
                _movieName = "OneOptionEvent";
            }
            else if (options.Count() == 2)
            {
                _movieName = "TwoOptionEvent";
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new EventViewModel(this._options, this._description);
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };

            this.AddLayer(this._gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(this._gauntletLayer);

            this._movie = this._gauntletLayer.LoadMovie(_movieName, this._dataSource);
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);

            _ = this._gauntletLayer.Input;
        }

        protected override void OnDeactivate()
        {
            this.RemoveLayer((ScreenLayer)this._gauntletLayer);
            this._gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus((ScreenLayer)this._gauntletLayer);
            base.OnDeactivate();
        }
    }
}