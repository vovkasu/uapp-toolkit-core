using UnityEngine;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class ExponentialEase : EasingFunctionBase
    {

        protected override float Function(float t)
        {
            if (t == 0)
            {
                return 0;
            }
            else
            {
                t -= 1;
                return Mathf.Pow(2, 10*t) ;
            }
        }
    }
}
