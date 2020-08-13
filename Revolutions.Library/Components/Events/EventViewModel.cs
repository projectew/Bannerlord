using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Library.Components.Events
{
    public class EventViewModel : ViewModel
    {
        private readonly EventOption _optionOne;

        private readonly EventOption _optionTwo;

        private readonly EventOption _optionThree;

        private readonly Event _event;

        public EventViewModel(string description, string sprite, List<EventOption> options, Event eventobj)
        {
            _optionOne = options[0];

            if (options.Count > 1)
            {
                _optionTwo = options[1];
            }

            if (options.Count > 2)
            {
                _optionThree = options[2];
            }

            Description = description;
            Sprite = sprite;
            _event = eventobj;
        }

        [DataSourceProperty]
        public string Description { get; }

        [DataSourceProperty]
        public string Sprite { get; }

        [DataSourceProperty]
        public string OptionOneText => new TextObject(_optionOne.Text).ToString();

        [DataSourceProperty]
        public string OptionTwoText => new TextObject(_optionTwo.Text).ToString();

        [DataSourceProperty]
        public string OptionThreeText => new TextObject(_optionThree.Text).ToString();

        private void InvokeOptionOne()
        {
            _optionOne.Invoke();
            ExitMenu();
        }

        private void InvokeOptionTwo()
        {
            _optionTwo.Invoke();
            ExitMenu();
        }

        private void InvokeOptionThree()
        {
            _optionThree.Invoke();
            ExitMenu();
        }

        private void ExitMenu()
        {
            EventManager.Instance.EndEvent(_event);
            Game.Current.GameStateManager.ActiveStateDisabledByUser = false;
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged("Description");
            OnPropertyChanged("OptionOneText");
            OnPropertyChanged("OptionTwoText");
            OnPropertyChanged("OptionThreeText");
            OnPropertyChanged("Sprite");
        }
    }
}