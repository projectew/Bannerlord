using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent(Kingdom plotKingdom) : base()
        {
            Id = "revolutions_civilwars_war";
            Description = "The plotters will declare war on the king. Do you want to take part or stay with you king?";
            Sprite = "Revolutions.PlottingLords";
            var optionOne = new WarEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)", plotKingdom);
            var optionTwo = new WarEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            this.AddOption(optionOne);
            this.AddOption(optionTwo);
            this.Invoke();
        }
    }
}