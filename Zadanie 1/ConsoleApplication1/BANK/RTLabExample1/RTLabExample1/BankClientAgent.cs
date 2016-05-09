using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class BankClientAgent : Agent
    {
        private readonly BankAgent bank;

        private readonly IWaitStrategy waiter;

        private bool silent;

        private readonly int operations;

        private readonly double balanceChange;

        private int iterations;

        public BankClientAgent(int id, double balanceChange, BankAgent bank, IWaitStrategy waiter, bool silent = true, int operations = 100, int iterations = 10) : base(id)
        {
            //We could make a simple POD structure to hold this information and reduce the boilerplate code a bit.
            this.bank = bank;
            this.waiter = waiter;
            this.silent = silent;
            this.operations = operations;
            this.iterations = iterations;
            this.balanceChange = balanceChange;
        }

        private double Operation(double balance)
        {
            //Simulate a long-running operation to observe various timing phenomena.
            waiter.Wait();
            var newBalance = balance + balanceChange;
            if (!this.silent)
                Console.WriteLine("Perceived account balance after operation: {0}.", bank.Balance);
            return newBalance;
        }

        public override void Update()
        {
            --iterations;
            for(int i = 0; i < operations; ++i)
            {
                waiter.Wait();
                if (!this.silent)
                    Console.WriteLine("Perceived account balance before operation: {0}.", bank.Balance);
                bank.Synchronizer.Synchronize(this.Id, this.Operation);                    
            }
            if (iterations < 0)
            {
                Console.WriteLine("Client {0} finished.", this.Id);
                this.Finish();
            }

        }
    }
}
