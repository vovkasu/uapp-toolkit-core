using UAppToolKit.Core.Application;
using UAppToolKit.Core.Popup;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UAppToolKit.Core.Pages
{
    public abstract class PageBase : Navigator
    {
        [HideInInspector]
        public PageBaseLink PageBaseLink;

        public T CreateCustomPopUp<T>(T popUpPrefab, Transform parent) where T : PopUpBase
        {
            var popup = Instantiate(popUpPrefab, parent);
            var rectTransform = popup.GetComponent<RectTransform>();
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            PopUps.Add(popup);
            return popup;
        }

#if UNITY_EDITOR
        [MenuItem("CONTEXT/PageBase/Registration Page for navigation")]
        public static void RegistratePage(MenuCommand command)
        {
            var pageBase = (PageBase)command.context;
            FindObjectOfType<EntryPointBase>().RegistratePageBase(pageBase);
        }
#endif
    }
}