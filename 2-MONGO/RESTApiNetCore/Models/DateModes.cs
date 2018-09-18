using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApiNetCore.Models
{
    public enum DateModes
    {
        FilterDateBefore,
        FilterDateEqual,
        FilterDateAfter,
        UnknownMode
    }
}
