using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Characters
{
    [Serializable]
    public class BaseCharacterInfo : IBaseInfoType, IBaseComponent<IBaseInfoType>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseCharacterInfo other)
        {
            return Id == other.Id;
        }

        bool IEquatable<IBaseInfoType>.Equals(IBaseInfoType other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseCharacterInfo info)
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

        public BaseCharacterInfo()
        {

        }

        public BaseCharacterInfo(CharacterObject character)
        {
            Id = character.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public CharacterObject Character => BaseManagers.Character.GetGameObject(Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomCharacter { get; set; } = false;

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}