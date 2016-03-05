using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ConsoleApplication1
{

    abstract class Agent : Interface1
    {

        abstract public void Update();

        IEnumerator Interface1.CoroutineUpdate()
        {
            throw new NotImplementedException();
        }

        void Interface1.Run()
        {

            while (HasFinished != true)
            {

                Thread.Sleep(100);
                this.Update();

            }

        }

        public bool HasFinished;

    }

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

        public AgentKonkretny(int agentId)
        {
            Licznik = 0;
            this.agentId = agentId;

        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            Interface1 agent = new AgentKonkretny(0);
            Interface1 agent1 = new AgentKonkretny(1);

            agent.Run();
            agent1.Run();

            Console.ReadLine();
        }
    }
}
