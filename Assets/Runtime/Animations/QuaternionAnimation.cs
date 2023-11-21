using UnityEngine;

namespace UAppToolKit.Core.Animations
{
    public class QuaternionAnimation : SimpleAnimationBase
    {
        public Quaternion From;
        public Quaternion To;

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
                var result = To;
                if (AutoReverse)
                {
                    var temp = From;
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

            var progress = EasingFunction.FunctionAndMode(_timeStateSec/_durationSec);
            return Quaternion.SlerpUnclamped(From, To, progress);
        }
    }
}