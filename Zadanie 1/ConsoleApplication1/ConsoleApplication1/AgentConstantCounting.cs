using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentConstantCounting : Agent
    { 
        public override bool HasFinished{
            set;
            get;
        }
        public override void Update(){
            if (Licznik < 10) Licznik++;
            else {
                Console.WriteLine("Agent o id: " + agentId);
                this.HasFinished = true;
            }
        }
        public AgentConstantCounting(int agentId){
            Licznik = 0;
            this.agentId = agentId;
        }
    }
}
