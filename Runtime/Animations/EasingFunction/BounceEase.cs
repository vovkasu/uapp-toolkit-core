namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class BounceEase : EasingFunctionBase 
    {
        protected override float Function(float timeProgress)
        {
            float t = 1 - timeProgress;
            if (t < (1f/2.75f))
            {
                return 1 - (7.5625f*t*t);
            }
            if (t < (2/2.75f))
            {
                t -= (1.5f/2.75f);
                return 1 - (7.5625f*t*t + 0.75f);
            }
            if (t < (2.5/2.75f))
            {
                t -= (2.25f/2.75f);
                return 1 - (7.5625f*t*t + 0.9375f);
            }
            t -= (2.625f/2.75f);
            return 1 - (7.5625f*t*t + 0.984375f);
        }
    }
}