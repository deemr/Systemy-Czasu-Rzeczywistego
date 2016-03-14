using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentSumSum : Agent
    {

        public int wynikSumSum;
        public List<Agent> ListaAgentow = new  List<Agent>();
        public bool CheckFinish;

        public override void Update()
        {
            CheckFinish = ListaAgentow.All(a => a.HasFinished);
            if (CheckFinish)
            {
                foreach (Agent agentElement in ListaAgentow)
                {
                    wynikSumSum = wynikSumSum + agentElement.Suma;           
                }
                Console.WriteLine("Suma wszystkich to: " + wynikSumSum);
                this.HasFinished = true;
            }

        }
        public AgentSumSum(List<Agent> ListaAgentow)
        {
            this.ListaAgentow = ListaAgentow;

        }
    }
}
