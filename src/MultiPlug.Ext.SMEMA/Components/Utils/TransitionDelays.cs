using System;
using System.Threading;
using System.Threading.Tasks;
using MultiPlug.Ext.SMEMA.Models.Exchange;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.SMEMA.Components.Utils
{
    internal static class TransitionDelays
    {
        internal static CancellationTokenSource Unblock(Action theAction, CancellationTokenSource theCancellationTokenSource, int theMillisecondsDelay)
        {
            return InvokeEvent(theAction, theCancellationTokenSource, theMillisecondsDelay);
        }

        internal static CancellationTokenSource InvokeEvent(Action theAction, CancellationTokenSource theCancellationTokenSource, int theMillisecondsDelay)
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
            DelayCancel(theCancellationTokenSource);
        }

        internal static void DelayCancel(CancellationTokenSource theCancellationTokenSource)
        {
            if (theCancellationTokenSource != null)
            {
                theCancellationTokenSource.Cancel();
            }
        }

        internal static bool UnblockInProgress(CancellationTokenSource theTimer)
        {
            return DelayInProgress(theTimer);
        }

        internal static bool DelayInProgress(CancellationTokenSource theTimer)
        {
            return theTimer != null && theTimer.IsCancellationRequested == false;
        }

        internal static void SMEMAEventChange(bool theCurrentState,
                                        bool theNewState,
                                        ref CancellationTokenSource theHighDelayCancellationSource,
                                        Action onHigh,
                                        ref CancellationTokenSource theLowDelayCancellationSource,
                                        Action onLow,
                                        SMEMAEvent theSMEMAEvent)
        {
            if (theHighDelayCancellationSource == null || theLowDelayCancellationSource == null)
            {
                throw new Exception("A Cancellation Source can not be null");
            }

            if (theNewState)
            {
                if (TransitionDelays.DelayInProgress(theLowDelayCancellationSource))
                {
                    TransitionDelays.DelayCancel(theLowDelayCancellationSource);
                }
                else if (theCurrentState != theNewState && (!TransitionDelays.DelayInProgress(theHighDelayCancellationSource)))
                {
                    theHighDelayCancellationSource = TransitionDelays.InvokeEvent(() =>
                    {
                        onHigh();
                        theSMEMAEvent.Invoke(new Payload(theSMEMAEvent.Id, new PayloadSubject[] {
                            new PayloadSubject(theSMEMAEvent.Subjects[0], theSMEMAEvent.HighValue)
                        }));
                    },
                    theHighDelayCancellationSource,
                    theSMEMAEvent.HighDelay.Value);
                }
            }
            else
            {
                if (TransitionDelays.DelayInProgress(theHighDelayCancellationSource))
                {
                    TransitionDelays.DelayCancel(theHighDelayCancellationSource);
                }
                else if (theCurrentState != theNewState && (!TransitionDelays.DelayInProgress(theLowDelayCancellationSource)))
                {
                    theLowDelayCancellationSource = TransitionDelays.InvokeEvent(() =>
                    {
                        onLow();
                        theSMEMAEvent.Invoke(new Payload(theSMEMAEvent.Id, new PayloadSubject[] {
                            new PayloadSubject(theSMEMAEvent.Subjects[0], theSMEMAEvent.LowValue )
                    }));
                    },
                    theLowDelayCancellationSource,
                    theSMEMAEvent.LowDelay.Value);
                }
            }
        }
    }
}
