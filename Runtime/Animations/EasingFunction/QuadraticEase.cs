namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class QuadraticEase : EasingFunctionBase
    {
        protected override float Function(float t)
        {
            return t*t;
        }
    }
}