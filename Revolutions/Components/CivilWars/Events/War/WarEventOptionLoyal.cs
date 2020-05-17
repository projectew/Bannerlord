using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEventOptionLoyal : EventOption
    {
        private Kingdom _plotKingdom = null;
        
        public WarEventOptionLoyal() : base()
        {

        }

        public WarEventOptionLoyal(string id, string text) : base(id, text)
        {
            
        }

        public override void Invoke()
        {
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState = PlotState.IsPlotting;
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade = DecisionMade.Yes;
        }
    }
}