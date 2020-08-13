using Revolutions.Library.Components.Events;
using Revolutions.Library.Components.Plots;
using Revolutions.Module.Components.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.CivilWars.Events.War
{
    internal class WarEventOptionLoyal : EventOption
    {
        public WarEventOptionLoyal()
        {

        }

        public WarEventOptionLoyal(string id, string text) : base(id, text)
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