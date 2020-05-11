using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars
{
    public class CivilWarManager
    {
        #region Singleton

        private CivilWarManager() { }

        static CivilWarManager()
        {
            CivilWarManager.Instance = new CivilWarManager();
        }

        public static CivilWarManager Instance { get; private set; }

        #endregion

        public HashSet<CivilWar> CivilWars = new HashSet<CivilWar>();

        public CivilWar GetCivilWarByKingdomId(string id)
        {
            return this.CivilWars.FirstOrDefault(i => i.KingdomId == id);
        }

        public CivilWar GetCivilWarByKingdom(Kingdom kingdom)
        {
            return this.GetCivilWarByKingdomId(kingdom.StringId);
        }

        public CivilWar GetCivilWarByClanId(string id)
        {
            return this.CivilWars.FirstOrDefault(i => i.ClanIds.Any(cid => cid == id));
        }

        public CivilWar GetCivilWarByClan(Clan clan)
        {
            return this.GetCivilWarByClanId(clan.StringId);
        }
    }
}
