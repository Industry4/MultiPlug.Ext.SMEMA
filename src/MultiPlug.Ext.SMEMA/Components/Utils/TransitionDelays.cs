using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiPlug.Ext.SMEMA.Components.Utils
{
    internal static class TransitionDelays
    {
        internal static CancellationTokenSource Unblock(Action theAction, CancellationTokenSource theCancellationTokenSource, int theMillisecondsDelay)
        {
            if (theCancellationTokenSource != null)
            {
                theCancellationTokenSource.Cancel();
            }

            theCancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                Task DelayTask = Task.Delay(theMillisecondsDelay);
                try
                {
                    DelayTask.Wait(theCancellationTokenSource.Token);
                    theCancellationTokenSource.Cancel(); // Leave as Canceled as a signal the timer is complete
                    theAction();
                }
                catch (OperationCanceledException)
                {
                }
            },
            theCancellationTokenSource.Token
            );

            return theCancellationTokenSource;
        }

        internal static void UnblockCancel(CancellationTokenSource theCancellationTokenSource)
        {
            if (theCancellationTokenSource != null)
            {
                theCancellationTokenSource.Cancel();
            }
        }

        internal static bool UnblockInProgress(CancellationTokenSource theTimer)
        {
            return theTimer != null && theTimer.IsCancellationRequested == false;
        }
    }
}
