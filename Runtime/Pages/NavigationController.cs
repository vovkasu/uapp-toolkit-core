using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UAppToolKit.Core.Application;
using UAppToolKit.Core.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UAppToolKit.Core.Pages
{
    public class NavigationController : NavigationControllerBase
    {
        protected PageBase CurrentPage;
        private Scene _applicationScene;
        protected bool IsNavigationRan;
        protected DateTime _startPageNavigationTime;
        public TimeSpan MinTimePageLoading = TimeSpan.FromSeconds(1);

        public event Action OnStartPageLoaded;

        private bool isSplashScreenReady;
        private bool isStartPageReady;
        private Action _onLoadedAction;

        public override IEnumerator<float> Initialize()
        {
            _applicationScene = EntryPointBase.Current.gameObject.scene;
            var startPageSceneName = GetSceneName(StartPageLink);

            var scenesForUnload = new List<Scene>();
            Scene? startPageScene = null;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene == _applicationScene)
                {
                    continue;
                }

                if (scene.name == startPageSceneName)
                {
                    startPageScene = scene;
                    continue;
                }
                scenesForUnload.Add(scene);
            }

            float progressSize = scenesForUnload.Count + 1f;
            float progress = 0f;

            foreach (var scene in scenesForUnload)
            {
                var unloadSceneAsync = SceneManager.UnloadSceneAsync(scene.name);
                while (!unloadSceneAsync.isDone)
                {
                    yield return (progress + unloadSceneAsync.progress) / progressSize;
                }

                progress += 1f;
            }

            if (startPageScene.HasValue)
            {
                SceneManager.SetActiveScene(startPageScene.Value);
                CurrentPage = FindPageBase(startPageScene.Value);
                InitStartPage(ActivePage());
                OnStartPageActive(OnStartPageLoaded);
            }
            else
            {
                var loadSceneAsync = LoadSceneAsync(StartPageLink, _ =>
                {
                    OnPageLoaded(_, null, InitStartPage, true);
                    OnStartPageActive(OnStartPageLoaded);
                });

                while (loadSceneAsync.MoveNext())
                {
                    yield return (progress + loadSceneAsync.Current) / progressSize;
                }
            }
            Pages.Add(StartPageLink);
            IsNavigationRan = false;

            InstantiateLoadingScreen(LoadingScreenPrefab);
            yield return 1f;
        }

        private void OnStartPageActive(Action onPageLoad)
        {
            if (ActivePage().IsActive)
            {
                StartPageLocker(false, onPageLoad);
            }
            else
            {
                ActivePage().OnPageLoaded += (sender, args) => { StartPageLocker(false, onPageLoad); };
            }
        }

        protected virtual void OnPageLoaded(PageBase page, object newPageArgs, Action<PageBase> initPage = null, bool isStartPage = false)
        {
            CurrentPage = page;
            if (initPage == null)
            {
                page.OnNavigatedTo(newPageArgs);
            }
            else
            {
                initPage(CurrentPage);
            }
            StartCoroutine(DelayStartPage(page, DateTime.Now - _startPageNavigationTime, isStartPage));
        }

        private IEnumerator DelayStartPage(PageBase page, TimeSpan pageLoadTime, bool isStartPage)
        {
            var secs = Mathf.Max((float) (MinTimePageLoading.TotalSeconds - pageLoadTime.TotalSeconds), 0f);
            yield return new WaitForSeconds(secs);

//            if (!isStartPage)
//            {
//                page.OnNavigatedToCompleted();
//            }
//            IsNavigationRan = false;
            HideLoadingScreen(() =>
            {
                if (!isStartPage)
                {
                    page.OnNavigatedToCompleted();
                }
                IsNavigationRan = false;
            });
        }

        public override Navigator ActivePage()
        {
            return CurrentPage;
        }

        public override bool CanGoBack()
        {
            if (CurrentPageIndex == Pages.Count - 1 && CurrentPage.PopUps.Count > 0) return true;
            return CurrentPageIndex > 0;
        }

        public override void NavigateTo(IPageBaseLink nextPage, object newPageArgs)
        {
            if (IsNavigationRan) return;
            CurrentPageIndex++;

            GoToPage((PageBaseLink) nextPage, newPageArgs);

            Pages.Add(nextPage);
        }

        public override void RestartLastPage(object restartPageArgs)
        {
            if (IsNavigationRan) return;
            GoToPage((PageBaseLink) Pages.Last(), restartPageArgs);
        }

        public void NavigateToLoadedPage(IPageBaseLink nextPage, Action onComplete)
        {
            if (IsNavigationRan) return;
            CurrentPageIndex++;
            
            IsNavigationRan = true;
            _startPageNavigationTime = DateTime.Now;
            CurrentPage.OnNavigatedFrom();
            ShowLoadingScreen(() =>
            {
                var sceneName = GetSceneName((PageBaseLink)nextPage);
                var sceneByName = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(sceneByName);
                var pageBase = FindPageBase(sceneByName);
                OnPageLoaded(pageBase, null);
                if(onComplete != null) onComplete();
            });
            CurrentPage.OnNavigatedFromCompleted();
            SceneManager.UnloadScene(GetSceneName(CurrentPage.PageBaseLink));

            Pages.Add(nextPage);
        }

        public override void GoBack(object prevPageArgs)
        {
            if (IsNavigationRan) return;
            if (!CanGoBack())
            {
                QuitApplication();
                return;
            }

            if (CurrentPage.PopUps.Count > 0)
            {
                var popUp = CurrentPage.PopUps.Last();
                CurrentPage.PopUps.Remove(popUp);
                popUp.Close(() =>
                {
                    popUp.gameObject.SetActive(false);
                    Object.Destroy(popUp.gameObject);
                });
                return;
            }
            var prevPageLink = Pages[CurrentPageIndex - 1];

            GoToPage((PageBaseLink) prevPageLink, prevPageArgs);

            Pages.Remove(Pages.Last());
            CurrentPageIndex--;
        }

        public override void GoToStartPage()
        {
            if (IsNavigationRan) return;
            var firstPageLink = Pages.First();
            Pages.Clear();
            CurrentPageIndex = 0;

            GoToPage((PageBaseLink) firstPageLink, null);

            Pages.Add(firstPageLink);
        }

        protected static PageBase FindPageBase(Scene sceneByName)
        {
            var rootGameObjects = sceneByName.GetRootGameObjects();
            return rootGameObjects.Select(_ => _.GetComponent<PageBase>()).FirstOrDefault(_ => _ != null);
        }

        protected virtual void GoToPage(PageBaseLink nextPage, object newPageArgs)
        {
            IsNavigationRan = true;
            _startPageNavigationTime = DateTime.Now;
            var currentPage = CurrentPage;

            currentPage.OnNavigatedFrom();
            var currentPageBaseLink = currentPage.PageBaseLink;
            ShowLoadingScreen(() =>
            {
                UnloadSceneAsync(currentPageBaseLink, () =>
                {
                    currentPage.OnNavigatedFromCompleted();
                    LoadScene(nextPage, _ => OnPageLoaded(_, newPageArgs));
                });
            });
        }

        private void UnloadSceneAsync(PageBaseLink pageBaseLink, Action onComplete)
        {
            var unloadSceneAsync = SceneManager.UnloadSceneAsync(GetSceneName(pageBaseLink));
            void Unloaded(AsyncOperation asyncOperation)
            {
                unloadSceneAsync.completed -= Unloaded;
                if (onComplete != null) onComplete();
            }

            unloadSceneAsync.completed += Unloaded;
        }

        protected virtual void ShowLoadingScreen(Action onComplete)
        {
            //EntryPointBase.Current.LoadingScreen.gameObject.SetActive(true);
            LoadingScreen.FadeIn(onComplete);
        }

        private void HideLoadingScreen(Action onComplete)
        {
            if (LoadingScreen != null)
            {
                //EntryPointBase.Current.LoadingScreen.gameObject.SetActive(false);
                LoadingScreen.FadeOut(onComplete);
            }
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

        protected virtual void LoadScene(PageBaseLink nextPage, Action<PageBase> onFinishAction)
        {
            StartCoroutine(LoadSceneAsync(nextPage, onFinishAction));
        }

        private IEnumerator<float> LoadSceneAsync(PageBaseLink nextPage, Action<PageBase> onFinishAction)
        {
            var sceneName = GetSceneName(nextPage);
            var result = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            result.allowSceneActivation = true;

            while (!result.isDone)
            {
                yield return result.progress;
            }
            var sceneByName = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(sceneByName);
            var pageBase = FindPageBase(sceneByName);
            onFinishAction(pageBase);
            yield return 1f;
        }

        public void StartPageLocker(bool isTimer, Action onLoadedAction)
        {
            if (onLoadedAction != null)
            {
                _onLoadedAction += onLoadedAction;
            }
            if (isTimer)
            {
                isSplashScreenReady = true;
            }
            else
            {
                isStartPageReady = true;
            }
            if ((isSplashScreenReady || !EntryPointBase.Current.ShowSplashScreen) && isStartPageReady)
            {
                _onLoadedAction();
            }
        }
    }
}