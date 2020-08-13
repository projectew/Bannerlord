using System;
using Revolutions.Library.Components.Clans;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Clans
{
    [Serializable]
    public class ClanInfo : BaseClanInfo
    {
        public ClanInfo()
        {

        }

        public ClanInfo(Clan clan) : base(clan)
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

        public bool CanChangeKingdom { get; set; } = true;

        public bool IsRevoltClan { get; set; } = false;

        public bool IsCivilWarClan { get; set; } = false;

        #endregion
    }
}