using KNTLibrary.Components.Events;
using KNTLibrary.Components.Plots;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEventOptionLoyal : EventOption
    {
        public WarEventOptionLoyal() : base()
        {

        }

        public WarEventOptionLoyal(string id, string text) : base(id, text)
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