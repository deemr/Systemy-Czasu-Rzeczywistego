using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

   
    class AgentBankier : Agent
    {

        int kasaBanku;

        public override void Update()
        {
            if (Licznik < 10)
            {
                Console.WriteLine(kasaBanku);
                Licznik++;
            } else
            {
               this.HasFinished = true;
            }
        }


        public AgentBankier(int Kasiora)
        {
            kasaBanku = Kasiora;

        }


    }
}
