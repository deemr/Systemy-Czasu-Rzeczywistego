using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    class WorkstealingAgent : Agent
    {

        public ConcurrentQueue<Action<WorkstealingAgent>> Queue = new ConcurrentQueue<Action<WorkstealingAgent>>();

        public List<IRunnable> agentList;

        public override void Update()
        {

            Action<WorkstealingAgent> result;

            bool isEmptyQueue = true;
       
            while (isEmptyQueue) {


                if (Queue.TryDequeue(out result)) result(this);
                if (this.Queue.IsEmpty) { 
                    foreach (WorkstealingAgent agent in agentList) {
                        //jezeli ma co sciagnac i nie bedzie probowal sciagnac z siebie samego to ma cos zrobic
                        if(agent.Queue.TryDequeue(out result) && agent != this) result(this);               

                    } 
                } 
            }
        }

        public WorkstealingAgent(int id, List<IRunnable> agentList) : base(id) {

            this.agentList = agentList;

        }

    }
}
