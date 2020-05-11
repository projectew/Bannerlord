﻿using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Characters;

namespace Revolutions.Components.Base.Characters
{
    [Serializable]
    public class CharacterInfo : BaseCharacterInfo
    {
        public CharacterInfo() : base()
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



        #endregion

        #region Normal Properties

        public bool IsRevoltKingdomLeader { get; set; } = false;

        public PlotState PlotState { get; set; } = PlotState.Loyal;

        #endregion
    }

    [Serializable]
    public enum PlotState
    {
        Loyal = 0,
        WillPlotting = 1,
        IsPlotting = 2
    }
}