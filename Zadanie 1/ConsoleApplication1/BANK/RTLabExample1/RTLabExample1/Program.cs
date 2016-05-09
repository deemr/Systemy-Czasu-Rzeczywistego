using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTLabExample1
{
    class Program
    {
        //We use our runnable abstraction here.
        static void RunThreads(IEnumerable<IRunnable> runnables)
        {
            //We keep threads in a list or otherwise the are free to be GC'ed (garbage collected) right after the loop iteration finishes.
            //var is like auto in C++11, i.e. the type is inferred from the right-hand side expression. Note that the most specific types 
            //(i.e. deepest types in respective inheritance hierarchy tree) are always inferred.
            var threads = new List<Thread>(runnables.Count());

            foreach (var runnable in runnables)
            {
                var t = new Thread(runnable.Run);
                threads.Add(t);
                t.Start();
            }

            //Let us now wait for finishing of all the agents;            
            bool allFinished = false;
            while (!allFinished)
            {
                Thread.Sleep(100);

                //We can be boring C-like programmers and loop by hand.
                /*allFinished = true;
                foreach (var runnable in runnables)
                {
                    if (!runnable.HasFinished)
                    {
                        allFinished = false;
                        break;
                    }
                }*/

                //Or we can be more brief and declarative by utilizing lambdas and LINQ combinators.
                allFinished = !runnables.Any(r => !r.HasFinished);
            }
        }

        static void RunFibers(IEnumerable<IRunnable> runnables)
        {
            //For now we will run all fibers with a fixed time step determined by the last one.
            var timeStep = 0.0f;

            //Select maps a list to another list (just like SQL select statement, map cobinator, mathematical one-to-one map, etc.)
            var enumerators = runnables.Select(r => r.CoroutineUpdate());

            bool allFinished = false;
            while (!allFinished)
            {
                foreach (var enumerator in enumerators)
                {
                    //Perform one step of every agent. Agents preempt voluntarily when they call yields.
                    //That is why we can code them like they have a dedicated thread and then run them in a single OS thread.
                    if (enumerator.MoveNext())
                    {
                        timeStep = enumerator.Current;
                    }
                }

                allFinished = !runnables.Any(r => !r.HasFinished);
                //Whereas with threads we could sleep as long as we want, here it is crucial to sleep just for a time step.
                Thread.Sleep(100);
            }
           
        }

        static List<IRunnable> GenerateRunnables()
        {
            //This is basically an analog of std::vector from C++. 
            var runnables = new List<IRunnable>(1000);
            int id = 0;
            for (; id < 10; ++id)
                runnables.Add(new CountingAgent(id));
            int limit = runnables.Count() + 10;
            for (; id < limit; ++id)
                runnables.Add(new SineGeneratingAgent(id));

            //What happens if we use a constant counter and by consequence force simultaneous console writes (see for 100 or more such agents)?
            //Even though we explicitly started our threads in a sequence they will not finish in the same sequence - the essence of concurrency problems.
            limit = runnables.Count() + 10;
            for (; id < limit; ++id)
                runnables.Add(new ConstantCountingAgent(id));
            return runnables;
        }

        static void RunEx1()
        {
            var runnables = GenerateRunnables();
            Console.WriteLine("Threads started.");
            RunThreads(runnables);
            Console.WriteLine("Threads finished.");

            //Let us perform the same test with fibers/coroutines. Runnables are regenerated to clear their state.
            runnables = GenerateRunnables();
            Console.WriteLine("Fibers started.");
            RunFibers(runnables);
            Console.WriteLine("Fibers finished.");
        }

        //Running a bank is actually tedious with an exception of synchronization and waiting strategy, thus we lambda-abstract.
        static void RunBank(Func<ISyncStrategy<double>> syncStratGen, Func<IWaitStrategy> waiterGen)
        {
            var runnables = new List<IRunnable>();
            var updates = 10;

            //We give additional 10 iterations to finish handling all clients. 
            //Assume that synchronizations failing to satisfy this constraint are too slow. This can be changed to investigate performance.
            var bank = new BankAgent(-1, syncStratGen(), updates + 10);
            runnables.Add(bank);
            for (int i = 0; i < 10; ++i)
            {
                //Last arguments: 100, 10 normally (default) and 10, 2 for Lamport (slow convergence).
                runnables.Add(new BankClientAgent(i,150, bank, waiterGen(), true, 20, 4));
                runnables.Add(new BankClientAgent(10 + i, 50, bank, waiterGen(), true, 20, 4));
            }
            RunThreads(runnables);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Program started.");

            //This obviously will not converge to desired balance. But let the students check for themselves!
            //RunBank(() => new NoSyncStrategy<double>(), () => new NoWaitStrategy());

            //Those should all converge, but with different performance/safety/complexity tradeoffs.
            //The excercise of changing/verifying other wait strategies is left to the reader/student.
            RunBank(() => new DatabaseSyncStrategy<double>(), () => new NoWaitStrategy());
            //RunBank(() => new LockSyncStrategy<double>(), () => new NoWaitStrategy());
            //RunBank(() => new SpinlockSyncStrategy<double>(), () => new NoWaitStrategy());
            //Notice how different sleep ratios here will uncover seemingly chaotic, yet correct behavior 
            //unless sleeping takes too much time and bank will shut down.
            //RunBank(() => new SpinlocklessSyncStrategy<double>(new SleepWaitStrategy(10)), () => new NoWaitStrategy());
            //RunBank(() => new AtomicSyncStrategy(), () => new NoWaitStrategy());
            //RunBank(() => new QueueSyncStrategy<double>(), () => new NoWaitStrategy());
            //RunBank(() => new DictionarySyncStrategy<double>(), () => new NoWaitStrategy());

            //Since Lamport converges slowly one must change the params a bit to decrease wait time (see above).
            //Run Lamport in release mode to see memory ordering problems en masse. Otherwise, memory fences will be inserted by VM during debugging.
            //This is a HUGE CONCLUSION for students. Some sync. protocols will fail without warnings if not tested properly in production environment.
            //RunBank(() => new LamportSyncStrategy<double>(), () => new NoWaitStrategy());
            //RunBank(() => new LamportFineSyncStrategy<double>(), () => new NoWaitStrategy());


            Console.WriteLine("Program finished.");
            Console.ReadKey();
        }
    }
}
