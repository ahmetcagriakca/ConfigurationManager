using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationManager.Core
{
    public interface IRefreshTimeManager
    {
        void StartTimer(Func<Task> updateValueFactory);
    }
    public class RefreshTimeManager: IRefreshTimeManager
    {
        private readonly int refreshTimerIntervalInMs;

        public RefreshTimeManager( int refreshTimerIntervalInMs)
        {
            this.refreshTimerIntervalInMs = refreshTimerIntervalInMs;
        }
        public void StartTimer(Func<Task> updateValueFactory)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    // don't run again for at least refreshTimerIntervalInMs milliseconds
                    await Task.Delay(refreshTimerIntervalInMs);

                    await updateValueFactory();

                }
            });
        }
    }
}
