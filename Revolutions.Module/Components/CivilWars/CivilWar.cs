using System;
using System.Collections.Generic;
using System.Linq;
using Revolutions.Library.Components;
using Revolutions.Module.Components.Kingdoms;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.CivilWars
{
    [Serializable]
    public class CivilWar : IBaseInfoType, IBaseComponent<CivilWar>
    {
        #region IGameComponent<InfoType>

        public bool Equals(CivilWar other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is CivilWar info)
            {
                return Id == info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        public CivilWar()
        {

        }

        public CivilWar(Kingdom kingdom, List<Clan> clans)
        {
            Id = kingdom.StringId;
            KingdomId = kingdom.StringId;
            ClanIds = clans.Select(go => go.StringId).ToList();
        }

        #region Reference Properties

        public string Id { get; set; }

        public string KingdomId { get; set; }

        public List<string> ClanIds { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Kingdom Kingdom => Managers.Kingdom.GetGameObject(KingdomId);

        public KingdomInfo KingdomInfo => Managers.Kingdom.Get(Kingdom);

        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}