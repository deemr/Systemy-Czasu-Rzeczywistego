using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTLabExample1
{
    class WorkstealingAgent : Agent
    {

        public ConcurrentQueue<Action<WorkstealingAgent>> Queue = new ConcurrentQueue<Action<WorkstealingAgent>>();
        private int currentResult, upperResult; //upper jest to wynik z poprzedniego(wyzszego levelu, poprzedniego kroku) "okrążenia drzewa", a current z obecnego kroku, w ktorym akurat sie znajduje algorytm
        public List<IRunnable> agentList;


        public void SumTree(TreeNode tree)
        {
            //sprawdzenie
            //Interlocked dlatego, zeby została zachowana atomowość operacji. zeby nic jej nie zaklocilo
            //CompareExchange porównuje wartosc currentResult z tree.node i jezeli jest równe to
            //zamienia wartosc currentResult na wartość tree.node, to porownywanie jest po to, zeby algorytm
            //mogl isc w glab drzewa zeby 
            //w ciele pętli nastapilo sumowanie
            while(Interlocked.CompareExchange(ref currentResult, currentResult + tree.node, tree.node) != upperResult)
            {
                upperResult += currentResult;
            }

            //tutaj dla kazego rodzica mamy liste dzieci, czyli liste odchodzacych galezi z dangeo node'a,
            //a wiec zlecamy wykonanie sumowania z powyzszej pętli 
            //poprzez dodanie tego do kolejki zadań dla strategii WS, ktora jest zdefiniowana wyzej.
            foreach (TreeNode branch in tree.children) this.Queue.Enqueue(a => SumTree(branch));

        }

        public override void Update()
        {

            Action<WorkstealingAgent> result;

            bool isEmptyQueue = true;
       
            //tutaj nalezaloby zrobic wywolanie metody SumTree, ale tylko raz, zeby nie liczyl w kolko
            //tego samego, do niej trzeba by zrobic jakis nowy obiekt drzewa z przykaldowa glebokoscia i ilsocia branchy


            while (isEmptyQueue) {
                if (Queue.TryDequeue(out result)) result(this);
                if (this.Queue.IsEmpty) { 
                    foreach (WorkstealingAgent agent in agentList) {
                        //jezeli ma co sciagnac i nie bedzie probowal sciagnac z siebie samego to ma cos zrobic
                        if(agent.Queue.TryDequeue(out result) && agent != this) result(this);               
                    } 
                } 
            }
        }
        public WorkstealingAgent(int id, List<IRunnable> agentList) : base(id) {

            this.agentList = agentList;

        }


    }
}
