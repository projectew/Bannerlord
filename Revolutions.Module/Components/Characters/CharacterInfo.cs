using System;
using TaleWorlds.CampaignSystem;
using Revolutions.Library.Components.Characters;
using Revolutions.Library.Components.Plots;

namespace Revolutions.Module.Components.Characters
{
    [Serializable]
    public class CharacterInfo : BaseCharacterInfo
    {
        public CharacterInfo()
        {

        }

        public CharacterInfo(CharacterObject character) : base(character)
        {

        }

        #region Reference Properties



        #endregion

        #region Virtual Objects

        #region Reference Properties



        #endregion

        #region Reference Properties Inherited



        #endregion

        #region Normal Properties



        #endregion

        #endregion

        #region Normal Properties

        public bool IsRevoltKingdomLeader { get; set; } = false;

        public bool IsCivilWarKingdomLeader { get; set; } = false;

        public PlotState PlotState { get; set; } = PlotState.IsLoyal;

        public DecisionMade DecisionMade { get; set; } = DecisionMade.No;

        #endregion
    }

    [Serializable]
    public enum PlotState
    {
        IsLoyal = 1,
        WillPlotting = 2,
        IsPlotting = 3,
        Considering = 4
    }
}