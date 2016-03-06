using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    abstract class Agent : IRunnable
    {

        abstract public void Update();

        void IRunnable.Run()
        {

            while (HasFinished != true)
            {

                Thread.Sleep(100);
                this.Update();

            }

        }

        public abstract IEnumerator<float> CoroutineUpdate();

        public bool HasFinished;

    }
}
