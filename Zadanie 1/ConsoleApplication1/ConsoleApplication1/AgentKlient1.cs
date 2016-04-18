using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

   
    class AgentKlient1 : Agent
    {

        AgentBankier bank;
        int StanKonta;

        public override void Update()
        {
            bool gotLock = false;
            if (Licznik < 100)
            {
                gotLock = false;
                try
                {
                    //lock (bank.thisLock)
                    //{
                    bank.sl.Enter(ref gotLock);
                    bank.kasaBanku = bank.kasaBanku - 1;
                //}
                //AgentBankier.mut.WaitOne();
                }
                finally
                {
                    bank.sl.Exit();
                    //  AgentBankier.mut.ReleaseMutex();
                }
                Licznik++;
            }
            else
            {
                this.HasFinished = true;
            }
        }


        public AgentKlient1(AgentBankier bankier)
        {

            bank = bankier;

        }


    }
}
