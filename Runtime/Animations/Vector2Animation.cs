using UnityEngine;

namespace UAppToolKit.Core.Animations
{
    public class Vector2Animation : SimpleAnimationBase
    {
        public Vector2 From;
        public Vector2 To;

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
                Vector2 result = To;
                if (AutoReverse)
                {
                    Vector2 temp = From;
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
            return Vector2.LerpUnclamped(From, To, EasingFunction.FunctionAndMode(_timeStateSec/_durationSec));
        }
    }
}