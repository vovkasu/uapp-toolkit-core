﻿using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace UAppToolKit.Core.Options
{
    public abstract class OptionsProviderBase : INotifyPropertyChanged
    {
        protected const string FirstLaunchName = "FirstLaunch";
        protected const string LaunchCountName = "LaunchCount";

        protected const string IntType = "Int";
        protected const string FloatType = "Float";
        protected const string StringType = "String";
        protected const string BooleanType = "Boolean";
        protected const string CharType = "Char";
        protected const string DateTimeType = "DateTime";
        protected const string ValueSuffix = "Value";
        protected const string TypeSuffix = "Type";

        public int LaunchCount
        {
            get
            {
                if (!ContainsPref(LaunchCountName))
                {
                    FirstLaunch = DateTime.Now;
                }

                return (int) GetValueOrDefault(LaunchCountName, 0);
            }
            set { Save(LaunchCountName, value); }
        }

        public DateTime FirstLaunch
        {
            get 
            {
                var now = DateTime.Now;
                if (!ContainsPref(FirstLaunchName))
                {
                    Save(FirstLaunchName, now);
                }
                return (DateTime)GetValueOrDefault(FirstLaunchName, now);
            }
            private set { Save(FirstLaunchName, value); }
        }

#region Int

        private void SaveInt(string propertyName, int value)
        {
            PlayerPrefs.SetInt(PropKey(propertyName), value);
            PlayerPrefs.SetString(TypeKey(propertyName), IntType);
            PlayerPrefs.Save();
        }

        private int LoadInt(string propertyName)
        {
            return PlayerPrefs.GetInt(PropKey(propertyName));
        }

#endregion

#region Float

        private void SaveFloat(string propertyName, float value)
        {
            PlayerPrefs.SetFloat(PropKey(propertyName), value); 
            PlayerPrefs.SetString(TypeKey(propertyName), FloatType);
            PlayerPrefs.Save();
        }

        private float LoadFloat(string propertyName)
        {
            return PlayerPrefs.GetFloat(PropKey(propertyName));
        }

#endregion

#region String

        private void SaveString(string propertyName, String value)
        {
            PlayerPrefs.SetString(PropKey(propertyName), value);
            PlayerPrefs.SetString(TypeKey(propertyName), StringType);
            PlayerPrefs.Save();
        }

        private string LoadString(string propertyName)
        {
            return PlayerPrefs.GetString(PropKey(propertyName));
        }

#endregion

#region Boolean

        private void SaveBoolean(string propertyName, Boolean value)
        {
            PlayerPrefs.SetInt(PropKey(propertyName), value ? 1 : 0);
            PlayerPrefs.SetString(TypeKey(propertyName), BooleanType);
            PlayerPrefs.Save();
        }

        private bool LoadBoolean(string propertyName)
        {
            return PlayerPrefs.GetInt(PropKey(propertyName)) == 1;
        }

#endregion

#region Char

        private void SaveChar(string propertyName, Char value)
        {
            PlayerPrefs.SetString(PropKey(propertyName), value.ToString());
            PlayerPrefs.SetString(TypeKey(propertyName), CharType);
            PlayerPrefs.Save();
        }

        private char LoadChar(string propertyName)
        {
            return PlayerPrefs.GetString(PropKey(propertyName))[0];
        }

#endregion

#region DateTime

        protected void SaveDateTime(string propertyName, DateTime value)
        {
            string dateTime = value.ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(PropKey(propertyName), dateTime);
            PlayerPrefs.SetString(TypeKey(propertyName), DateTimeType);
            PlayerPrefs.Save();
        }

        protected DateTime LoadDateTime(string propertyName)
        {
            return DateTime.Parse(PlayerPrefs.GetString(PropKey(propertyName)), CultureInfo.InvariantCulture);
        }

#endregion

        protected virtual void Save(string propertyName, object value)
        {
            if (value is Int16 || value is Int32)
            {
                SaveInt(propertyName, (int) value);
            }
            if (value is float)
            {
                SaveFloat(propertyName, (float) value);
            }
            if (value as string != null)
            {
                SaveString(propertyName, (string) value);
            }
            if (value is bool)
            {
                SaveBoolean(propertyName, (bool) value);
            }
            if (value is char)
            {
                SaveChar(propertyName, (char) value);
            }
            if (value is DateTime)
            {
                SaveDateTime(propertyName, (DateTime) value);
            }
        }

        protected virtual object GetValueOrDefault(string propertyName, object defaultValue)
        {
            string type = TypeKey(propertyName);
            if (ContainsPref(propertyName) && !String.IsNullOrEmpty(PlayerPrefs.GetString(type)))
            {
                string typeName = PlayerPrefs.GetString(type);
                if (typeName == IntType)
                {
                    return LoadInt(propertyName);
                }
                if (typeName == FloatType)
                {
                    return LoadFloat(propertyName);
                }
                if (typeName == StringType)
                {
                    return LoadString(propertyName);
                }
                if (typeName == BooleanType)
                {
                    return LoadBoolean(propertyName);
                }
                if (typeName == CharType)
                {
                    return LoadChar(propertyName);
                }
                if (typeName == DateTimeType)
                {
                    return LoadDateTime(propertyName);
                }
                Debug.LogError("GetValueOrDefault: typeName " + typeName + " is not supported.");
                return defaultValue;
            }
            return defaultValue;
        }

        protected bool ContainsPref(string propertyName)
        {
            var type = TypeKey(propertyName);
            return PlayerPrefs.HasKey(PropKey(propertyName)) && PlayerPrefs.HasKey(type);
        }

        protected void DeletePref(string propertyName)
        {
            PlayerPrefs.DeleteKey(PropKey(propertyName));
            PlayerPrefs.DeleteKey(TypeKey(propertyName));
        }

        protected string PropKey(string propertyName)
        {
            return propertyName + ValueSuffix;
        }

        protected string TypeKey(string propertyName)
        {
            var typeKey = "";
            typeKey += propertyName + TypeSuffix;
            return typeKey;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}