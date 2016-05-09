using System;
using System.Collections.Generic;
using System.Threading;

namespace RTLabExample1
{
    //While we could stick it all in a single class, let us introduce some proper basic OOP design for educational purposes and simplificataion of further examples.
    //IRunnable is an interface exposed to our thread/process/fiber management code which actually decides which, when and where agents should be running.
    //Agent is an abstract class encapsulating the most common implementation pattern of IRunnable
    //Specific agents derive from Agent and implement only Update. In more complex cases, they can implement IRunnable from scratch to achieve specific control flow.
    public abstract class Agent : IRunnable
    {
        //This is not wall-clock time, it is just a monotonic counter. Observe how we make it protected to expose it only to concrete Agent implementations
        protected float virtualTimeS = 0.0f;

        //A readonly field can be only assigned during construction. Perfect for object-invariant (i.e. constant during object lifetime) private data.
        protected readonly float timeStep;

        //Agent Id used for bookkeeping. 
        //Notice how it is defined as an auto-property with private setter to protect Id from outside manipulation without additional boilerplate code.
        public int Id { get; private set; }

        public Agent(int id, float timeStep = 0.1f)
        {
            //Observe how one can utilize the same variable identifier thanks to lexical scoping of C# if "this" is used.
            this.timeStep = timeStep;
            Id = id;
        }

        public void Finish() { HasFinished = true; }

        //IRunnable implementation
        public bool HasFinished { get; private set; } = false;

        //We block a thread/process (depend on how run was called) and yield cpu time by sleeping. 
        //This depends on OS context switching and scheduler. Every switch is an OS call and full process/thread stack restoration.
        //Every process addtionally gets a GC heap and memory pool of approx 1 MB on a 32-bit machine.
        public void Run()
        {
            while (!HasFinished)
            {                
                Update();
                virtualTimeS += timeStep;
                Thread.Sleep((int)Math.Round(timeStep * 1000.0f));
            }
        }

        //Here on the other hand, we exploit C# iterators to achieve fibers residing fully in userspace. 
        //No full stack restoration happens, natural control flow is still perimitted. No memory overhead.
        //Agents are executed serially and concurrency happens by voluntary preemption.
        //Useful for massive scale agent simulations, high performance networking code, complex business logic orchestration, etc.
        //Remark: OS controlled fibers can only be achieved from native Windows API called by P/Invoke in C#, which is way too complex for this course.
        //They behave in a very similar manner.
        public IEnumerator<float> CoroutineUpdate()
        {
            while (!HasFinished)
            {
                Update();
                virtualTimeS += timeStep;
                if (HasFinished)
                    yield break;
                else
                    yield return virtualTimeS;
            }
        }

        public abstract void Update();
    }
}