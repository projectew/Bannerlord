using KNTLibrary.Components.Events;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent() : base()
        {
            Id = "revolutions_civilwars_war";
            Description = "The plotters will declare war on the king. Do you want to take part or stay with you king?";
            Sprite = "Revolutions.PlottingLords";
            var optionOne = new WarEventOptionOne("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            var optionTwo = new WarEventOptionTwo("OptionTwo", "How dare you? For the king! (Join Loyals)");
            this.AddOption(optionOne);
            this.AddOption(optionTwo);
        }
    }
}