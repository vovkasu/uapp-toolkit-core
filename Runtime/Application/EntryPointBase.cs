using System;
using System.Collections.Generic;
using RescueMatch.Core.Audio;
using UAppToolKit.Core.Options;
using UAppToolKit.Core.Pages;
using UAppToolKit.Core.SplashScreen;
using UnityEngine;

namespace UAppToolKit.Core.Application
{
    public abstract class EntryPointBase : MonoBehaviour
    {
        public static EntryPointBase Current;

        [Header("Splash screen")]
        public bool ShowSplashScreen;
        public SplashScreenPageBase SplashScreenPrefab;
        protected SplashScreenPageBase SplashScreen;

        [Header("Services")]
        public NavigationControllerBase NavigationControllerBase;
        public MediaPlayerBase MediaPlayerBase;
        public abstract OptionsProviderBase GetOptionsProviderBase();

        public event Action OnAppStarted;

        public virtual void AppStarted()
        {
            NavigationControllerBase.RunStartPage();
            SplashScreen.StartHide();
            if (OnAppStarted != null)
            {
                OnAppStarted();
            }
        }

        protected virtual void Awake()
        {
            Current = this;
            var optionsProviderBase = GetOptionsProviderBase();
            optionsProviderBase.LaunchCount++;

            if (ShowSplashScreen)
            {
                SplashScreen = Instantiate(SplashScreenPrefab, transform);
                SplashScreen.FinishHideEvent += OnFinishHideSplashScreen;
                OnSplashScreenStarted();
            }
        }

        protected virtual void Start()
        {
            var initializeApplicationTask = InitializeApplication();
            if (ShowSplashScreen)
            {
                SplashScreen.ShowProgressBar(initializeApplicationTask);
            }
            StartCoroutine(initializeApplicationTask);
        }

        private IEnumerator<float> InitializeApplication()
        {
            float fullProgress = 2f;
            float progress = 0f;

            var navigationControllerInitializingTask = NavigationControllerBase.Initialize();
            while (navigationControllerInitializingTask.MoveNext())
            {
                yield return (progress + navigationControllerInitializingTask.Current) / fullProgress;
            }
            progress = 1f;

            var mediaPlayerInitializingTask = MediaPlayerBase.Initialize();
            while (mediaPlayerInitializingTask.MoveNext())
            {
                yield return (progress + mediaPlayerInitializingTask.Current) / fullProgress;
            }

            yield return 1f;
            AppStarted();
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnApplicationFocus(bool focusStatus)
        {
        }

        protected virtual void OnApplicationPause(bool paused)
        {
        }

        protected virtual void OnApplicationQuit()
        {
        }

        protected virtual void OnDestroy()
        {
        }
        

        protected virtual void OnFinishHideSplashScreen()
        {
        }

        protected virtual void OnSplashScreenStarted()
        {
        }
    }
}