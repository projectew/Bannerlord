using KNTLibrary.Components.Events;

namespace Revolutions.Components.CivilWars.Events.Plotting
{
    internal class PlottingEvent : Event
    {
        internal PlottingEvent() : base()
        {
            Id = "revolutions_civilwars_plotting";
            Description = "Some one tells you that members of your kingdom are starting a plot. Do you want to take part?";
            Sprite = "Revolutions.PlottingLords";
            var optionOne = new PlottingEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)");
            var optionTwo = new PlottingEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            var optionThree = new PlottingEventOptionUndecided("OptionThree", "I need more time to consider this. (Wait 7 days until decide)");
            this.AddOption(optionOne);
            this.AddOption(optionTwo);
            this.AddOption(optionThree);
            this.Invoke();
        }
    }
}