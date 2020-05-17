using KNTLibrary.Components.Events;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent() : base()
        {
            var id = "revolutions_civilwars_plotting";
            var description = "The plotters will declare war on the king. Do you want to take part or stay with you king?";
            var sprite = "Revolutions.PlottingLords";
            var warEvent = new Event(id, description, sprite);
            var optionOne = new WarEventOptionOne("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            var optionTwo = new WarEventOptionTwo("OptionTwo", "How dare you? For the king! (Join Loyals)");
            warEvent.AddOption(optionOne);
            warEvent.AddOption(optionTwo);
            warEvent.Invoke();
        }
    }
}