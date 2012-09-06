

namespace TellHer.Domain
{
    public static class CommandProcessor
    {
        public static ICommandProcessor GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<ICommandProcessor>();
        }
    }
}
