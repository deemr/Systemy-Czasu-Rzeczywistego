using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTLabExample1
{

    public class DatabaseSyncStrategy<double> : ISyncStrategy<double>
    {
        //We declare an empty continuation to be safe when user breaks the protocol and does not call SetOpContinuation.
        //One could eliminate the need for this by using constraints on generics and forcing synchronizer construction in the bank, but
        //this would be too contrived for students.
        protected Action<double> continuation = (v) => { };

        protected Func<double> reader = () => default(double);

        //This can be empty - so it is not abstract;
        public virtual void ProcessRequests()
        {
        }

        //This is not supposed to be overriden. Ask Your students: What is the difference between overriding members with "new" and "override"?
        public void SetOperationContinuation(Func<double> reader, Action<double> cont)
        {
            this.continuation = cont;
            this.reader = reader;
        }

        //This is mandatory.
        public virtual void Synchronize(int id, Func<T, T> operation)
        {
            try
            {
                using (var file = new System.IO.StreamWriter(@"C:\Users\ksis\Source\Repos\Systemy-Czasu-Rzeczywistego\Zadanie 1\ConsoleApplication1\BANK\RTLabExample1\WriteLines.txt", true))
                {
                    var value = this.reader();
                    var diff = operation()
                
                    file.WriteLine(operation(reader()).ToString());
                    file.Flush();  
                }
              
            }
            catch (Exception e)
            {
                // your message here.
            }
        }
    }

    //Client should not be able to modify the balance directly, because that could bypass synchronization.
    //On the other hand, modification of account balance is fragile and intrinsic to synchronization, so synchronizer/sync strategy 
    //should be able to control how that happens.
    //We could pass around additional objects, break encapsulation by giving the clients arbitrary access to the account, or we could naturally
    //pass an additional operation of changing account balance around. It is done in the same way as abstraction of synchronized operations in 
    //"Synchronize". This is called "Inversion of Control (IOC)" pattern.
    //This naturally and practically introduces a functional style. One could take it further and use higher-order functions and currying to 
    //eliminate all other interfaces in the projects utilizing the same patterns (which we don't do to retain students sanity).
    public abstract class SyncStrategy<T> : ISyncStrategy<T>
    {
        //We declare an empty continuation to be safe when user breaks the protocol and does not call SetOpContinuation.
        //One could eliminate the need for this by using constraints on generics and forcing synchronizer construction in the bank, but
        //this would be too contrived for students.
        protected Action<T> continuation = (v) => {};

        protected Func<T> reader = () => default(T);

        //This can be empty - so it is not abstract;
        public virtual void ProcessRequests()
        {
        }

        //This is not supposed to be overriden. Ask Your students: What is the difference between overriding members with "new" and "override"?
        public void SetOperationContinuation(Func<T> reader, Action<T> cont)
        {
            this.continuation = cont;
            this.reader = reader;
        }

        //This is mandatory.
        public abstract void Synchronize(int id, Func<T, T> operation);
    }


    public class NoSyncStrategy<T> : SyncStrategy<T>
    {
        public override void Synchronize(int id, Func<T, T> operation)
        {
            //The simplest application - no synchronization. Observe how we plug the operation into the continuation to make
            //the final operation. 
            continuation(operation(reader()));
            //We could capture this "composed operation" explicitly by function composition: 
            //Action<double> composed = (v) => continuation(operation(v));
            //In this manner one can construct arbitrary synchronized operations without knowledge about other parties constructing the operation and without evaluation
            //of any arguments. 
            //At this point it is clear that one can write functionally in C#, yet the syntax is a bit clumsier than in e.g. Haskell or even F#.
        }
    }

    //This is the easiest and most used strategy. Always start with this. Under the hood it actually uses a combination of spinwait and a mutex.
    public class LockSyncStrategy<T> : SyncStrategy<T>
    {
        //This is a so called token, i.e. an entity used to differentiate between locked resources.
        //The locked resource is often not considered to be a token by itself, since it must be a reference type, but it can be any object.
        //The rationale behind forcing reference types is simple - copy semantics.
        Object obj = new Object();
        public override void Synchronize(int id, Func<T, T> operation)
        {
            lock(obj)
            {
                continuation(operation(reader()));
            }
        }
    }

    public class MutexSyncStrategy<T> : SyncStrategy<T>
    {
        //Some students will erroneously try to aquire a system-wide mutex by using examples with creating named mutexes. 
        //This is not needed in our examples and can lead to problems (name clashes) in general.
        private Mutex mutex = new Mutex(); 

        public override void Synchronize(int id, Func<T, T> operation)
        {
            //We do not use WaitOne timeout mechanism.
            //Also, we must use try-finally clauses to account for cases when WaitOne or synchronized operations throw exceptions.
            //In such cases the mutex must be still released, but we must be careful to not release it twice (see if in finally).
            //This pattern is visible in most of synchronization protocols.
            //It is now visible why lock statement is easier and safer to use in general cases.
            bool locked = false;
            try
            {
                locked = mutex.WaitOne();
                continuation(operation(reader()));
            }
            finally
            {
                if(locked)
                    mutex.ReleaseMutex();
            }
            
        }

        ~MutexSyncStrategy()
        {
            mutex.Dispose();
        }
    }

    public class SpinlockSyncStrategy<T> : SyncStrategy<T>
    {
        SpinLock spinlock = new SpinLock();

        public override void Synchronize(int id, Func<T, T> operation)
        {
            bool locked = false;
            try
            {
                //This blocks.
                while(!locked)
                    spinlock.Enter(ref locked);
                continuation(operation(reader()));
            }
            finally
            {
                if (locked)
                {
                    //Investigate performance hit of setting true/false here. Discuss after introducing memory barriers.
                    spinlock.Exit(true);
                }
            }
        }
    }

    public class SpinlocklessSyncStrategy<T> : SyncStrategy<T>
    {
        SpinLock spinlock = new SpinLock();

        private readonly IWaitStrategy waiter;

        //We don't couple ourselves to a waiting behavior - we just show a pattern in terms of abstractions that were already introduced.
        public SpinlocklessSyncStrategy(IWaitStrategy waiter)
        {
            this.waiter = waiter;
        }

        public override void Synchronize(int id, Func<T, T> operation)
        {
            bool locked = false;
            try
            {
                //This does not block, so we need a hand-rolled retry mechanism, which is good - application can decide how to handle high load scenarios.
                while (!locked)
                {
                    spinlock.TryEnter(ref locked);
                    if (locked)
                    {
                        continuation(operation(reader()));
                        break;
                    }
                    waiter.Wait();
                }                                
            }
            finally
            {
                if (locked)
                {
                    //Investigate performance hit of setting true/false here. Discuss after introducing memory barriers.
                    spinlock.Exit(true);
                }
            }
        }
    }

    //Now we utilize ProcessRequests and cache account balance in the synchronizer.
    //Otherwise, one would need explicit access to balance AND forced access patterns at bank side which is bad.
    //This example also showcases how one could add arbitrary synchronization to a legacy system without any modifications in business logic,
    //if this system was designed properly.
    //Notice that genericity is now abandoned at the implement atomic operations, but interface remains generic and type safe at the same time.
    public class AtomicSyncStrategy : SyncStrategy<double>
    {
        double relativeValue = 0.0;

        //Remember that this strategy becomes pointless when thread sleeping is applied between retries. 
        //This is intented for high performance scenarios where one cannot afford context switching. 
        //Thus we omit IWaitStrategy and go straight for indefinite busy waiting. 
        //This is guaranteed to converge under the assumption of bounded number of client operations.
        public AtomicSyncStrategy()
        {
        }

        //Notice how our architecture lets us fool the synchronized operation into believing that it modifies the real balance,
        //while in reality it only modifies our temporary balance.
        //This is one of the benefits of clean separation of components. 
        //Ability to guarantee such behavior is essential for scalability, testability, and so on. It hinges on eleminating any hidden effects of 
        //code execution introduced by global shared state or non-idempotent operations (where it matters).
        //This is (in principle) mocking! No frameworks (like e.g. Moq) are needed and no complicated mechanisms are involved. 
        //Mocking is natural if the code is written properly to account for it.
        public override void Synchronize(int id, Func<double, double> operation)
        {
            //We show students how to use compareexchange (the so called CAS operation), because it is the hardest atomic primitive.
            //One could use Interlocked.Add in this case, but it is trivial after understanding this example.
            double oldValue = 0.0;
            do
            {
                //It is crucial that comparand for CompareExchange lives in its own local variable!
                //Keep CompareExchange calls, assignment of local variables and actual computation as close as possible to maximize success rate.
                oldValue = relativeValue;
                //operation(oldValue)
            } while (oldValue != Interlocked.CompareExchange(ref relativeValue, oldValue + 100, oldValue));
        }

        public override void ProcessRequests()
        {
            //No sync. here, because we just read the value. We can let it converge.
            this.continuation(relativeValue);
        }
    }

    public class QueueSyncStrategy<T> : SyncStrategy<T>
    {
        //Functions are first class objects while encapsulated in Func, thus we can queue them just like messages.
        private readonly ConcurrentQueue<Func<T, T>> queue = new ConcurrentQueue<Func<T, T>>();

        public override void Synchronize(int id, Func<T, T> operation)
        {
            //This is thread-safe and lockless by design of concurrent queue built into .NET. 
            //Actual design of such a queue should be deferred to later classes if shown at all.
            queue.Enqueue(operation);
        }

        public override void ProcessRequests()
        {
            //No sync. here - this is executed serially by the bank.
            //We assume that clients will work for bounded time and end before the bank.
            //In such case one can always dequeue until the first failure, because final iterations will have zero failures apart from exhaustion of queue.
            //This policy easily favors clients (which is frequent in real life). One can attempt to reverse this priority by changing concurrent container and
            //dequeueing strategy. Priority with mutex/spinlock/atomics is much harder. 
            //This is also more tollerant to load spikes and guarantees constant client handling time.
            Func<T, T> op;
            while (queue.TryDequeue(out op))
                continuation(op(reader()));
        }
    }

    //We show a dictionary example with just one key/value, i.e. one bank account to exploit assumed abstractions.
    //Students can (and should!) easily roll their own "MultiUserBankAgent" and additional clients to handle multiple accounts 
    //using code similiar to the one given below. Sync. in that case is the same as below.
    //Key equal to 0 is arbitrarily assumed here just for demonstration.
    public class DictionarySyncStrategy<T> : SyncStrategy<T>
    {
        private readonly ConcurrentDictionary<int, T> dictionary = new ConcurrentDictionary<int, T>();

        public override void Synchronize(int id, Func<T, T> operation)
        {
            //If there is no value, there were no modifications yet, so default(T) is appropriate and no further action is needed. 
            //Observe how close this is to atomics. The difference is that it works with arbitrary types and keys by using GetHashCode internally.
            //Internal locking is done using CompareExchange          
            var value = default(T);
            do
            {
                value = dictionary.GetOrAdd(0, k => default(T));
            }
            while (!dictionary.TryUpdate(0, operation(value), value));

            //Always use the above! Beware of doing this:
            //dictionary.AddOrUpdate(0, k => default(T), (k, v) => operation(v));
           //The last argument is executed internally outside of a critical section, so this will not work.
           //Discuss with students to demonstrate how fragile sync. can be.
        }

        public override void ProcessRequests()
        {

            //Once again, if there is no value, there were no modifications yet, so default(T) is appropriate and no further action is needed.
            var value = dictionary.GetOrAdd(0, k => default(T));
            continuation(value);
        }
    }

    //This is "piekarniany" from the description. Check literature for details.
    //No atomic operations required here, but we must take care of cache/memory access ordering to avoid cache pollution errors.
    //Max number of clients is constant for simplicity. Do not use lists - their resizing behavior is dangerous here.
    //This implementation full barriers, i.e. volatile (which corresponds to points 9 and 10 - point 9 will not work without barriers by design!).
    //Implementation with separate read/write barriers corresponds to 11.
    //Transaction logs are going to be implemented separately during next classes.
    public class LamportSyncStrategy<T> : SyncStrategy<T>
    {
        private volatile bool[] entering = new bool[20];
        private volatile int[] number = new int[20];

        public LamportSyncStrategy()
        {
            for(int i =0; i < entering.Count(); ++i)
            {
                entering[i] = false;
                number[i] = 0;
            }
        }

        private void Lock(int i)
        {
            entering[i] = true;
            number[i] =  1 + number.Max();            
            entering[i] = false;
            for (int j = 0; j < entering.Count(); j++)
            {
                while (entering[j]) {}
                while ((number[j] != 0) && (number[j] < number[i] || (number[j] == number[i] && j < i))) { }
            }
        }
   
       private void Unlock(int i)
       {
            number[i] = 0;
        }


       public override void Synchronize(int id, Func<T, T> operation)
       {
            Lock(id);
            continuation(operation(reader()));
            Unlock(id);
       }
    }

    //This is technically also a "full" barrier, but enforced only in a single place as opposed to all the accesses as in volatile.
    //The precise implementation of the described "read/write" fences is too tedious in my opinion.
    //Also, precise memory semantics of C# and C differ a bit. We tend towards showing how it should behave with C, since C# is safer.
    public class LamportFineSyncStrategy<T> : SyncStrategy<T>
    {
        private bool[] entering = new bool[20];
        private int[] number = new int[20];

        public LamportFineSyncStrategy()
        {
            for (int i = 0; i < entering.Count(); ++i)
            {
                entering[i] = false;
                number[i] = 0;
            }
        }

        private void Lock(int i)
        {
            entering[i] = true;
            number[i] = 1 + number.Max();
            entering[i] = false;
            for (int j = 0; j < entering.Count(); j++)
            {
                while (entering[j]) {}
                while ((number[j] != 0) && (number[j] < number[i] || (number[j] == number[i] && j < i))) { Thread.MemoryBarrier();  }
            }
        }

        private void Unlock(int i)
        {
            number[i] = 0;
        }


        public override void Synchronize(int id, Func<T, T> operation)
        {
            Lock(id);
            continuation(operation(reader()));
            Unlock(id);
        }
    }
}
