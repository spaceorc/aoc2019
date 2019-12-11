using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aoc
{
    public class Input
    {
        private readonly Queue<long> data = new Queue<long>();
        private TaskCompletionSource<long> tcs;

        public void Send(long value)
        {
            if (tcs != null)
            {
                var localTcs = tcs;
                tcs = null;
                localTcs.SetResult(value);
                return;
            }

            data.Enqueue(value);
        }

        public async Task<long> Wait()
        {
            if (data.Count > 0)
                return data.Dequeue();

            if (tcs != null)
                throw new InvalidOperationException();

            tcs = new TaskCompletionSource<long>();
            return await tcs.Task;
        }
    }
}