using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;


namespace TellHer.Data
{
    public static class DataStoreInitialize
    {
        public static void Initialize(IInitializationExpression x)
        {
            x.For<IDataStore>().Use<TellHerDb>();
        }
    }
}
