using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zen.Svcs
{
    /// <summary>
    /// Access list items using a round robin approach.
    /// If a service is hosted in more then 1 location, let the different locations balance the load. 
    /// </summary>
    /// <remarks>
    /// Allows services to load-balance with a software only solution. 
    /// Normally, a load balancer should be used here.    
    /// </remarks>
    public class RoundRobinList<T> : IEnumerable<T>
    {
        private readonly T[] _items;
        private int _head;

        public RoundRobinList(IEnumerable<T> endpoints)
        {
            if (endpoints == null || !endpoints.Any())
                throw new ArgumentException("One or more items must be provided", "endpoints");
            // copy the list to ensure it doesn't change on us (and so we can lock() our private copy) 
            _items = endpoints.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int currentHead;
            lock (_items)
            {
                currentHead = _head++;
                if (_head == _items.Length) _head = 0; // wrap back to the start                
            }

            // if only 1 endpoint is wanted, as opposed to a list with backup endpoints, 
            // this 'yield' is all that would be needed:
            //      yield return this.endpoints[currentHead]; 
            //      return results [current] ... [last] 
            for (var i = currentHead; i < _items.Length; i++)
                yield return _items[i];
            
            //return wrap-around (if any) [0] ... [current-1] 
            for (var i = 0; i < currentHead; i++)
                yield return _items[i];            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}