﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    interface IRunnable
    {
        void Run();
        IEnumerator<float> CoroutineUpdate();
        bool HasFinished{
            get;
            set;
        }
    }

}

