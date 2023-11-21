using System;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public static class EasingModes
    {
        public static float EaseIn(float timeProgress, Func<float, float> function)
        {
            return function(timeProgress);
        }
        public static float EaseOut(float timeProgress, Func<float, float> function)
        {
            return 1f - function(1 - timeProgress);
        }

        public static float EaseInOut(float timeProgress, Func<float, float> function)
        {
            if (timeProgress < 0.5f)
            {
                return 0.5f*EaseIn(timeProgress*2f, function);
            }
            else
            {
                return 0.5f+0.5f * EaseOut((timeProgress-0.5f) * 2f, function);
            }
        }

        public static float EaseOutIn(float timeProgress, Func<float, float> function)
        {
            if (timeProgress < 0.5f)
            {
                return 0.5f * EaseOut(timeProgress * 2f, function);
            }
            else
            {
                return 0.5f + 0.5f * EaseIn((timeProgress - 0.5f) * 2f, function);
            }
        }
    }
}