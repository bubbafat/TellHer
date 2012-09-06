using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Domain
{
    public interface IConfiguration
    {
        string AuthToken { get; }
        string AccountSid { get; }
        string AdminNumber { get; }
        string FromNumber { get; }
        int BetaLimit { get; }
    }
}
