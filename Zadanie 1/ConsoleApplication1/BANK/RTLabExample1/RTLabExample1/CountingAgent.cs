using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class CountingAgent : Agent
    {
        private int counter = 0;

        public CountingAgent(int id) : base(id)
        {
        }

        public override void Update()
        {            
            if (counter++ >= this.Id)
            {
                //This is not generally a thread-safe console writing method. You will learn what this means and how to fix it later.
                //For now use it normally.
                Console.WriteLine("I counted to {0} and now I quit.", counter);
                Finish();
            }
        }
    }
}
