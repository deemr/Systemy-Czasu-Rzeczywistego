using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class BankAgent : Agent
    {
        private int updates;

        public double Balance { get; private set; } = 0.0;

        public ISyncStrategy<double> Synchronizer { get; private set; }        

        public BankAgent(int id, ISyncStrategy<double> synchronizer, int updates) : base(id)
        {
            this.Synchronizer = synchronizer;
            this.updates = updates;

            //Synchronizer is granted the ability to modify balance without any knowledge about Bank implementation.
            synchronizer.SetOperationContinuation(() => this.Balance, newBalance => this.Balance = newBalance);
        }

        public override void Update()
        {
            --updates;
            //We utilize an ability to write lambdas with multiple statements by enclosing them in braces.
            Console.WriteLine("Bank account state: {0}.", this.Balance);
            Synchronizer.ProcessRequests();
            if(updates < 0)
            {
                Console.WriteLine("Final bank account state: {0}.", this.Balance);
                this.Finish();
            }
        }
    }
}
