using System;
using UnityEngine;

namespace UAppToolKit.Core.Timer
{
    public class DispatcherTimer : MonoBehaviour
    {
        public int IntervalMilliseconds;
        public TimeSpan Interval;
        public bool IsEnabled;
        public bool IsLooped;
        public bool StopOnDisable = true;
        public bool DestroyOnDisable = false;
        public event EventHandler Tick;
        private float _currentTimerState;

        public void Stop()
        {
            IsEnabled = false;
        }

        public void Begin()
        {
            IsEnabled = true;
            if (Interval.TotalMilliseconds <= 0 && IntervalMilliseconds > 0)
            {
                Interval = TimeSpan.FromMilliseconds(IntervalMilliseconds);
            }
            _currentTimerState = (float)Interval.TotalSeconds;
        }

        protected virtual void OnTick()
        {
            EventHandler handler = Tick;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
            {
                return;
            }

            _currentTimerState -= Time.fixedDeltaTime;
            if (_currentTimerState < 0)
            {
                _currentTimerState = (float)Interval.TotalSeconds;
                OnTick();
                if (!IsLooped)
                {
                    IsEnabled = false;
                }
            }
        }

        private void OnDisable()
        {
            if (StopOnDisable)
            {
                IsEnabled = false;
            }
            if (DestroyOnDisable)
            {
                Destroy(this);
            }
        }
    }
}