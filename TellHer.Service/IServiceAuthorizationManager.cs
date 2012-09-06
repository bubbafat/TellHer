using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TellHer.Service
{
    public interface IServiceAuthorizationManager
    {
        bool CheckAccess(OperationContext operationContext, ref Message message);
    }
}
