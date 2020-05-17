using KNTLibrary.Components.Events;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent() : base()
        {
            this.Id = "revolutions_civilwars_war";
            this.Description = "The plotters will declare war on the king. Do you want to take part or stay with your king?";
            this.Sprite = "Revolutions.PlottingLords";

            var optionOne = new WarEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            this.AddOption(optionOne);

            var optionTwo = new WarEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            this.AddOption(optionTwo);

            this.Invoke();
        }
    }
}