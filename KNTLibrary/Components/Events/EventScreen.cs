using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace KNTLibrary.Components.Events
{
    public class EventScreen : ScreenBase
    {
        private readonly string _movieName;
        private EventViewModel _dataSource;
        private GauntletLayer _gauntletLayer;

        private readonly string _description;
        private readonly string _sprite;
        private readonly List<EventOption> _options;
        private readonly Event _event;
        
        public EventScreen(string description, string sprite, List<EventOption> options, Event eventobj)
        {
            this._description = description;
            this._sprite = sprite;
            this._options = options;
            this._event = eventobj;

            if (options.Count() == 1)
            {
                this._movieName = "OneOptionEvent";
            }
            else if (options.Count() == 2)
            {
                this._movieName = "TwoOptionEvent";
            }
            else if (options.Count() == 3)
            {
                this._movieName = "ThreeOptionEvent";
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this._dataSource = new EventViewModel(this._description, this._sprite, this._options, this._event);
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