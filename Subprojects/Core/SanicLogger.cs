using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanicballCore
{
    public static class SanicLogger
    {
        public static Action<object> DebugLog;
        public static Action<object> NormalLog;
        public static Action<object> WarnLog;
        public static Action<object> ErrorLog;
    }
}
