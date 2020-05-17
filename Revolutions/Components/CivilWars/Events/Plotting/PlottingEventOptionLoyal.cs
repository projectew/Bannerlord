using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.Plotting
{
    internal class PlottingEventOptionLoyal : EventOption
    {
        public PlottingEventOptionLoyal() : base()
        {

        }

        public PlottingEventOptionLoyal(string id, string text) : base(id, text)
        {

        }

        public override void Invoke()
        {
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState = PlotState.IsLoyal;
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade = DecisionMade.Yes;
        }
    }
}