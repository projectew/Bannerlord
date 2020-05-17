using KNTLibrary.Components.Events;

namespace Revolutions.Components.CivilWars.Events.Plotting
{
    internal class PlottingEvent : Event
    {
        internal PlottingEvent() : base()
        {
            var id = "revolutions_civilwars_plotting";
            var description = "Some one tells you that members of your kingdom are starting a plot. Do you want to take part?";
            var sprite = "Revolutions.PlottingLords";
            var plottingEvent = new Event(id, description, sprite);
            var optionOne = new PlottingEventOptionOne("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            var optionTwo = new PlottingEventOptionTwo("OptionTwo", "How dare you? For the king! (Join Loyals)");
            var optionThree = new PlottingEventOptionThree("OptionThree", "I need more time to consider this. (Wait 7 days until decide)");
            plottingEvent.AddOption(optionOne);
            plottingEvent.AddOption(optionTwo);
            plottingEvent.AddOption(optionThree);
            plottingEvent.Invoke();
        }
    }
}