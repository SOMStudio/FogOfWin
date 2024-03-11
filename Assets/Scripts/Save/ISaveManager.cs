using System;

namespace Save
{
    public interface ISaveManager
    {
        event Action<string> ChangeValueEvent;

        void SetValue(string nameValue, float value);
        float GetValue(string nameValue, float value = 0f);
        void SetValue(string nameValue, int value);
        int GetValue(string nameValue, int value = 0);
        void SetValue(string nameValue, string value);
        string GetValue(string nameValue, string value = "");
        bool HasValue(string nameValue);
    }
}