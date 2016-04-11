using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

   
    class AgentKlient1 : Agent
    {


        int StanKonta;

        public override void Update()
        {
            if (Licznik < 10)
            {
                StanKonta = StanKonta + 2;
                Console.WriteLine(StanKonta);
                Licznik++;
            }
            else
            {
                this.HasFinished = true;
            }
        }


        public AgentKlient1(int Kasiora)
        {

            StanKonta = Kasiora;

        }


    }
}
