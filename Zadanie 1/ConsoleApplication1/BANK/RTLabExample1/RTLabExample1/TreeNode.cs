using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    class TreeNode
    {

        private int node { get; set; }
        private TreeNode parent { get; set; }
        private List<TreeNode> children { get; set; }

        public TreeNode(int depth, int branching)
        {
            generateRandomTree(depth, branching);
        }
        
        private void generateRandomTree(int depth, int branching)
        {

            children = new List<TreeNode>();
            Random random = new Random();
            node = random.Next(10);

            if (depth > 0)
            {
                for (int i = 0; i < random.Next(branching); i++)
                {
                    children.Add(new TreeNode(depth - 1, random.Next(branching)));
                    children[i].parent = this;
                }
            } else
            {

                Console.Write("jak drzewo ma miec ujemna glebokosc, no jak?");
                
            }
        }

    }
}
