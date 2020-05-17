using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace Revolutions.Components.CivilWars.Plots.Screens
{
    public class CivilWarsPlotViewModel : ViewModel
    {
        public CivilWarsPlotViewModel()
        {

        }

        private void ExitMenu()
        {
            Game.Current.GameStateManager.ActiveStateDisabledByUser = false;
            ScreenManager.PopScreen();
        }

        private void RefreshProperties()
        {

        }
    }
}