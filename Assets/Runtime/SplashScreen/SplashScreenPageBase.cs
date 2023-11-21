using System;
using UAppToolKit.Core.Animations;
using UAppToolKit.Core.Application;
using UnityEngine;
using UnityEngine.UI;

namespace UAppToolKit.Core.SplashScreen
{
    public class SplashScreenPageBase : MonoBehaviour
    {
        public Image Background;
        public float HideBackgroundSec = 0.3f;

        public delegate void AnimationTickEventHandler(object value);

        public AnimationTickEventHandler AnimationTick;
        private Storyboard _hideBackgroundStoryboard;

        public event Action StartHideEvent;

        protected virtual void OnStartHideEvent()
        {
            Action handler = StartHideEvent;
            if (handler != null) handler();
        }

        public event Action FinishHideEvent;

        protected virtual void OnFinishHideEvent()
        {
            Action handler = FinishHideEvent;
            if (handler != null) handler();
        }

        public virtual void StartHide()
        {
            _hideBackgroundStoryboard = gameObject.AddComponent<Storyboard>();

            OnStartHideEvent();
            var backgroundColor = Background.color;
            var targetColor = backgroundColor;
            targetColor.a = 0;
            var colorAnimation = new Vector4Animation
            {
                From = backgroundColor,
                To = targetColor,
                Duration = TimeSpan.FromSeconds(HideBackgroundSec)
            };
            colorAnimation.Tick += color =>
            {
                Background.color = (Vector4)color;
            };
            _hideBackgroundStoryboard.Children.Add(colorAnimation);
            _hideBackgroundStoryboard.Completed += (o, eventArgs) =>
            {
                OnFinishHideEvent();
                FinalizeSpalsh();
            };
        }

        private void Update()
        {
            if (EntryPointBase.NavigationController != null && _hideBackgroundStoryboard != null &&
                !_hideBackgroundStoryboard.IsEnabled)
            {
                _hideBackgroundStoryboard.Begin();
            }
        }

        public virtual void StartHide(TimeSpan timeToShow)
        {
            StartHide();
        }

        protected virtual void FinalizeSpalsh()
        {
            Destroy(gameObject);
        }
    }
}