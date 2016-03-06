using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentKonkretny : Agent
    {

        private int agentId;
        private int Licznik;

        public override void Update()
        {


            Console.WriteLine("Agent o id: " + agentId + ", wartosc licznika: " + Licznik);
            Licznik++;
            this.HasFinished = true;


        }

        public override IEnumerator<float> CoroutineUpdate()
        {
            throw new NotImplementedException();
        }

        public AgentKonkretny(int agentId)
        {
            Licznik = 0;
            this.agentId = agentId;

        }

    }
}
