using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentSine : Agent{
        
        private double sineValue;
        private double Output {

            get {
                return sineValue;
            }
            set { //set działa jak argument metody
                sineValue = value;
            }
            dsfs
        }
        public override void Update()
        {
            if (agentId%10 !=0 ){
                Licznik++;   
            }
            else {
                sineValue = Math.Sin(Convert.ToDouble(Licznik));
                Console.WriteLine("Output: " + sineValue);
                Console.WriteLine("Agent Sine o id: " + agentId);
                this.HasFinished = true;
            }
        }
        public AgentSine(int agentId)
        {
            Licznik = 0;
            this.agentId = agentId;
        }

    }
}
