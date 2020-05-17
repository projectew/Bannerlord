using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.Plotting
{
    internal class PlottingEventOptionUndecided : EventOption
    {
        public PlottingEventOptionUndecided() : base()
        {

        }

        public PlottingEventOptionUndecided(string id, string text) : base(id, text)
        {

        }

        public override void Invoke()
        {
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState = PlotState.Considering;
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade = DecisionMade.No;
        }
    }
}