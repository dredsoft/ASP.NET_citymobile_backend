using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Enums
{
    public enum ErrorCode
    {
        None = 0,

        [Description("You are forbidden to access this resource. ")]
        Forbidden = 403 ,

        [Description("The precondition given in one or more of the request-header fields evaluated to false when it was tested on the server.")]
        PreconditionFailed = 412,

        [Description("Unknown server error")]
        Server = 500
    }
}
