

namespace TellHer.Sms
{
    public static class RequestValidator
    {
        public static ITwilioRequestValidator GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<ITwilioRequestValidator>();
        }
    }
}
