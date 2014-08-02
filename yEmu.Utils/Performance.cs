using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yEmu.Util
{
    class Performance
    {
        public static long GCTotal()
        {
           return GC.GetTotalMemory(true);
        }
        public static double BytesToMegabytes(long bytes)
        {
            return bytes / 1024d / 1024d;
        }
        public static int cThread()
        {
            return Process.GetCurrentProcess().Threads.Count;
        }
   
        public static string CurrentCPUusage()
        {
            
                PerformanceCounter cpuCounter;
                cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                return cpuCounter.NextValue() + "%";

        }




    }
}
