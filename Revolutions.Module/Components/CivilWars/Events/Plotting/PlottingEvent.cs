using Revolutions.Library.Components.Events;

namespace Revolutions.Module.Components.CivilWars.Events.Plotting
{
    internal class PlottingEvent : Event
    {
        internal PlottingEvent()
        {
            Id = "revolutions_civilwars_plotting";
            Description = "Some one tells you that members of your kingdom are starting a plot. Do you want to take part?";
            Sprite = "Revolutions.PlottingLords";

            var optionOne = new PlottingEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            AddOption(optionOne);

            var optionTwo = new PlottingEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            AddOption(optionTwo);

            var optionThree = new PlottingEventOptionUndecided("OptionThree", "I need more time to consider this. (Wait 7 days until decide)");
            AddOption(optionThree);

            Invoke();
        }
    }
}