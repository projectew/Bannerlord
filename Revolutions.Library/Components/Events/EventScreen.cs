using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;

namespace Revolutions.Library.Components.Events
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
            _description = description;
            _sprite = sprite;
            _options = options;
            _event = eventobj;

            if (options.Count == 1)
            {
                _movieName = "OneOptionEvent";
            }
            else if (options.Count == 2)
            {
                _movieName = "TwoOptionEvent";
            }
            else if (options.Count == 3)
            {
                _movieName = "ThreeOptionEvent";
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _dataSource = new EventViewModel(_description, _sprite, _options, _event);
            _gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            AddLayer(_gauntletLayer);
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            ScreenManager.TrySetFocus(_gauntletLayer);
            _gauntletLayer.LoadMovie(_movieName, _dataSource);
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