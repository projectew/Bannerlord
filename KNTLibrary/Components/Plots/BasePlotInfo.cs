using System;
using System.Collections.Generic;
using System.Linq;

namespace KNTLibrary.Components.Plots
{
    [Serializable]
    public class BasePlotInfo<TargetObjectiveType, AttendeeType> : IEquatable<BasePlotInfo<TargetObjectiveType, AttendeeType>> where TargetObjectiveType : IBaseInfoType where AttendeeType : IBaseInfoType
    {
        #region IGameComponent<InfoType>

        public TargetObjectiveType TargetObjective { get; }

        public bool Equals(BasePlotInfo<TargetObjectiveType, AttendeeType> other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BasePlotInfo<TargetObjectiveType, AttendeeType> info)
            {
                return this.Id == info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion

        public BasePlotInfo()
        {

        }

        public BasePlotInfo(TargetObjectiveType targetObjective, List<AttendeeType> proAttendees, List<AttendeeType> conAttendees)
        {
            this.TargetObjectiveId = targetObjective.Id;
            this.ProAttendeeIds = proAttendees.Select(a => a.Id).ToList();
            this.ConAttendeeIds = conAttendees.Select(a => a.Id).ToList();
        }

        #region Reference Properties

        public string Id { get; set; }

        public string TargetObjectiveId { get; set; }

        public List<string> ProAttendeeIds { get; set; }

        public List<string> ConAttendeeIds { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects



        #endregion

        public int CountTotalAttendees => this.ProAttendeeIds.Count + this.ConAttendeeIds.Count;

        #endregion

        #region Normal Properties



        #endregion

        public virtual bool CanExecuteStart()
        {
            return true;
        }

        public virtual bool CanExecuteEnd()
        {
            return true;
        }

        public virtual void ExecuteStart()
        {

        }

        public virtual void ExecuteEnd()
        {

        }
    }
}