using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoencoderVisualize
{
    public class Job
    {
        // 0 idle , 1 loop , 2 done;
        public int state;
        public String name;

        public delegate void CallBack();
        public CallBack cb;

        public Job(string n)
        {
            state = 0;
            name = n;
            cb = null;
        }
    }
}
