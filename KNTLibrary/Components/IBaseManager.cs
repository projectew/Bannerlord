using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace KNTLibrary.Components
{
    public interface IBaseManager<InfoType, GameObjectType> where GameObjectType : MBObjectBase
    {
        HashSet<InfoType> Infos { get; set; }

        void Initialize();

        void RemoveInvalids();

        void RemoveDuplicates();

        InfoType Get(GameObjectType gameObject);

        InfoType Get(string id);

        void Remove(string id);

        GameObjectType GetGameObject(string id);

        GameObjectType GetGameObject(InfoType info);
    }
}