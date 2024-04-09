using FishNet.Object;
using UnityEngine;

namespace MarkUlrich.Utils
{
    public abstract class NetworkedSingletonInstance<T> : NetworkBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {            
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                    return _instance;
                
                GameObject container = new(typeof(T).Name);
                _instance = container.AddComponent<T>();

                return _instance;
            }
        }

        protected virtual void OnEnable() => InitSingletonInstance();

        /// <summary>
        /// Initialises the Singleton Instance.
        /// </summary>
        /// <returns>The Singleton Instance.</returns>
        protected virtual T InitSingletonInstance()
        {
            transform.parent = transform.root;

            if (Instance != this)
                Destroy(gameObject);

            return Instance;
        }
    }
}
