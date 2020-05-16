using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace KNTLibrary.Components.Plots.Screens
{
    public class PlotViewModel : ViewModel
    {
        public PlotViewModel()
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