using System;
using UAppToolKit.Core.Animations.EasingFunction;

namespace UAppToolKit.Core.Animations
{
    public abstract class SimpleAnimationBase
    {
        public delegate void AnimationTickEventHandler(object value);

        public bool AutoReverse = false;

        public TimeSpan BeginTime;
        public TimeSpan Duration;
        public bool IsEnabled;
        public bool RepeatForever = false;

        protected float _durationSec;
        protected float _timeStateSec;

        protected SimpleAnimationBase()
        {
            EasingFunction = new Linear();
        }

        public EasingFunctionBase EasingFunction { get; set; }

        public virtual void InitAnimation()
        {
            _durationSec = (float) Duration.TotalSeconds;
            _timeStateSec = (float) BeginTime.TotalSeconds;
        }

        public event AnimationTickEventHandler Tick;

        protected abstract object InternalTick();

        public void OnTick(float timeDelta)
        {
            _timeStateSec += timeDelta;

            if (_timeStateSec < 0) return;

            AnimationTickEventHandler handler = Tick;
            if (handler != null) handler(InternalTick());
        }
    }
}