using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TellHer.Data
{
    public static class DataStore
    {
        public static IDataStore GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<IDataStore>();
        }
    }
}
