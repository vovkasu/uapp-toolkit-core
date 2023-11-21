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
        private float _currectTimerState;

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
            _currectTimerState = (float)Interval.TotalSeconds;
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

            _currectTimerState -= Time.deltaTime;
            if (_currectTimerState < 0)
            {
                _currectTimerState = (float)Interval.TotalSeconds;
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