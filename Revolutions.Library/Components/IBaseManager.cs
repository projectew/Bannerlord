using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace Revolutions.Library.Components
{
    public interface IBaseManager<TInfoType, TGameObjectType> where TGameObjectType : MBObjectBase
    {
        HashSet<TInfoType> Infos { get; set; }

        void Initialize();

        void RemoveInvalids();

        void RemoveDuplicates();

        TInfoType Get(TGameObjectType gameObject);

        TInfoType Get(string id);

        void Remove(string id);

        TGameObjectType GetGameObject(string id);

        TGameObjectType GetGameObject(TInfoType info);
    }
}