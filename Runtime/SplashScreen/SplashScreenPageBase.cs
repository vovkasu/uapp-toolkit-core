using System;
using System.Collections.Generic;
using UAppToolKit.Core.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace UAppToolKit.Core.SplashScreen
{
    public class SplashScreenPageBase : MonoBehaviour
    {
        public List<Graphic> AnimatedGraphics = new List<Graphic>();
        public float HideBackgroundSec = 0.3f;
        public Slider Progress;

        private IEnumerator<float> _progressTask;

        public event Action StartHideEvent;
        public event Action FinishHideEvent;


        private void Awake()
        {
            Progress.gameObject.SetActive(false);
        }

        protected virtual void OnStartHideEvent()
        {
            Action handler = StartHideEvent;
            if (handler != null) handler();
        }

        protected virtual void OnFinishHideEvent()
        {
            Action handler = FinishHideEvent;
            if (handler != null) handler();
        }

        public virtual void StartHide()
        {
            var hideBackgroundStoryboard = gameObject.AddComponent<Storyboard>();

            OnStartHideEvent();

            foreach (var animatedGraphic in AnimatedGraphics)
            {
                var graphic = animatedGraphic;
                var fromColor = graphic.color;
                var targetColor = fromColor;
                targetColor.a = 0;
                var colorAnimation = new Vector4Animation
                {
                    From = fromColor,
                    To = targetColor,
                    Duration = TimeSpan.FromSeconds(HideBackgroundSec)
                };
                colorAnimation.Tick += color =>
                {
                    graphic.color = (Vector4)color;
                };                

                hideBackgroundStoryboard.Children.Add(colorAnimation);
            }

            hideBackgroundStoryboard.Completed += (o, a) =>
            {
                OnFinishHideEvent();
                FinalizeSplash();
            };

            hideBackgroundStoryboard.Begin();
            HideProgressBar();
        }

        protected virtual void FinalizeSplash()
        {
            Destroy(gameObject);
        }

        public void ShowProgressBar(IEnumerator<float> task)
        {
            Progress.gameObject.SetActive(true);
            _progressTask = task;
        }

        public void HideProgressBar()
        {
            Progress.gameObject.SetActive(false);
            _progressTask = null;
        }

        private void FixedUpdate()
        {
            if (_progressTask != null)
            {
                Progress.value = _progressTask.Current;
            }
        }
    }
}