using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    abstract class Agent : IRunnable
    {
        public abstract bool HasFinished {
            get;
            set;
        }
        abstract public void Update();

        void IRunnable.Run(){
            while (!HasFinished){
                Thread.Sleep(100);
                this.Update();
            }
        }

        public abstract IEnumerator<float> CoroutineUpdate();

    }
}
