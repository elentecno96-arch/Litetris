using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isQuitting;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name + " (Persistent)");
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            _isQuitting = false;
            if (_instance == null)
            {
                _instance = this as T;
                if (transform.parent == null) DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this) Destroy(gameObject);
        }
        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            // 인스턴스가 나 자신이라면 null로 밀어줌 (메모리 정리)
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
