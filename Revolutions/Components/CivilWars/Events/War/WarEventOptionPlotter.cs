using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Characters;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEventOptionPlotter : EventOption
    {
        private Kingdom _plotKingdom = null;
        
        public WarEventOptionPlotter() : base()
        {

        }

        public WarEventOptionPlotter(string id, string text, Kingdom plotKingdom) : base(id, text)
        {
            _plotKingdom = plotKingdom;
        }

        public override void Invoke()
        {
            ChangeKingdomAction.ApplyByJoinToKingdom(Hero.MainHero.Clan, _plotKingdom, false);
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState = PlotState.IsPlotting;
            Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade = DecisionMade.Yes;
        }
    }
}