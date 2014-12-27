using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahjongXNA
{
    public class EventArg<T> : EventArgs
    {
        // Property variable
        private readonly T p_EventData;

        // Constructor
        public EventArg(T data)
        {
            p_EventData = data;
        }

        // Property for EventArgs argument
        public T Data
        {
            get { return p_EventData; }
        }
    }
}
