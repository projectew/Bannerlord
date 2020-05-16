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
        //data goes here

        public EventViewModel(List<Option> options, TextObject description)
        {
            _options = options;
            _description = description;
            //Data init here here
        }
        
        private void ExitMenu()
        {
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {

        }
    }
}