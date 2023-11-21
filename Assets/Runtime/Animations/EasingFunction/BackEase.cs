namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class BackEase : EasingFunctionBase
    {
        public float Amplitude = 1.70158f;
        protected override float Function(float t)
        {
            return t*t*((Amplitude + 1)*t - Amplitude);
        }
    }
}