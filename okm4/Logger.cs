using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okm4
{
    class Logger
    {
        static object locker = new object();
        public static void Log(string message)
        {
            lock (locker)
            {
                try
                {
                    string log = DateTime.Now.ToLongDateString();
                    log += " " + message;
                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(@"logging.txt", true))
                    {
                        file.WriteLine(log);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
