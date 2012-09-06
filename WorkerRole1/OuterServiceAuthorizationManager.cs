using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Service;
using System.ServiceModel;


namespace WorkerRole1
{
    public class OuterServiceAuthorizationManager : ServiceAuthorizationManager
    {
        IServiceAuthorizationManager _manager;

        public OuterServiceAuthorizationManager()
        {
            _manager = AuthorizationManager.GetInstance();
        }

        public override bool CheckAccess(OperationContext operationContext, ref System.ServiceModel.Channels.Message message)
        {
            return _manager.CheckAccess(operationContext, ref message);
        }
    }
}
