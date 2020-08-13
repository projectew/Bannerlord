using Revolutions.Library.Components.Events;
using Revolutions.Library.Components.Plots;
using Revolutions.Module.Components.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.CivilWars.Events.Plotting
{
    internal class PlottingEventOptionLoyal : EventOption
    {
        public PlottingEventOptionLoyal()
        {

        }

        public PlottingEventOptionLoyal(string id, string text) : base(id, text)
        {

        }

        public override void Invoke()
        {
            var mainHeroInfo = Managers.Character.Get(Hero.MainHero.CharacterObject);
            mainHeroInfo.PlotState = PlotState.IsLoyal;
            mainHeroInfo.DecisionMade = DecisionMade.Yes;
        }
    }
}