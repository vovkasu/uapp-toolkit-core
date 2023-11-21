using UnityEngine;

namespace UAppToolKit.Core.Animations
{
    public class Vector3Animation : SimpleAnimationBase
    {
        public Vector3 From;
        public Vector3 To;

        public override void InitAnimation()
        {
            base.InitAnimation();

            IsEnabled = true;
            if (_timeStateSec > _durationSec)
            {
                IsEnabled = false;
            }
        }

        protected override object InternalTick()
        {
            if (_timeStateSec >= _durationSec)
            {
                Vector3 result = To;
                if (AutoReverse)
                {
                    Vector3 temp = From;
                    From = To;
                    To = temp;
                    InitAnimation();
                    if (!RepeatForever)
                    {
                        AutoReverse = false;
                    }
                }
                else
                {
                    IsEnabled = false;
                    _timeStateSec = 0;
                }
                return result;
            }
            return Vector3.LerpUnclamped(From, To, EasingFunction.FunctionAndMode(_timeStateSec / _durationSec));
        }
    }
}