using UnityEngine;

namespace UAppToolKit.Core.Animations.EasingFunction
{
    public class PowerEase : EasingFunctionBase
    {
        public float Power { get; set; }

        protected override float Function(float t)
        {
            return Mathf.Pow(t, Power);
        }
    }
}