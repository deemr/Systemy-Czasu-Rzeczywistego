using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class AgentText : Agent
    {

        public override void Update()
        {

            List<string> result = Stringi.Split(new char[] { ' ', '.', ',', '\t', '\n' }).ToList();


            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            

            result.ForEach(r =>
            {

                int value = 0;
                if (dictionary.TryGetValue(r, out value))
                {
                    dictionary[r] = dictionary[r] + 1;
                } else
                {
                    dictionary.Add(r, 1);
                }
            });

            Dictionary<string, int>.KeyCollection keyColl =
            dictionary.Keys;
            Dictionary<string, int>.ValueCollection valueColl =
           dictionary.Values;
            foreach (string key in keyColl)
            {
                Console.WriteLine("Words = {0} - {1}", key, dictionary[key]);
                //Console.WriteLine("Value={0}",);
            }


            this.HasFinished = true;


        }
        public AgentText(int agentId, string Stringi)
        {
            this.Stringi = Stringi;
            this.agentId = agentId;
        }


    }
}
