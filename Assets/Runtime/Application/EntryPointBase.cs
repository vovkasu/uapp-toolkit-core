using System;
using System.Collections.Generic;
using System.Linq;
using RescueMatch.Core.Audio;
using UAppToolKit.Core.Loading;
using UAppToolKit.Core.Options;
using UAppToolKit.Core.Pages;
using UAppToolKit.Core.SplashScreen;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UAppToolKit.Core.Application
{
    public abstract class EntryPointBase : MonoBehaviour
    {
        public PageBaseLink StartPage;
        public static NavigationControllerBase NavigationController;
        public static OptionsProviderBase Options;
        public static EntryPointBase Current;

        public string SoundRootPath = "Sounds/";
        public MediaPlayerBase MediaPlayerBase;
        
        [HideInInspector] public bool BackNavigationByEscape = true;

        [HideInInspector] public LoadingScreen LoadingScreen;
        public LoadingScreen LoadingScreenPrefab;

        public bool ShowSplashScreen;
        public SplashScreenPageBase SplashScreenPrefab;

        public List<PageBaseLink> PageLinkList = new List<PageBaseLink>();

        [HideInInspector]
        public PageBase AddPageBase;

        protected SplashScreenPageBase SplashScreen;
        protected Navigator[] _allPages;

        public abstract NavigationControllerBase InitNavigationController();
        public abstract OptionsProviderBase GetOptionsProviderBase();
        protected abstract MediaPlayerBase GetMediaPlayerBase(GameObject root, string soundRootPath, float backgroundMusicVolume, float sfxSoundVolume);

        public abstract void SetStartPage(GameObject startPage);
        public Action AppStarted;

        [Header("Sounds")] 
        [Range(0f, 1f)]
        public float BackgroundMusicVolume;
        [Range(0f, 1f)]
        public float SfxSoundVolume;

        public virtual void OnAppStarted()
        {
            NavigationController.RunStartPage();
            if (AppStarted != null)
            {
                AppStarted();
            }
        }

        protected virtual void Awake()
        {
#if UNITY_IOS
            UnityEngine.Application.targetFrameRate = 60;
#endif
            Options = GetOptionsProviderBase();
            Options.LaunchCount++;

            MediaPlayerBase = GetMediaPlayerBase(gameObject, SoundRootPath, BackgroundMusicVolume, SfxSoundVolume);

            _allPages = gameObject.GetComponentsInChildren<Navigator>(true);
            foreach (var children in _allPages)
            {
                children.gameObject.SetActive(false);
            }

            if (ShowSplashScreen)
            {
                SplashScreen = Instantiate(SplashScreenPrefab, transform);
                SplashScreen.FinishHideEvent += OnFinishHideSplashScreen;
                OnSplashScreenStarted();
            }
        }

        protected virtual void Start()
        {
            InitNavigationController();
        }

        public virtual LoadingScreen InstantiateLoadingScreen(LoadingScreen prefab)
        {
            if (prefab != null)
            {
                if (LoadingScreen != null)
                {
                    DestroyImmediate(LoadingScreen.gameObject, true);
                }
                LoadingScreen = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                LoadingScreen.name = "Loading Screen";
                LoadingScreen.Init();
            }
            return LoadingScreen;
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
            OnAppStarted();
        }

        protected virtual void OnSplashScreenStarted()
        {
            SplashScreen.StartHide();
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            RegistratePageBase(AddPageBase);
        }

        public void RegistratePageBase(PageBase addPageBase)
        {
            if (addPageBase != null)
            {
                var scene = addPageBase.gameObject.scene;
                var assetPathToGuid = AssetDatabase.AssetPathToGUID(scene.path);

                addPageBase.PageBaseLink = new PageBaseLink
                {
                    SceneGuid = assetPathToGuid,
                    SceneTitle = addPageBase.name,
                    SceneName = scene.name
                };

                EditorSceneManager.MarkSceneDirty(addPageBase.gameObject.scene);
                EditorSceneManager.SaveOpenScenes();

                var oldPageBaseLink = PageLinkList.FirstOrDefault(_ => _.SceneGuid == assetPathToGuid);

                if (oldPageBaseLink == null)
                {
                    var pageBaseLink = new PageBaseLink();
                    pageBaseLink.SceneGuid = assetPathToGuid;
                    pageBaseLink.SceneTitle = addPageBase.name;
                    pageBaseLink.SceneName = scene.name;
                    AddPageBase = null;
                    PageLinkList.Add(pageBaseLink);
                }
                else
                {
                    oldPageBaseLink.SceneTitle = addPageBase.PageBaseLink.SceneTitle;
                    oldPageBaseLink.SceneName = addPageBase.PageBaseLink.SceneName;
                    AddPageBase = null;
                }
                EditorUtility.SetDirty(addPageBase);
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}