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
        public OptionsProviderBase Options;

        public MediaPlayerBase MediaPlayerBase;

        public abstract OptionsProviderBase GetOptionsProviderBase();

        public abstract void SetStartPage(GameObject startPage);
        public Action AppStarted;

        [Header("Sounds")] 
        [Range(0f, 1f)]
        public float BackgroundMusicVolume;
        [Range(0f, 1f)]
        public float SfxSoundVolume;

        public virtual void OnAppStarted()
        {
            NavigationControllerBase.RunStartPage();
            SplashScreen.StartHide();
            if (AppStarted != null)
            {
                AppStarted();
            }
        }

        protected virtual void Awake()
        {
            Current = this;
            Options = GetOptionsProviderBase();
            Options.LaunchCount++;

            MediaPlayerBase.Initialize(BackgroundMusicVolume, SfxSoundVolume);

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
            float fullProgress = 1f;
            float progress = 0f;

            var navigationControllerInitializingTask = NavigationControllerBase.Initialize();
            while (navigationControllerInitializingTask.MoveNext())
            {
                yield return (progress + navigationControllerInitializingTask.Current) / fullProgress;
            }

            yield return 1f;
            OnAppStarted();
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