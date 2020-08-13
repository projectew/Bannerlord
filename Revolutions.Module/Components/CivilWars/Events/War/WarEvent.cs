using Revolutions.Library.Components.Events;

namespace Revolutions.Module.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent()
        {
            Id = "revolutions_civilwars_war";
            Description = "The plotters will declare war on the king. Do you want to take part or stay with your king?";
            Sprite = "Revolutions.PlottingLords";

            var optionOne = new WarEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            AddOption(optionOne);

            var optionTwo = new WarEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            AddOption(optionTwo);

            Invoke();
        }
    }
}