using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class ConstantCountingAgent : Agent
    {
        private int counter = 0;

        public ConstantCountingAgent(int id) : base(id)
        {
        }

        public override void Update()
        {
            if (counter++ >= 10)
            {
                //This is not generally a thread-safe console writing method. You will learn what this means and how to fix it later.
                //For now use it normally.
                Console.WriteLine("I counted to 10 and my Id is {0}.", this.Id);
                Finish();
            }
        }
    }
}
