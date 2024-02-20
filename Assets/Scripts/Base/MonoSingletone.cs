using UnityEngine;

public class MonoSingletone : MonoBehaviour
{
    public static MonoSingletone instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
