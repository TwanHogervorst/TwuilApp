using System;
using System.Collections.Generic;
using System.Text;

namespace TwuilAppLib.Interface
{
    public interface IStateContext<T>
    {

        T State { get; }

        void SetState(T newState);

        void SetState(Type newStateType);

    }
}
