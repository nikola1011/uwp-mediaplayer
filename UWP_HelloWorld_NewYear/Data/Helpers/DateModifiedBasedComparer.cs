using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_HelloWorld_NewYear.Data.Helpers
{
    public class DateModifiedBasedComparer : IComparer<SWatchFile>
    {
        public int Compare(SWatchFile x, SWatchFile y)
        {
            if (x.DateModified > y.DateModified)
                return 1;
            if (x.DateModified < y.DateModified)
                return -1;

            return 0;
        }
    }
}
