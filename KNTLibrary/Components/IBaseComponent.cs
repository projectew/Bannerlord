using System;

namespace KNTLibrary.Components
{
    public interface IBaseComponent<InfoType> : IEquatable<InfoType> where InfoType : IBaseInfoType
    {

    }
}