using System.Collections.Generic;
using System.Linq;
using UAppToolKit.Core.Loading;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace UAppToolKit.Core.Pages
{
    public abstract class NavigationControllerBase : MonoBehaviour
    {
        [Header("Page links")]
        public PageBaseLink StartPageLink;
        public List<PageBaseLink> PageLinkList = new List<PageBaseLink>();

        [Header("Page loading screen")]
        public LoadingScreen LoadingScreenPrefab;
        // [HideInInspector] 
        public LoadingScreen LoadingScreen;

        public static int CurrentPageIndex;
        public static readonly List<IPageBaseLink> Pages = new List<IPageBaseLink>();
        [HideInInspector] public bool BackNavigationByEscape = true;

        [HideInInspector]
        public PageBase AddPageBase;

        public abstract IEnumerator<float> Initialize();
        public abstract Navigator ActivePage();
        public abstract bool CanGoBack();

        public virtual void NavigateTo(IPageBaseLink nextPage)
        {
            NavigateTo(nextPage, null);
        }

        public abstract void NavigateTo(IPageBaseLink nextPage, object newPageArgs);

        public virtual void RestartLastPage()
        {
            RestartLastPage(null);
        }
        public abstract void RestartLastPage(object restartPageArgs);

        public virtual void GoBack()
        {
            GoBack(null);
        }

        protected virtual void QuitApplication()
        {
            Debug.Log("quit");
            UnityEngine.Application.Quit();
        }

        public void InitStartPage(Navigator startPage)
        {
            startPage.OnNavigatedTo(null);
        }

        public virtual void RunStartPage()
        {
            var activePage = ActivePage();
            activePage.OnNavigatedToCompleted();
        }

        public abstract void GoToStartPage();
        public abstract void GoBack(object prevPageArgs);

        protected virtual string GetSceneName(PageBaseLink pageBaseLink)
        {
            var baseLink = PageLinkList.First(_ => _.SceneGuid == pageBaseLink.SceneGuid);
            return baseLink.SceneName;
        }


#if UNITY_EDITOR
        protected void OnValidate()
        {
            RegisterPageBase(AddPageBase);
        }

        public void RegisterPageBase(PageBase addPageBase)
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