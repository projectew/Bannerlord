using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Characters
{
    [Serializable]
    public class BaseCharacterInfo : IBaseInfoType, IBaseComponent<IBaseInfoType>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseCharacterInfo other)
        {
            return this.Id == other.Id;
        }

        bool IEquatable<IBaseInfoType>.Equals(IBaseInfoType other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseCharacterInfo info)
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

        public BaseCharacterInfo()
        {

        }

        public BaseCharacterInfo(CharacterObject character)
        {
            this.Id = character.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public CharacterObject Character => BaseManagers.Character.GetGameObject(this.Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomCharater { get; set; } = false;

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}