using System;
using Base;
using UnityEngine;

namespace Save
{
    public class SaveManager : MonoSingleton<SaveManager>, ISaveManager
    {
        public event Action<string> ChangeValueEvent;

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(this);
        }

        public void SetValue(string nameValue, float value)
        {
            PlayerPrefs.SetFloat(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public float GetValue(string nameValue, float value = 0f)
        {
            if (HasValue(nameValue))
                return PlayerPrefs.GetFloat(nameValue);
            else
            {
                PlayerPrefs.SetFloat(nameValue, value);
                return GetValueFloat(nameValue);
            }
        }

        public float GetValueFloat(string nameValue)
        {
            return PlayerPrefs.GetFloat(nameValue);
        }

        public void SetValue(string nameValue, int value)
        {
            PlayerPrefs.SetInt(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public int GetValue(string nameValue, int value = 0)
        {
            if (HasValue(nameValue))
                return PlayerPrefs.GetInt(nameValue);
            else
            {
                PlayerPrefs.SetInt(nameValue, value);
                return GetValueInt(nameValue);
            }
        }

        public int GetValueInt(string nameValue)
        {
            return PlayerPrefs.GetInt(nameValue);
        }

        public void SetValue(string nameValue, string value)
        {
            PlayerPrefs.SetString(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public string GetValue(string nameValue, string value = "")
        {
            if (HasValue(nameValue))
                return PlayerPrefs.GetString(nameValue);
            else
            {
                PlayerPrefs.SetString(nameValue, value);
                return GetValueString(nameValue);
            }
        }

        public string GetValueString(string nameValue)
        {
            return PlayerPrefs.GetString(nameValue);
        }

        public bool HasValue(string nameValue)
        {
            return PlayerPrefs.HasKey(nameValue);
        }
    }
    
    public interface ISaveManager
    {
        event Action<string> ChangeValueEvent;

        void SetValue(string nameValue, float value);
        float GetValue(string nameValue, float value = 0f);
        float GetValueFloat(string nameValue);
        void SetValue(string nameValue, int value);
        int GetValue(string nameValue, int value = 0);
        int GetValueInt(string nameValue);
        void SetValue(string nameValue, string value);
        string GetValue(string nameValue, string value = "");
        string GetValueString(string nameValue);
        bool HasValue(string nameValue);
    }
}