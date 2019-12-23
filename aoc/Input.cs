using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aoc
{
    public class Input
    {
        private readonly Queue<long> data = new Queue<long>();
        private TaskCompletionSource<long> tcs;

        public event Action OnWait;

        public bool DataReady => data.Count > 0;
        
        public void Send(params long[] values)
        {
            if (tcs != null)
            {
                var localTcs = tcs;
                tcs = null;
                foreach (var next in values.Skip(1))
                    data.Enqueue(next);
                localTcs.SetResult(values[0]);
                return;
            }

            foreach (var next in values)
                data.Enqueue(next);
        }

        public async Task<long> Wait()
        {
            if (data.Count > 0)
                return data.Dequeue();

            OnWait?.Invoke();
            
            if (data.Count > 0)
                return data.Dequeue();
            
            if (tcs != null)
                throw new InvalidOperationException();

            tcs = new TaskCompletionSource<long>();
            var result = await tcs.Task;
            return result;
        }
    }
}