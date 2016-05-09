using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class BusyWaitStrategy : IWaitStrategy
    {
        private readonly int iterations;

        public BusyWaitStrategy(int iterations)
        {
            this.iterations = iterations;
        }

        //Otherwise, the whole body will be pruned in release mode.
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Wait()
        {
            for (int i = 0; i < iterations; ++i)
            {
            }
        }
    }

    public class SleepWaitStrategy : IWaitStrategy
    {
        private readonly int ms;

        public SleepWaitStrategy(int ms)
        {
            this.ms = ms;
        }

        public void Wait()
        {
            Thread.Sleep(ms);
        }
    }

    public class SpinWaitStrategy : IWaitStrategy
    {
        //We utilize efficient builtin waiter to do our job. This is a waiting object, no a sync primitive. Do not confuse this with spinlock.       
        private readonly SpinWait spinwait = new SpinWait();

        public SpinWaitStrategy()
        {
        }

        public void Wait()
        {
            //Let's wait as long as possible without incurring context switching.
            while (!spinwait.NextSpinWillYield)
                spinwait.SpinOnce();
        }
    }

    public class NoWaitStrategy : IWaitStrategy
    {
        public void Wait()
        {
        }
    }
}
