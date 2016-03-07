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
        public static List<Agent> GenerateRunnables()
        {

            List<Agent> AgentsList = new List<Agent>();
            for (int i = 0; i < 10; i++){

                AgentsList.Add(new AgentConstantCounting(i));
                AgentsList.Add(new AgentCounting(i));
                AgentsList.Add(new AgentSine(i));

            }

            return AgentsList;
        }

        public static void RunThreads(List<Agent> AgentsList) {
            int a = 0;
            Thread[] threadList = new Thread[AgentsList.Count];
            foreach (Agent agentElement in AgentsList) {

                threadList[a] = new Thread(new ThreadStart(agentElement.Run));
                threadList[a].Start();
                a++;
            }
            foreach (Thread thread in threadList) {
                thread.Join();//blokuje wątek zanim zostanie zakończony
                thread.Abort();
            }
        }

        public static void RunFibers(IEnumerable<Agent> AgentsList) {

            var enumerators = AgentsList.Select(ag => ag.CoroutineUpdate());

            bool check;

            while (check=true) {
                foreach (var e in enumerators) {
                    e.MoveNext();
                    check = true;
                }
                Thread.Sleep(100);
            }
            
    
        }

        static void Main(string[] args){

            List<Agent> List = new List<Agent>(GenerateRunnables());

            RunFibers(List); 
            Console.WriteLine("a teraz ta druga metodka");
            RunThreads(List);

            Console.ReadLine();
        }
    }
}
