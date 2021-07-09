using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IronBlock
{
    public enum RunMode
    {
        /// <summary>
        /// The Evaluate method runs without interruptions
        /// </summary>
        Continuous,

        /// <summary>
        ///  The Evaluate Method is stopped at the beginning of each block and waits for Step() call
        /// </summary>
        Stepped,

        /// <summary>
        /// The Evaluate Method is stopped at the beginning of the block and waits for Step();
        /// The Step() is triggered automatically at specified interval 
        /// </summary>
        Timed,
        
        /// <summary>
        /// Execution is stopped, Step() function will not trigger evaluation
        /// </summary>
        Stopped
    }

    /// <summary>
    /// Used to time the execution block by block manually or on timer 
    /// </summary>
    public class RunnerContext : Context, IDisposable
    {
        public RunMode RunMode => _runMode;

        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphore;
        private RunMode _runMode;


        public RunnerContext(RunMode stepMode,
            double stepIntervalMilliSeconds = 1000.0,
            CancellationToken interruptToken = default,
            IContext parentContext = null) : base(parentContext, interruptToken)
        {
            _timer = new Timer(stepIntervalMilliSeconds);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsedHandler;
            _semaphore = new SemaphoreSlim(0, 1);
            SetRunMode(stepMode);
        }

        public void SetRunMode(RunMode mode)
        {
            // move to known state
            _timer.Enabled = false;
            BeforeEvent -= BeforeEventHandler;
            switch (mode)
            {
                case RunMode.Continuous:
                    Step();
                    break;

                case RunMode.Stepped:
                    BeforeEvent += BeforeEventHandler;
                    break;
                
                case RunMode.Stopped:
                    break;

                case RunMode.Timed:
                    BeforeEvent += BeforeEventHandler;
                    _timer.Enabled = true;
                    break;
            }

            _runMode = mode;
        }

        protected void TimerElapsedHandler(object sender, ElapsedEventArgs e)
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
            if (_runMode == RunMode.Stopped)
            {
                return;
            }

            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
        }


        #region IDisposable Pattern Support

        private bool _isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Enabled = false;
                    _timer.Elapsed -= TimerElapsedHandler;
                    _timer.Dispose();
                }
            }

            _isDisposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}