using UnityEngine;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class CircleEase : EasingFunctionBase
    {
        protected override float Function(float t)
        {
            return 1 - Mathf.Sqrt(1 - t*t);
        }
    }
}