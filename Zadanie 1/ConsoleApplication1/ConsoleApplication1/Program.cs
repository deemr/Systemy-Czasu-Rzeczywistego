using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ConsoleApplication1
{

    class Program{
        static void Main(string[] args){

            IRunnable agent = new AgentKonkretny(0);
            IRunnable agent1 = new AgentKonkretny(1);

            Console.ReadLine();
        }
    }
}
