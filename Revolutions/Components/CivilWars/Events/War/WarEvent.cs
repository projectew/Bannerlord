using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.Events.War
{
    internal class WarEvent : Event
    {
        internal WarEvent(Kingdom plotKingdom, Kingdom currentKingdom) : base()
        {
            Id = "revolutions_civilwars_war";
            Description = "The plotters will declare war on the king. Do you want to take part or stay with you king?";
            Sprite = "Revolutions.PlottingLords";
            
            if (currentKingdom.Leader != Hero.MainHero)
            {
                var optionOne = new WarEventOptionPlotter("OptionOne", "You're right, I'll be with you! (Join Plotters)", plotKingdom);
                this.AddOption(optionOne);
            }
            
            var optionTwo = new WarEventOptionLoyal("OptionTwo", "How dare you? For the king! (Join Loyals)");
            
            this.AddOption(optionTwo);
            this.Invoke();
        }
    }
}