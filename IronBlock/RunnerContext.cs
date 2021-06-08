using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        Timed
    }

    public class RunnerContext : Context, IDisposable
    {
        public RunMode RunMode => _runMode;

        private Timer _timer;
        private SemaphoreSlim _semaphore;
        private RunMode _runMode;

        public RunnerContext(RunMode stepMode, double stepIntervalMilliSeconds = 1000.0) : base()
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
                    BeforeEvent += BeforeEventHandler;
                    break;
                case RunMode.Timed:
                    BeforeEvent += BeforeEventHandler;
                    _timer.Start();
                    break;
            }

            _runMode = mode;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Step();
        }


        private void BeforeEventHandler(object sender, IBlock block)
        {
            _semaphore.Wait();
        }


        public void Step()
        {
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