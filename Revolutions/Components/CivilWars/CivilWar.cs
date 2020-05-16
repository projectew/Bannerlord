using KNTLibrary.Components;
using Revolutions.Components.Base.Kingdoms;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars
{
    [Serializable]
    public class CivilWar : IBaseInfoType, IBaseComponent<CivilWar>
    {
        #region IGameComponent<InfoType>

        public bool Equals(CivilWar other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is CivilWar info)
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

        public CivilWar()
        {

        }

        public CivilWar(Kingdom kingdom, List<Clan> clans)
        {
            this.Id = kingdom.StringId;
            this.KingdomId = kingdom.StringId;
            this.ClanIds = clans.Select(go => go.StringId).ToList();
        }

        #region Reference Properties

        public string Id { get; set; }

        public string KingdomId { get; set; }

        public List<string> ClanIds { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Kingdom Kingdom => Managers.Kingdom.GetGameObject(this.KingdomId);

        public KingdomInfo KingdomInfo => Managers.Kingdom.GetInfo(this.Kingdom);

        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}