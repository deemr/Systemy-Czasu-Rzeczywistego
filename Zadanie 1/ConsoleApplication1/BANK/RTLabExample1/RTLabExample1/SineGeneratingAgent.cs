using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    public class SineGeneratingAgent : Agent
    {
        public float Output { get; private set; } = 0.0f;

        public SineGeneratingAgent(int id) : base(id)
        {
        }

        public override void Update()
        {
            Output = (float)Math.Sin(this.virtualTimeS);
            if (this.virtualTimeS  >=  this.Id % 10)
            {
                Console.WriteLine("I happily generated some sines and now I quit.");
                Finish();
            }
        }
    }
}
