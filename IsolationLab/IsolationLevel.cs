using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsolationLab
{
    public enum IsolationLevel
    {
        Serializable = 0,
        ReadUncommitted = 1,
        ReadCommitted = 2,
        RepeatableRead = 3,
        Snapshot = 4,
    }
}
