using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace KNTLibrary.Components.Events
{
    public class EventViewModel : ViewModel
    {
        private readonly EventOption _optionOne;

        private readonly EventOption _optionTwo;

        private readonly EventOption _optionThree;

        public EventViewModel(string description, string sprite, List<EventOption> options)
        {
            this._optionOne = options[0];
            this._optionTwo = options[1];
            this._optionThree = options[2];
            this.Description = description.ToString();
            this.Sprite = sprite;
        }

        [DataSourceProperty]
        public string Description { get; }

        [DataSourceProperty]
        public string Sprite { get; }

        [DataSourceProperty]
        public string OptionOneText => new TextObject(this._optionOne.Text).ToString();

        [DataSourceProperty]
        public string OptionTwoText => new TextObject(this._optionTwo.Text).ToString();
        
        [DataSourceProperty]
        public string OptionThreeText => new TextObject(this._optionThree.Text).ToString();

        private void InvokeOptionOne()
        {
            this._optionOne.Invoke();
            this.ExitMenu();
        }

        private void InvokeOptionTwo()
        {
            this._optionTwo.Invoke();
            this.ExitMenu();
        }
        
        private void InvokeOptionThree()
        {
            this._optionThree.Invoke();
            this.ExitMenu();
        }

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
            this.OnPropertyChanged("OptionThreeText");
            this.OnPropertyChanged("Sprite");
        }
    }
}