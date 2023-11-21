using UnityEngine;

namespace UAppToolKit.Core.Animations
{
    public class Vector4Animation : SimpleAnimationBase
    {
        public Vector4 From;
        public Vector4 To;

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
                Vector4 result = To;
                if (AutoReverse)
                {
                    Vector4 temp = From;
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
            return Vector4.LerpUnclamped(From, To, EasingFunction.FunctionAndMode(_timeStateSec / _durationSec));
        }
    }
}