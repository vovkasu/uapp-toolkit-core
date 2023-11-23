using System.Linq;
using UnityEngine;

namespace UAppToolKit.Core.ExtendedMethods
{
    public static class GameObjectExtendedMethods
    {
        public static void RemoveAllChild(this GameObject gameObject)
        {
            while (gameObject.transform.childCount > 0)
            {
                Destroy(gameObject.transform.GetChild(0).gameObject);
            }
        }

        public static void Remove(this GameObject root, GameObject child)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                if (root.transform.GetChild(i).gameObject == child)
                {
                    Destroy(root.transform.GetChild(i).gameObject);
                    return;
                }
            }
        }

        private static void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (UnityEngine.Application.isPlaying)
                {
                    if (obj is GameObject)
                    {
                        GameObject go = obj as GameObject;
                        go.transform.SetParent(null);
                    }

                    UnityEngine.Object.Destroy(obj);
                }
                else UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        public static GameObject AddChild(this GameObject parent, GameObject prefab, bool worldPositionStays = true)
        {
            var go = Object.Instantiate(prefab) as GameObject;
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform, worldPositionStays);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        public static T AddChildByComponent<T>(this GameObject parent, T prefabComponent, bool worldPositionStays = true) where T : Component
        {
            var component = Object.Instantiate(prefabComponent);
            if (component != null && parent != null)
            {
                var t = component.transform;
                t.SetParent(parent.transform, worldPositionStays);

                

                                t.localPosition = Vector3.zero;
                                t.localRotation = Quaternion.identity;
                                t.localScale = Vector3.one;
                                component.gameObject.layer = parent.layer;

                var rectTransform = prefabComponent.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }
            return component;
        }

        public static GameObject FindInActiveObjectByTag(this GameObject parent, string tag)
        {
            var objs = Resources.FindObjectsOfTypeAll<Transform>();
            return 
                (from t in objs
                 where t.hideFlags == HideFlags.None
                 where t.CompareTag(tag)
                 select t.gameObject).FirstOrDefault();
        }

        public static bool IsDestroyed(this UnityEngine.Object unityObject)
        {
            return unityObject == null && !ReferenceEquals(unityObject, null);
        }
    }
}