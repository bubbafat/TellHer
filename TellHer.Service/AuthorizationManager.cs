

namespace TellHer.Service
{
    public static class AuthorizationManager
    {
        public static IServiceAuthorizationManager GetInstance()
        {
            return StructureMap.ObjectFactory.GetInstance<IServiceAuthorizationManager>();
        }
    }
}
