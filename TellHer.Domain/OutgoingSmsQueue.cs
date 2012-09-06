

namespace TellHer.Domain
{
    public static class OutgoingSmsQueue
    {
        public static IOutgoingSmsQueue GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<IOutgoingSmsQueue>();
        }
    }
}
