﻿using System;
using UnityEngine;

namespace UAppToolKit.Core.Pages
{
    [Serializable]
    public class PageBaseLink : ScriptableObject, IPageBaseLink
    {
        public string SceneName;
        public string PageName;
        public string ScenePath;


#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset;

        private void OnValidate()
        {
            if (SceneAsset == null)
            {
                Debug.LogError($"Scene {SceneName} lost.", this);
                return;
            }

            if (SceneAsset.name != SceneName)
            {
                Debug.LogWarning($"Scene {SceneName} renamed to {SceneAsset.name}.");
                SceneName = SceneAsset.name;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(SceneAsset);
            if (ScenePath != assetPath)
            {
                Debug.LogWarning($"Scene {SceneName} moved to {assetPath}.");
                ScenePath = assetPath;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}