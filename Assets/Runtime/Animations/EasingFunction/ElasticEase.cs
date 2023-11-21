
using UnityEngine;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class ElasticEase : EasingFunctionBase
    {

        protected override float Function(float t)
        {
            if (t == 1)
                return 1;

            var p =  0.3f;
            var s = p / 4f;
            t -= 1;
            return -(Mathf.Pow(2, 10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p));
        }
    }
}
