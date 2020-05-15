using HarmonyLib;
using KNTLibrary.Components.Settlements;
using KNTLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace KNTLibrary.Components.Characters
{
    public class BaseCharacterManager<InfoType> : IBaseManager<InfoType, CharacterObject> where InfoType : BaseCharacterInfo, new()
    {
        #region Singleton

        private BaseCharacterManager() { }

        static BaseCharacterManager()
        {
            BaseCharacterManager<InfoType>.Instance = new BaseCharacterManager<InfoType>();
        }

        public static BaseCharacterManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public bool DebugMode { get; set; }

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void InitializeInfos()
        {
            if (this.Infos.Count() == Campaign.Current.Characters.Count)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                this.GetInfo(gameObject);
            }
        }

        public InfoType GetInfo(CharacterObject gameObject)
        {
            var infos = this.Infos.ToList().Where(i => i.CharacterId == gameObject.StringId);
            if (this.DebugMode && infos.Count() > 1)
            {
                InformationManager.DisplayMessage(new InformationMessage("Revolutions: Multiple Characters with same Id. Using first one.", ColorHelper.Orange));
                foreach (var duplicatedInfo in infos)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Name: {duplicatedInfo.Character.Name} | StringId: {duplicatedInfo.CharacterId}", ColorHelper.Orange));
                }
            }

            var info = infos.FirstOrDefault();
            if (info != null)
            {
                return info;
            }

            info = (InfoType)Activator.CreateInstance(typeof(InfoType), gameObject);
            this.Infos.Add(info);

            return info;
        }

        public InfoType GetInfo(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.GetInfo(gameObject);
        }

        public void RemoveInfo(string id)
        {
            var info = this.Infos.FirstOrDefault(i => i.CharacterId == id);
            if (id == null)
            {
                return;
            }

            this.Infos.RemoveWhere(i => i.CharacterId == id);
        }

        public CharacterObject GetGameObject(string id)
        {
            return Campaign.Current.Characters.FirstOrDefault(go => go.StringId == id);
        }

        public CharacterObject GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.CharacterId);
        }

        public void UpdateInfos(bool onlyRemoving = false)
        {
            if (this.Infos.Count == Campaign.Current.Characters.Count)
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Characters.Any(go => go.StringId == i.CharacterId));

            if (onlyRemoving)
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters.Where(go => !this.Infos.Any(i => i.CharacterId == go.StringId)))
            {
                this.GetInfo(gameObject);
            }
        }

        public void CleanupDuplicatedInfos()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.CharacterId)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        #endregion

        public Hero CreateRandomLeader(Clan clan, BaseSettlementInfo settlementInfo)
        {
            var random = new Random();
            Hero hero = null;

            var templateBase = clan.Leader.CharacterObject;

            var characterTemplate = CharacterObject.Templates.Where(go => go.Culture == clan.Culture && (go.Occupation == Occupation.Lord || go.Occupation == Occupation.Lady)).GetRandomElement();
            characterTemplate.InitializeEquipmentsOnLoad(templateBase.AllEquipments.ToList());

            foreach (var attribute in CharacterAttributes.All)
            {
                characterTemplate.SetAttributeValue(attribute.AttributeEnum, templateBase.GetAttributeValue(attribute.AttributeEnum));
            }

            foreach (var skill in SkillObject.All)
            {
                characterTemplate.SetSkillValue(skill, templateBase.GetSkillValue(skill));
            }

            hero = HeroCreator.CreateSpecialHero(characterTemplate, settlementInfo.Settlement, null, null, -1);
            hero.ChangeState(Hero.CharacterStates.NotSpawned);
            hero.IsNoble = true;
            hero.IsMinorFactionHero = false;
            hero.ChangeState(Hero.CharacterStates.Active);

            this.GetInfo(hero.CharacterObject).IsCustomCharater = true;
            return hero;
        }

        public void ModifyCharacterList(Func<List<CharacterObject>, List<CharacterObject>> modificator)
        {
            var characters = modificator(Campaign.Current.Characters.ToList());
            if (characters != null)
            {
                AccessTools.Field(Campaign.Current.GetType(), "_characters").SetValue(Campaign.Current, new MBReadOnlyList<CharacterObject>(characters));
            }
        }

        public void RemoveCharacter(CharacterObject character)
        {
            this.ModifyCharacterList(gos =>
            {
                if (gos.RemoveAll(go => go.StringId == character.StringId) > 0)
                {
                    return gos;
                }

                return null;
            });

            this.RemoveInfo(character.StringId);
        }

        public void DestroyCharacter(CharacterObject character)
        {
            KillCharacterAction.ApplyByRemove(character.HeroObject);
            this.RemoveCharacter(character);
        }
    }
}