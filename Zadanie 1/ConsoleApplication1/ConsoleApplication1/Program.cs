using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{

    class Program{
        public static List<Agent> GenerateRunnables()
        {

            List<Agent> AgentsList = new List<Agent>();
            //for (int i = 0; i < 10; i++){

            //AgentsList.Add(new AgentConstantCounting(i));
            //AgentsList.Add(new AgentCounting(i));
            //AgentsList.Add(new AgentSine(i));


            //}
            //List<int> ListaLiczb = new List<int>(GenerateRandoms());
            /*for (int i = 0; i < ListaLiczb.Count; i++)
            {

                if (i == 249) AgentsList.Add(new AgentSum(0, 250));
                if (i == 499) AgentsList.Add(new AgentSum(1, 250));
                if (i == 749) AgentsList.Add(new AgentSum(2, 250));
                if (i == 999) AgentsList.Add(new AgentSum(3, 250));
               
            }*/

            /*string [] Stringi = GenerateText().Split(new Char [] {' ', '.', ',', '\t', '\n' });
            string kleks = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j< Stringi.Length/2; j++)
                {
                    if (Stringi[j].Trim() != "")
                    {
                        
                        kleks += Stringi[j] + " ";
                    }
                }
                AgentsList.Add(new AgentText(i + 1,kleks));
                kleks = null;
            }*/

            int Kasiora = 0;

            AgentsList.Add(new AgentBankier(Kasiora));
            AgentsList.Add(new AgentKlient1());
            AgentsList.Add(new AgentKlient2());
            
            //AgentsList.Add(new AgentBankier());


            return AgentsList;
        }

        public static List<int> GenerateRandoms()
        {

            List<int> IntList = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                IntList.Add(1);
            }

            return IntList;
        }
    
        private static String GenerateText()
        {
            return System.IO.File.ReadAllText(@"C:\Users\ksis\Downloads\Systemy-Czasu-Rzeczywistego-master (1)\Systemy-Czasu-Rzeczywistego-master\Zadanie 1\ConsoleApplication1\tekst.txt");
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
                
            }
        }

      

        public static void RunFibers(IEnumerable<Agent> AgentsList) {

            var enumerators = AgentsList.Select(ag => ag.CoroutineUpdate());

            bool check=true;

            while (check) {
                check = false;
                foreach (var e in enumerators) {
                    if (e.MoveNext())
                        check = true;
                }
                Thread.Sleep(100);
            }
            
    
        }

        static void Main(string[] args){

            //List<Agent> List = new List<Agent>(GenerateRunnables());

            List<Agent> List = new List<Agent>(GenerateRunnables());

            //RunFibers(List); 
            //Console.WriteLine("a teraz ta druga metodka");
            RunThreads(List);
            

            //1000 liczb, w liscie,
            //robimy agenta, ktory sumuje podliste



            Console.ReadLine();
        }

        
    }
}
