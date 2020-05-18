using System;
using System.Collections.Generic;
using System.Linq;
using KNTLibrary.Components.Events;
using Revolutions.Components.Base.Kingdoms;
using Revolutions.Components.Base.Parties;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Revolutions.CampaignBehaviors
{
    internal class CleanupBehavior : CampaignBehaviorBase
    {
        private const int RefreshAtTick = 0;

        private int _currentTick = 0;

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));

            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyedEvent));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartyRemovedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void TickEvent(float dt)
        {
            if (this._currentTick == CleanupBehavior.RefreshAtTick + 60)
            {
                this.HandleEventCalls();
            }

            //Assuming, that we have 30 ticks per second, we update one of our data pieces once per second. So after 6 seconds all data was updated one time.
            switch (this._currentTick)
            {
                case CleanupBehavior.RefreshAtTick:
                    Managers.Faction.UpdateInfos();
                    break;
                case CleanupBehavior.RefreshAtTick + 30:
                    Managers.Kingdom.UpdateInfos();
                    break;
                case CleanupBehavior.RefreshAtTick + 60:
                    Managers.Clan.UpdateInfos();
                    break;
                case CleanupBehavior.RefreshAtTick + 90:
                    Managers.Settlement.UpdateInfos();
                    break;
                case CleanupBehavior.RefreshAtTick + 120:
                    Managers.Character.UpdateInfos();
                    this._currentTick = 0;
                    break;
                default:
                    this._currentTick++;
                    break;
            }
        }

        private void HandleEventCalls()
        {
            if (!EventManager.Instance.InEvent && EventManager.Instance.Events.Count() > 0)
            {
                EventManager.Instance.Events[0].Invoke();
            }
        }

        private void DailyTickEvent()
        {
            Managers.Faction.CleanupDuplicatedInfos();
            Managers.Kingdom.CleanupDuplicatedInfos();
            Managers.Clan.CleanupDuplicatedInfos();
            Managers.Party.CleanupDuplicatedInfos();
            Managers.Character.CleanupDuplicatedInfos();
            Managers.Settlement.CleanupDuplicatedInfos();
            DestroyGhostKingdoms();
            Managers.Party.UpdateInfos();
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            var kingdomInfo = Managers.Kingdom.GetInfo(kingdom);
            if (kingdomInfo != null && kingdomInfo.IsCustomKingdom)
            {
                Managers.Kingdom.RemoveKingdom(kingdom);
            }
        }

        private void DestroyGhostKingdoms()
        {
            int kingdomsCount = Managers.Kingdom.Infos.Count();
            for (int i = 0; i < kingdomsCount; i++)
            {
                var kingdomInfo = Managers.Kingdom.Infos.ElementAtOrDefault(i);
                if (kingdomInfo.IsCustomKingdom && kingdomInfo.Kingdom.Leader.IsDead || 
                    kingdomInfo.Kingdom.Leader.IsPrisoner && !kingdomInfo.Kingdom.Parties.Any())
                {
                    DestroyKingdomAction.Apply(kingdomInfo.Kingdom);
                    kingdomsCount = Managers.Kingdom.Infos.Count();
                    i--;
                }
            }
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var clanInfo = Managers.Clan.GetInfo(clan);
            if (clanInfo != null && clanInfo.IsCustomClan)
            {
                Managers.Clan.RemoveClan(clan);
            }
        }

        private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase party)
        {
            PartyInfo partyInfo;

            if (party != null)
            {
                partyInfo = Managers.Party.GetInfo(party);
            }
            else if (mobileParty.Party != null)
            {
                partyInfo = Managers.Party.GetInfo(mobileParty.Party);
            }
            else
            {
                return;
            }

            if (partyInfo != null && partyInfo.IsCustomParty)
            {
                Managers.Party.RemoveInfo(mobileParty.Party.Id);
            }
        }

        private void OnPartyRemovedEvent(PartyBase party)
        {
            var partyInfo = Managers.Party.GetInfo(party);
            if (partyInfo != null && partyInfo.IsCustomParty)
            {
                Managers.Party.RemoveInfo(party.Id);
            }
        }
    }
}