using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLabExample1
{
    //Synchronized operations can be abstracted properly if we assume that they are a T -> T mapping. Extension to e.g. T,T -> T map is trivial.
    //Observe that in e.g. Haskell such an extension would be almost pointless thanks to partial application, i.e. T, T -> T is equivalent to T -> T -> T,
    //which fits this declaration.
    //The usage of generics below is very close to C++ templates (albeit simpler).
    public interface ISyncStrategy<double>
    {
        void SetOperationContinuation(Func<double> reader, Action<double> cont);

        void Synchronize(int id, Func<T, T> operation);

        //Sometimes this is empty - i.e. mutex, spinlock. But when queueing is utilized, this is how we provide means to empty the queue.
        void ProcessRequests();
    }
}
