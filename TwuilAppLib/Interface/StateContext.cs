using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppLib.Interface
{
    public interface StateContext<T>
    {

        T State { get; set; }

    }
}
