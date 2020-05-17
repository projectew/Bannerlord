using System.Collections.Generic;

namespace KNTLibrary.Components.Plots
{
    public class BasePlotManager<InfoType> where InfoType : BasePlotInfo<IBaseInfoType, IBaseInfoType>, new()
    {
        #region Singleton

        private BasePlotManager() { }

        static BasePlotManager()
        {
            BasePlotManager<InfoType>.Instance = new BasePlotManager<InfoType>();
        }

        internal static BasePlotManager<InfoType> Instance { get; private set; }

        #endregion

        public HashSet<InfoType> Infos { get; set; } = new HashSet<InfoType>();
    }
}