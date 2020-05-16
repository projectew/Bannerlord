using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace KNTLibrary.Components.Events
{
    public class EventViewModel : ViewModel
    {
        private List<Option> _options;

        private TextObject _description;

        private string _sprite;
        //data goes here

        public EventViewModel(List<Option> options, TextObject description, string sprite)
        {
            _options = options;
            _description = description;
            _sprite = sprite;
        }

        [DataSourceProperty]
        public string Description => _description.ToString();

        [DataSourceProperty]
        public string OptionOneText => _options[0].Text.ToString();

        [DataSourceProperty]
        public string OptionTwoText => _options[1].Text.ToString();

        [DataSourceProperty] 
        public string Sprite => _sprite;
        
        private void ExitMenu()
        {
            Game.Current.GameStateManager.ActiveStateDisabledByUser = false;
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged("Description");
            this.OnPropertyChanged("OptionOneText");
            this.OnPropertyChanged("OptionTwoText");
            this.OnPropertyChanged("Sprite");
        }

        private void OptionOneTrigger()
        {
            _options[0].Result();
            ExitMenu();
        }

        private void OptionTwoTrigger()
        {
            _options[1].Result();
            ExitMenu();
        }
    }
}