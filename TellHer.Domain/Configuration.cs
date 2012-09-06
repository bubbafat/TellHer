using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TellHer.Domain
{
    public static class Configuration
    {
        public static IConfiguration GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<IConfiguration>();
        }
    }
}
