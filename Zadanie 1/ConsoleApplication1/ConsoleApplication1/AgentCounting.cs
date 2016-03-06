using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentCounting : Agent{
       
        public override void Update()
        {
            if (Licznik < agentId) Licznik++;
            else {
                Console.WriteLine("Agent Counting o id: " + agentId);
                this.HasFinished = true;
            }
        }
        public AgentCounting(int agentId)
        {
            Licznik = 0;
            this.agentId = agentId;
        }

    }
}
