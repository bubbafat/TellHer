using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.ServiceModel.Activation;

namespace TellHer.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITwilioSmsListener" in both code and config file together.
    [ServiceContract]
    public interface ITwilioSmsListener
    {
        [OperationContract]
        [WebInvoke(Method="POST")]
        void Incoming(Stream input);
    }
}
