using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMThreads
{
    interface ZhiRenCallBack
    {
        void onSuccess();

        void onError(String message);
    }
}
