using HarmonyLib;
using KNTLibrary.Components.Settlements;
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

        static BaseCharacterManager()
        {
            Instance = new BaseCharacterManager<InfoType>();
        }

        public static BaseCharacterManager<InfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public bool DebugMode { get; set; }

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();

        public void Initialize()
        {
            if (this.Infos.Count == Campaign.Current.Characters.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                this.Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (this.Infos.Count == Campaign.Current.Characters.Count())
            {
                return;
            }

            this.Infos.RemoveWhere(i => !Campaign.Current.Characters.Any(go => go.StringId == i.Id));
        }

        public void RemoveDuplicates()
        {
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            this.Infos.Reverse();
        }

        public InfoType Get(CharacterObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = this.Infos.Where(i => i.Id == gameObject.StringId);
            if (infos.Count() > 1)
            {
                this.RemoveDuplicates();
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

        public InfoType Get(string id)
        {
            var gameObject = this.GetGameObject(id);
            if (gameObject == null)
            {
                return null;
            }

            return this.Get(gameObject);
        }

        public void Remove(string id)
        {
            this.Infos.RemoveWhere(i => i.Id == id);
        }

        public CharacterObject GetGameObject(string id)
        {
            return Campaign.Current.Characters.FirstOrDefault(go => go.StringId == id);
        }

        public CharacterObject GetGameObject(InfoType info)
        {
            return this.GetGameObject(info.Id);
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
                characterTemplate.SetAttributeValue(attribute.AttributeEnum, 255);
            }

            foreach (var skill in SkillObject.All)
            {
                characterTemplate.SetSkillValue(skill, 255);
            }

            characterTemplate.SetSkillValue(SkillObject.GetSkill(13), 1000);

            hero = HeroCreator.CreateSpecialHero(characterTemplate, settlementInfo.Settlement, null, null, -1);
            hero.IsNoble = true;
            hero.IsMinorFactionHero = false;

            foreach (var perk in PerkObject.All)
            {
                hero.SetPerkValue(perk, true);
            }

            hero.ChangeState(Hero.CharacterStates.Active);

            this.Get(hero.CharacterObject).IsCustomCharater = true;
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
            this.ModifyCharacterList(characters =>
            {
                if (characters.RemoveAll(go => go.StringId == character.StringId) > 0)
                {
                    return characters;
                }

                return null;
            });

            this.Remove(character.StringId);
        }

        public void DestroyCharacter(CharacterObject character)
        {
            KillCharacterAction.ApplyByRemove(character.HeroObject);
            this.RemoveCharacter(character);
        }
    }
}