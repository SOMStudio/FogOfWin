using System;
using UnityEngine;

namespace Base
{
    public class MonoSingleton<T> : MonoBehaviour where T : class
    {
        protected static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this as T) instance = null;
        }
    }
}
