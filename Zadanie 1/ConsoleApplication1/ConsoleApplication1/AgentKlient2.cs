using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

   
    class AgentKlient2 : Agent
    {

        int StanKonta;

        public override void Update()
        {
            if (Licznik < 10)
            {
                StanKonta--;
                Console.WriteLine(StanKonta);
                Licznik++;
            }
            else
            {
                this.HasFinished = true;
            }
        }


        public AgentKlient2(int Kasiora, Agent bank)
        {
            //trzeba wrzucic do kazdego klienta referencje do banku, zeby moc operowac na wspolnym hajsie
            // bank.Kasiora;
        }


    }
}
