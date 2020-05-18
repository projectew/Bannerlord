using KNTLibrary;
using Revolutions.Components.Base.Settlements;
using Revolutions.Settings;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Revolutions.Components.Revolts.CampaignBehaviors
{
    internal class RevoltBehavior : CampaignBehaviorBase
    {
        internal RevoltBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new RevoltsDailyBehavior());
            campaignGameStarter.AddBehavior(new LuckyNationBehaviour());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyedEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void MapEventEnded(MapEvent mapEvent)
        {
            var involvedParty = mapEvent.InvolvedParties.Intersect(RevoltManager.Instance.GetParties()).FirstOrDefault();
            if (involvedParty == null)
            {
                return;
            }

            var currentRevolt = Managers.Revolt.GetRevoltByPartyId(involvedParty.Id);

            var winnerSide = mapEvent.BattleState == BattleState.AttackerVictory ? mapEvent.AttackerSide : mapEvent.DefenderSide;
            if (winnerSide.PartiesOnThisSide.FirstOrDefault(party => party.Id == involvedParty.Id) == null)
            {
                Managers.Revolt.EndFailedRevolt(currentRevolt);
            }
            else
            {
                Managers.Revolt.EndSucceededRevolut(currentRevolt);
            }
        }

        private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturedHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            var settlementInfo = Managers.Settlement.GetInfo(settlement);
            settlementInfo.UpdateOwnerRevolt(newOwner.MapFaction);

            if (capturedHero?.PartyBelongedTo?.Party != null)
            {
                var revolt = RevoltManager.Instance.GetRevoltByParty(capturedHero.PartyBelongedTo.Party);
                if (revolt != null && !RevolutionsSettings.Instance.RevoltsMinorFactionsMechanic && revolt.IsMinorFaction)
                {
                    if (revolt.SettlementInfo.CurrentFaction == revolt.SettlementInfo.LoyalFaction)
                    {
                        var previousFactionOwner = BaseManagers.Faction.GetLordWithLeastFiefs(revolt.SettlementInfo.PreviousFaction).HeroObject;
                        ChangeOwnerOfSettlementAction.ApplyByRevolt(previousFactionOwner, settlement);
                    }
                    else
                    {
                        var loyalFactionOwner = BaseManagers.Faction.GetLordWithLeastFiefs(revolt.SettlementInfo.LoyalFaction).HeroObject;
                        ChangeOwnerOfSettlementAction.ApplyByRevolt(loyalFactionOwner, settlement);
                    }

                    Managers.Kingdom.DestroyKingdom(capturedHero.Clan.Kingdom);
                    Managers.Clan.DestroyClan(capturedHero.Clan);
                    capturedHero.PartyBelongedTo.RemoveParty();
                    KillCharacterAction.ApplyByRemove(capturedHero);

                    RevoltManager.Instance.Revolts.Remove(revolt);
                }
            }
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            var kingdomInfo = Managers.Kingdom.GetInfo(kingdom);
            if (kingdomInfo != null && kingdomInfo.IsRevoltKingdom)
            {
                Managers.Kingdom.RemoveKingdom(kingdom);
            }
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var clanInfo = Managers.Clan.GetInfo(clan);
            if (clanInfo != null && clanInfo.IsRevoltClan)
            {
                Managers.Clan.RemoveClan(clan);
            }
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            var clanInfo = Managers.Clan.GetInfo(clan);

            if (clan.StringId == Clan.PlayerClan.StringId || clanInfo.CanChangeKingdom || !clanInfo.IsRevoltClan || clan.StringId == newKingdom.RulingClan.StringId
                || clan.Culture.Name.ToString().ToLower().Contains("empire") && newKingdom.Culture.Name.ToString().ToLower().Contains("empire")
                || clan.Culture.Name.ToString() == newKingdom.Culture.Name.ToString())
            {
                return;
            }

            clan.ClanLeaveKingdom(false);
        }
    }
}