using System;
using UnityEngine;

namespace UAppToolKit.Core.Pages
{
    [Serializable]
    public class PageBaseLink : IPageBaseLink
    {
        public string SceneTitle;
        [HideInInspector] public string SceneName;
        [HideInInspector] public string SceneGuid;
    }
}