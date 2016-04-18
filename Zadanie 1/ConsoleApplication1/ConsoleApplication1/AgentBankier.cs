using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

   
    class AgentBankier : Agent
    {

        public int kasaBanku;
        public static Mutex mut = new Mutex();
        public Object thisLock = new Object();
        public SpinLock sl = new SpinLock();


        public override void Update()
        {
            if (Licznik < 120)
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
