using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;

    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                MyDebug.LogWarning("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_Instance == null)
                {
                    _Instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        MyDebug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                        return _Instance;
                    }

                    if (_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _Instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        MyDebug.Log("[Singleton] An instance of " + typeof(T) +
                        " is needed in the scene, so '" + singleton +
                        "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        MyDebug.Log("[Singleton] Using instance already created: " + _Instance.gameObject.name);
                    }
                }

                return _Instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}