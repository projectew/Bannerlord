using KNTLibrary.Components.Events;
using KNTLibrary.Components.Plots;
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
            var mainHeroInfo = Managers.Character.GetInfo(Hero.MainHero.CharacterObject);
            mainHeroInfo.PlotState = PlotState.IsLoyal;
            mainHeroInfo.DecisionMade = DecisionMade.Yes;
        }
    }
}