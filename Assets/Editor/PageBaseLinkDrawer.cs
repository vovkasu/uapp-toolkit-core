using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UAppToolKit.Core.Application;
using UAppToolKit.Core.Pages;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Editor
{
    [CustomPropertyDrawer(typeof(PageBaseLink))]
    public class PageBaseLinkDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var sceneNameRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            var nameRect = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.5f, position.height);

            var targetScript = property.serializedObject.targetObject;

            if (fieldInfo.Name == "PageLinkList")
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(sceneNameRect, property.FindPropertyRelative("SceneName"), GUIContent.none);
                GUI.enabled = true;

                EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("SceneTitle"), GUIContent.none);
            }
            else
            {
                var navigationController = Object.FindObjectOfType<EntryPointBase>();
                if (navigationController != null)
                {
                    var titles = navigationController.PageLinkList.Select(_ => _.SceneTitle).ToArray();
                    PageBaseLink targetPageBaseLink = null;

                    var path = property.propertyPath.Replace(".Array.data[", "[");
                    var paths = path.Split('.');
                    var targetScriptType = targetScript.GetType();

                    if (property.propertyPath.Contains(".Array.data["))
                    {
                        var index = 0;
                        var field = fieldInfo;
                        var element = paths[0];
                        if (element.Contains("["))
                        {
                            var elementName = element.Substring(0, element.IndexOf("["));
                            index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                            element = element.Substring(0, element.IndexOf("["));
                        }
                        field = targetScriptType.GetField(element);
                        var obj = field.GetValue(targetScript);
                        object objElement = null;
                        if (obj != null && obj is object[])
                        {
                            var objArray = (object[])field.GetValue(targetScript);
                            if (objArray.Length <= index)
                            {
                                return;
                            }
                            objElement = objArray[index];
                        }
                        else
                        {
                            var objEnumerable = (ICollection)field.GetValue(targetScript);
                            var objEnumerator = objEnumerable.GetEnumerator();
                            if (objEnumerable.Count <= index)
                            {
                                return;
                            }
                            for (int i = 0; i < index + 1; i++)
                            {
                                objEnumerator.MoveNext();
                            }
                            objElement = objEnumerator.Current;
                        }
                        field = objElement.GetType().GetField(fieldInfo.Name);
                        if (fieldInfo != null)
                        {
                            targetPageBaseLink = fieldInfo.GetValue(objElement) as PageBaseLink;
                        }
                        else
                        {
                            Debug.Log("Something went wrong!");
                            return;
                        }
                    }
                    else
                    {
                        var field = fieldInfo;
                        object obj = targetScript;
                        for (int i = 0; i < paths.Length - 1; i++)
                        {
                            field = targetScriptType.GetField(paths[i]);
                            obj = field.GetValue(targetScript);
                        }
                        field = obj.GetType().GetField(fieldInfo.Name);
                        if (field != null)
                        {
                            targetPageBaseLink = field.GetValue(obj) as PageBaseLink;
                        }
                        else
                        {
                            Debug.Log("Something went wrong!");
                            return;
                        }
                    }

                    var oldPageLink = navigationController.PageLinkList.FirstOrDefault(_ => _.SceneGuid == targetPageBaseLink.SceneGuid);
                    var indexOf = navigationController.PageLinkList.IndexOf(oldPageLink);
                    var newIndex = EditorGUI.Popup(position, indexOf, titles);
                    if (newIndex < 0 || newIndex >= titles.Length)
                    {
                        newIndex = 0;
                    }

                    if (titles.Length > 0)
                    {
                        var newPageLink = navigationController.PageLinkList[newIndex];
                        targetPageBaseLink.SceneGuid = newPageLink.SceneGuid;
                        targetPageBaseLink.SceneTitle = newPageLink.SceneTitle;
                        targetPageBaseLink.SceneName = newPageLink.SceneName;
                    }
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}