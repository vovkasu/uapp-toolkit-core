namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class QuarticEase : EasingFunctionBase
    {
        protected override float Function(float t)
        {
            return t*t*t*t;
        }
    }
}