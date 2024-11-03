using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ResultPattern
{
    public enum ReasonType
    {
        None = 0,
        Undefine = 1,
        Failure = 2,
        BadParameter = 3,
        Unexpected = 4,
        NotFound,
        RefreshTokenFaillure,
        NotImplemented
    }
}
