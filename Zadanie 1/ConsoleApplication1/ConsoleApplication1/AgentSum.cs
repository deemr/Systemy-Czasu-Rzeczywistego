using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentSum : Agent
    {
        List<int> ListaLiczb = new List<int>();

        public int wynik;

        public override void Update()
        {
            if (Licznik < Dodawaj) {            
                Licznik++;
                Suma++;
            } else {
                Console.WriteLine("Agent Sum o id: " + agentId + " - suma " + Suma );
                this.HasFinished = true;
                wynik = Suma;
            }
        }
        public AgentSum(int agentId, int Dodawaj)
        {
            this.Dodawaj = Dodawaj;
            Licznik = 0;
            Suma = 0;
            this.agentId = agentId;

        }

      
    }
}
