using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Win32;
using UnityEngine;
using UnityEditor;

namespace UAppToolKit.Core.Editor.PlayerPrefsTool
{
    public class PlayerPrefsEditor : EditorWindow
    {
        public enum PrefType
        {
            Float,
            Int,
            String
        }

        public List<PlayerPrefStore> playerPrefs;

        private Vector2 scrollPosition;
        private bool isCreatingNew;

        private PlayerPrefStore newPref;

        private GUIStyle boxStyle;
        private bool _isDeleteAll;

        private GUIStyle BoxStyle
        {
            get
            {
                if (boxStyle == null)
                {
                    boxStyle = new GUIStyle();
                    GUIStyleState state = new GUIStyleState();
                    state.background = MakeBoxBGTexture();
                    boxStyle.normal = state;
                    boxStyle.border = new RectOffset(6, 6, 6, 6);
                    boxStyle.margin = new RectOffset(4, 4, -1, -1);
                    boxStyle.padding = new RectOffset(10, 6, 6, 6);
                    boxStyle.stretchHeight = boxStyle.stretchWidth = true;
                }
                return boxStyle;
            }
        }

        private Texture2D MakeBoxBGTexture()
        {
            Color light = new Color(0.812f, 0.812f, 0.812f, 0.153f);
            Color dark = new Color(0f, 0f, 0f, 0.090f);
            Texture2D texture = new Texture2D(8, 8, TextureFormat.ARGB32, false);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 0 || y == 0 || x == 7 || y == 7)
                    {
                        texture.SetPixel(x, y, light);
                    }
                    else
                    {
                        texture.SetPixel(x, y, dark);
                    }
                }
            }
            texture.Apply();
            return texture;
        }

        public bool IsWindows
        {
            get { return UnityEngine.Application.platform == RuntimePlatform.WindowsEditor; }
        }

        [MenuItem("UAppToolKit/PlayerPrefs Editor", false, 1)]
        private static void Init()
        {
            GetWindow<PlayerPrefsEditor>("PlayerPrefs");
        }

        private void OnGUI()
        {
            // Sanity Checks
            if (playerPrefs == null) RefreshPlayerPrefs();

            // Toolbar
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Create New Pref", EditorStyles.toolbarButton))
            {
                newPref = new PlayerPrefStore("", "integer", "0");
                isCreatingNew = true;
            }
            if (GUILayout.Button("Delete All", EditorStyles.toolbarButton))
            {
                _isDeleteAll = true;
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                SaveAll();
            }

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                RefreshPlayerPrefs();
            }
            GUILayout.EndHorizontal();

            if (_isDeleteAll && playerPrefs != null)
            {
                _isDeleteAll = false;

                foreach (var playerPrefStore in playerPrefs)
                {
                    playerPrefStore.isMarkedForDelete = true;
                }
            }

            // Create New Pref.
            if (isCreatingNew)
            {
                if (newPref == null) newPref = new PlayerPrefStore("", "integer", "0");
                GUILayout.BeginArea(new Rect(5, 20, position.width - 10, 98), BoxStyle);
                GUILayout.Space(3);
                GUILayout.Label("Create New PlayerPref", EditorStyles.boldLabel);
                newPref.name = EditorGUILayout.TextField("New Pref Name : ", newPref.name);
                GUILayout.BeginHorizontal();
                var newPrefValue = newPref.value;
                switch (newPrefValue.type)
                {
                    case PrefType.Int:
                        newPrefValue.intValue = EditorGUILayout.IntField("Initial Value : ", newPrefValue.intValue);
                        break;
                    case PrefType.Float:
                        newPrefValue.floatValue = EditorGUILayout.FloatField("Initial Value : ", newPrefValue.floatValue);
                        break;
                    case PrefType.String:
                        newPrefValue.stringValue = EditorGUILayout.TextField("Initial Value : ", newPrefValue.stringValue);
                        break;
                }
                newPrefValue.type = (PrefType) EditorGUILayout.EnumPopup(newPrefValue.type, GUILayout.MaxWidth(80));
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create"))
                {
                    if (playerPrefs == null) playerPrefs = new List<PlayerPrefStore>();
                    playerPrefs.Add(new PlayerPrefStore(newPref.name, newPref.StringType, newPref.StringValue));
                    SaveAll();
                    isCreatingNew = false;

                    SortPlayerPrefs();
                }

                if (GUILayout.Button("Cancel"))
                {
                    isCreatingNew = false;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndArea();
                GUILayout.Space(104);
            }


            GUILayout.Label(
                "PlayerPrefs for : " + "unity." + PlayerSettings.companyName + "." + PlayerSettings.productName,
                EditorStyles.boldLabel);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (playerPrefs.Count == 0)
            {
                GUILayout.Label("No PlayerPrefs for current project", EditorStyles.miniLabel);
            }
            else
            {
                for (int i = 0; i < playerPrefs.Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.MinHeight(18));
                    if (playerPrefs[i].isMarkedForDelete)
                    {
                        GUI.color = Color.red;
                    }
                    else if (playerPrefs[i].Changed)
                    {
                        GUI.color = Color.green;
                    }

                    var prefName = playerPrefs[i].name;
                    var label = new GUIContent(prefName, $"{prefName} ");

                    var prefValue = playerPrefs[i].value;
                    var inputWidth = GUILayout.MaxWidth(500);
                    switch (prefValue.type)
                    {
                        case PrefType.Int:
                            prefValue.intValue = EditorGUILayout.IntField(label, prefValue.intValue, EditorStyles.textField, inputWidth);
                            break;
                        case PrefType.Float:
                            prefValue.floatValue = EditorGUILayout.FloatField(label, prefValue.floatValue, EditorStyles.textField, inputWidth);
                            break;
                        case PrefType.String:
                            prefValue.stringValue = EditorGUILayout.TextField(label, prefValue.stringValue, EditorStyles.textField, inputWidth);
                            break;
                    }
                    GUILayout.FlexibleSpace();
                    prefValue.type = (PrefType) EditorGUILayout.EnumPopup(prefValue.type, GUILayout.MaxWidth(140));
                    if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        playerPrefs[i].isMarkedForDelete = !playerPrefs[i].isMarkedForDelete;
                    }
                    GUILayout.EndHorizontal();
                    GUI.color = Color.white;
                }
            }
            GUILayout.EndScrollView();
        }

        private void SaveAll()
        {
            for (int i = playerPrefs.Count - 1; i >= 0; i--)
            {
                PlayerPrefStore pref = playerPrefs[i];
                if (pref.isMarkedForDelete)
                {
                    PlayerPrefs.DeleteKey(pref.name);
                    playerPrefs.RemoveAt(i);
                    continue;
                }

                switch (pref.value.type)
                {
                    case PrefType.Int:
                        PlayerPrefs.SetInt(pref.name, pref.value.intValue);
                        break;
                    case PrefType.Float:
                        PlayerPrefs.SetFloat(pref.name, pref.value.floatValue);
                        break;
                    case PrefType.String:
                        PlayerPrefs.SetString(pref.name, pref.value.stringValue);
                        break;
                }
                pref.Save();
            }
        }

        private void RefreshPlayerPrefs()
        {
            if (playerPrefs != null) playerPrefs.Clear();
            playerPrefs = new List<PlayerPrefStore>();
            if (IsWindows)
            {
                GetPrefKeysWindows();
            }
            else
            {
                GetPrefKeysMac();
            }
        }

        private void GetPrefKeysWindows()
        {
            // Unity stores prefs in the registry on Windows. 
            string regKey = @"Software\";
#if UNITY_5_5_OR_NEWER
            regKey += @"Unity\UnityEditor\";
#endif
            regKey +=  PlayerSettings.companyName + @"\" + PlayerSettings.productName;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regKey);
            foreach (string subkeyName in key.GetValueNames())
            {
                string keyName = subkeyName.Substring(0, subkeyName.LastIndexOf("_"));
                string val = key.GetValue(subkeyName).ToString();
                // getting the type of the key is not supported in Mono with registry yet :(
                // Have to infer type and guess...
                int testInt = -1;
                string newType = "";
                bool couldBeInt = int.TryParse(val, out testInt);

                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(keyName, null)))
                {
                    newType = "string";
                    val = PlayerPrefs.GetString(keyName);
                }
                else if (!float.IsNaN(PlayerPrefs.GetFloat(keyName, float.NaN)))
                {
                    newType = "real";
                    val = PlayerPrefs.GetFloat(keyName).ToString();
                }
                else
                {
                    if (couldBeInt && (PlayerPrefs.GetInt(keyName, testInt - 10) == testInt))
                    {
                        newType = "integer";
                        val = PlayerPrefs.GetInt(keyName).ToString();
                    }
                    else
                    {
                        newType = "string";
                        val = PlayerPrefs.GetString(keyName);
                    }
                }

                PlayerPrefStore pref = new PlayerPrefStore(keyName, newType, val);
                playerPrefs.Add(pref);
            }

            SortPlayerPrefs();
        }

        private void GetPrefKeysMac()
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string pListPath = homePath + "/Library/Preferences/unity." + PlayerSettings.companyName + "." +
                               PlayerSettings.productName + ".plist";
            // Convert from binary plist to xml.
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("plutil", "-convert xml1 \"" + pListPath + "\"");
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();

            StreamReader sr = new StreamReader(pListPath);
            string pListData = sr.ReadToEnd();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(pListData);

            XmlElement plist = xml["plist"];
            if (plist == null) return;
            XmlNode node = plist["dict"].FirstChild;
            while (node != null)
            {
                string name = node.InnerText;
                node = node.NextSibling;
                PlayerPrefStore pref = new PlayerPrefStore(name, node.Name, node.InnerText);
                node = node.NextSibling;
                playerPrefs.Add(pref);
            }

            //		// Convert plist back to binary
            Process.Start("plutil", " -convert binary1 \"" + pListPath + "\"");

            SortPlayerPrefs();
        }

        private void SortPlayerPrefs()
        {
            playerPrefs = playerPrefs.OrderBy(pref => pref.name).ToList();
        }
    }
}