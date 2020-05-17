using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.Plotting
{
    internal class PlottingEventOptionPlotter : EventOption
    {
        public PlottingEventOptionPlotter() : base()
        {

        }

        public PlottingEventOptionPlotter(string id, string text) : base(id, text)
        {

        }

        public override void Invoke()
        {
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState = PlotState.IsPlotting;
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade = DecisionMade.Yes;
        }
    }
}