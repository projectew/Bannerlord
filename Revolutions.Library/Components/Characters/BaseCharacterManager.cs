using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Revolutions.Library.Components.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Revolutions.Library.Components.Characters
{
    public class BaseCharacterManager<TInfoType> : IBaseManager<TInfoType, CharacterObject> where TInfoType : BaseCharacterInfo, new()
    {
        #region Singleton

        static BaseCharacterManager()
        {
            Instance = new BaseCharacterManager<TInfoType>();
        }

        public static BaseCharacterManager<TInfoType> Instance { get; private set; }

        #endregion

        #region IManager

        public bool DebugMode { get; set; }

        public HashSet<TInfoType> Infos { get; set; } = new HashSet<TInfoType>();

        public void Initialize()
        {
            if (Infos.Count == Campaign.Current.Characters.Count())
            {
                return;
            }

            foreach (var gameObject in Campaign.Current.Characters)
            {
                Get(gameObject);
            }
        }

        public void RemoveInvalids()
        {
            if (Infos.Count == Campaign.Current.Characters.Count())
            {
                return;
            }

            Infos.RemoveWhere(i => Campaign.Current.Characters.All(go => go.StringId != i.Id));
        }

        public void RemoveDuplicates()
        {
            Infos.Reverse();
            Infos = Infos.GroupBy(i => i.Id)
                                   .Select(i => i.First())
                                   .ToHashSet();
            Infos.Reverse();
        }

        public TInfoType Get(CharacterObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            var infos = Infos.Where(i => i.Id == gameObject.StringId);
            var baseCharacterInfos = infos as TInfoType[] ?? infos.ToArray();
            if (baseCharacterInfos.Count() > 1)
            {
                RemoveDuplicates();
            }

            var info = baseCharacterInfos.FirstOrDefault();
            if (info != null)
            {
                return info;
            }

            info = (TInfoType)Activator.CreateInstance(typeof(TInfoType), gameObject);
            Infos.Add(info);

            return info;
        }

        public TInfoType Get(string id)
        {
            var gameObject = GetGameObject(id);
            return gameObject == null ? null : Get(gameObject);
        }

        public void Remove(string id)
        {
            Infos.RemoveWhere(i => i.Id == id);
        }

        public CharacterObject GetGameObject(string id)
        {
            return Campaign.Current.Characters.FirstOrDefault(go => go.StringId == id);
        }

        public CharacterObject GetGameObject(TInfoType info)
        {
            return GetGameObject(info.Id);
        }

        #endregion

        public Hero CreateRandomLeader(Clan clan, BaseSettlementInfo settlementInfo)
        {
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

            var hero = HeroCreator.CreateSpecialHero(characterTemplate, settlementInfo.Settlement);
            hero.IsNoble = true;
            hero.IsMinorFactionHero = false;

            foreach (var perk in PerkObject.All)
            {
                hero.SetPerkValue(perk, true);
            }

            hero.ChangeState(Hero.CharacterStates.Active);

            Get(hero.CharacterObject).IsCustomCharacter = true;
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
            ModifyCharacterList(characters =>
            {
                return characters.RemoveAll(go => go.StringId == character.StringId) > 0 ? characters : null;
            });

            Remove(character.StringId);
        }

        public void DestroyCharacter(CharacterObject character)
        {
            KillCharacterAction.ApplyByRemove(character.HeroObject);
            RemoveCharacter(character);
        }
    }
}