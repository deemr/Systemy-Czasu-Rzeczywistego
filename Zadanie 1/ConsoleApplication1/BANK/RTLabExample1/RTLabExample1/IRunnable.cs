using System;
using System.Collections.Generic;

namespace RTLabExample1
{
    //The I in the name is customary in purely OOP languages to separate base class definitions from interface definitons.
    //Some communities actually use an active-voice-like naming convention, i.e. ICanRun or IAmRunnable.
    interface IRunnable
    {
        void Run();

        //Return a sequence of times at which the runable agent should be called again.
        IEnumerator<float> CoroutineUpdate();

        bool HasFinished { get; }
    }
}