using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IronBlock
{
    public enum RunMode
    {
        // The Evaluate method runs without interruptions
        Continuous,

        // The Evaluate Method is stopped at the beginning of each block and waits for Step() call
        Stepped,

        // The Evaluate Method is stopped at the beginning of the block and waits for Step();
        // The Step() is triggered automatically at specified interval
        Timed,
        Stopped
    }

    public class RunnerContext : Context, IDisposable
    {
        public RunMode RunMode => _runMode;

        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphore;
        private RunMode _runMode;


        public RunnerContext(RunMode stepMode,
            double stepIntervalMilliSeconds = 1000.0,
            CancellationToken interruptToken = default,
            Context parentContext = null) : base(parentContext, interruptToken)
        {
            _timer = new Timer(stepIntervalMilliSeconds);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsed;
            _semaphore = new SemaphoreSlim(0, 1);
            SetRunMode(stepMode);
        }

        public void SetRunMode(RunMode mode)
        {
            // move to known state
            _timer.Stop();
            BeforeEvent -= BeforeEventHandler;
            switch (mode)
            {
                case RunMode.Continuous:
                    break;

                case RunMode.Stepped:
                case RunMode.Stopped:
                    BeforeEvent += BeforeEventHandler;
                    break;

                case RunMode.Timed:
                    BeforeEvent += BeforeEventHandler;
                    _timer.Start();
                    break;
            }

            _runMode = mode;
        }

        protected void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Step();
        }


        private void BeforeEventHandler(object sender, IBlock block)
        {
            _semaphore.Wait();
        }

        public override void Interrupt()
        {
            base.Interrupt();
            Step();
        }

        public void Step()
        {
            if (_runMode != RunMode.Stopped)
            {
                return;
            }

            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
        }


        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= TimerElapsed;
                _timer.Dispose();
            }
        }
    }
}