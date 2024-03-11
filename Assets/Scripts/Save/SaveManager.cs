using System;
using Base;
using UnityEngine;

namespace Save
{
    public class SaveManager : MonoSingleton<SaveManager>, ISaveManager
    {
        public event Action<string> ChangeValueEvent;
        
        public void SetValue(string nameValue, float value)
        {
            PlayerPrefs.SetFloat(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public float GetValue(string nameValue, float value = 0f)
        {
            return HasValue(nameValue) ? PlayerPrefs.GetFloat(nameValue) : value;
        }
        
        public void SetValue(string nameValue, int value)
        {
            PlayerPrefs.SetInt(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public int GetValue(string nameValue, int value = 0)
        {
            return HasValue(nameValue) ? PlayerPrefs.GetInt(nameValue) : value;
        }
        
        public void SetValue(string nameValue, string value)
        {
            PlayerPrefs.SetString(nameValue, value);
            
            ChangeValueEvent?.Invoke(nameValue);
        }

        public string GetValue(string nameValue, string value = "")
        {
            return HasValue(nameValue) ? PlayerPrefs.GetString(nameValue) : value;
        }

        public bool HasValue(string nameValue)
        {
            return PlayerPrefs.HasKey(nameValue);
        }
    }
}
