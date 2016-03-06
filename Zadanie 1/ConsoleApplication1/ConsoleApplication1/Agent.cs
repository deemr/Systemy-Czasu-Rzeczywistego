using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public abstract class Agent : IRunnable
    {

        public int agentId;
        public int Licznik;
        private float numerator;

        public abstract bool HasFinished {
            get;
            set;
        }
        abstract public void Update();

        public void Run(){
            while (!HasFinished){
                Thread.Sleep(100);
                this.Update();
            }
        }

        public IEnumerator<float> CoroutineUpdate() {
            while (!HasFinished) {
                this.Update();
                yield return numerator;
            }   yield break;
        }

    }
}
