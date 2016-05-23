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
        public ConcurrentQueue<Action<WorkstealingAgent>> Queue;


        public override void Update()
        {

            Action<WorkstealingAgent> result;
            
            while (true) {

                Queue.TryDequeue(out result);

            }
    
        }

        public WorkstealingAgent(int id) : base(id) { }

    }
}
