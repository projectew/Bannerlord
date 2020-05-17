using KNTLibrary.Components.Plots;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.Base.Kingdoms;
using System;
using System.Collections.Generic;

namespace Revolutions.Components.CivilWars.Plots
{
    [Serializable]
    public class CivilWarsPlotInfo : BasePlotInfo<KingdomInfo, CharacterInfo>
    {
        public CivilWarsPlotInfo() : base()
        {

        }

        public CivilWarsPlotInfo(KingdomInfo targetObjective, List<CharacterInfo> proAttendees, List<CharacterInfo> conAttendee) : base(targetObjective, proAttendees, conAttendee)
        {

        }

        #region Reference Properties



        #endregion

        #region Virtual Objects

        #region Reference Properties Objects



        #endregion



        #endregion

        #region Normal Properties



        #endregion

        public override bool CanExecuteStart()
        {
            return true;
        }

        public override bool CanExecuteEnd()
        {
            return true;
        }

        public override void ExecuteStart()
        {

        }

        public override void ExecuteEnd()
        {

        }
    }
}