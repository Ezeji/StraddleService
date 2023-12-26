using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Models
{
    public enum AuthType
    {
        Bearer = 1,
        Basic
    }

    public enum CustomHttpMethod
    {
        Get = 1,
        Post,
        Put,
        Delete,
        Option
    }
}
