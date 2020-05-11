using System;
using TaleWorlds.CampaignSystem;

namespace Revolts.CampaignBehaviors
{
    public class CleanupBehavior : CampaignBehaviorBase
    {
        private const int RefreshAtTick = 0;

        private int _currentTick = 0;

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));

            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.PartyRemovedEvent));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.ClanDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            RevoltsManagers.Faction.CleanupDuplicatedInfos();
            RevoltsManagers.Kingdom.CleanupDuplicatedInfos();
            RevoltsManagers.Clan.CleanupDuplicatedInfos();
            RevoltsManagers.Party.CleanupDuplicatedInfos();
            RevoltsManagers.Character.CleanupDuplicatedInfos();
            RevoltsManagers.Settlement.CleanupDuplicatedInfos();

            RevoltsManagers.Party.UpdateInfos();
        }

        private void TickEvent(float dt)
        {
            //Assuming, that we have 30 ticks per second, we update one of our data pieces once per second. So after 6 seconds all data was updated one time.
            switch (this._currentTick)
            {
                case RefreshAtTick:
                    RevoltsManagers.Faction.UpdateInfos();
                    break;
                case RefreshAtTick + 30:
                    RevoltsManagers.Kingdom.UpdateInfos();
                    break;
                case RefreshAtTick + 60:
                    RevoltsManagers.Clan.UpdateInfos();
                    break;
                case RefreshAtTick + 90:
                    RevoltsManagers.Settlement.UpdateInfos();
                    break;
                case RefreshAtTick + 120:
                    RevoltsManagers.Character.UpdateInfos();
                    this._currentTick = 0;
                    break;
                default:
                    this._currentTick++;
                    break;
            }
        }

        private void PartyRemovedEvent(PartyBase party)
        {
            RevoltsManagers.Party.RemoveInfo(party.Id);
        }

        private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase party)
        {
            RevoltsManagers.Party.RemoveInfo(mobileParty.Party.Id);
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            RevoltsManagers.Kingdom.RemoveInfo(kingdom.StringId);
        }

        private void ClanDestroyedEvent(Clan clan)
        {
            RevoltsManagers.Clan.RemoveInfo(clan.StringId);
        }
    }
}