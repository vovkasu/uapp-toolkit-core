using System;
using System.Collections.Generic;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public abstract class EasingFunctionBase
    {
        private static readonly Dictionary<EasingMode, Func<float, Func<float, float>, float>> ModesMap =
            new Dictionary<EasingMode, Func<float, Func<float, float>, float>>
            {
                {EasingMode.EaseIn, EasingModes.EaseIn},
                {EasingMode.EaseOut, EasingModes.EaseOut},
                {EasingMode.EaseInOut, EasingModes.EaseInOut},
                {EasingMode.EaseOutIn, EasingModes.EaseOutIn}
            };

        private EasingMode _easingMode = EasingMode.EaseIn;
        public EasingMode EasingMode
        {
            get { return _easingMode; }
            set { _easingMode = value; }
        }

        protected abstract float Function(float timeProgress);

        public float FunctionAndMode(float timeProgress)
        {
            return ModesMap[EasingMode](timeProgress, Function);
        }
    }

    public enum EasingMode
    {
        EaseIn,
        EaseOut,
        EaseInOut,
        EaseOutIn
    }
}