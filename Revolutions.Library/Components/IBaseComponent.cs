using System;

namespace Revolutions.Library.Components
{
    public interface IBaseComponent<TInfoType> : IEquatable<TInfoType> where TInfoType : IBaseInfoType
    {

    }
}