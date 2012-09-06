

namespace TellHer.Domain
{
    public static class SmsTransport
    {
        public static ISmsTransport GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<ISmsTransport>();
        }
    }
}
