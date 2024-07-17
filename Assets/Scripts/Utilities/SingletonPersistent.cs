using UnityEngine;

namespace GlueGames.Utilities
{
    /// <summary>
    /// Persistent singleton.
    /// </summary>
    public class SingletonPersistent<T> : Singleton<T> where T : Component
    {
        /// <summary>
	    /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
	    /// </summary>
	    protected override void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
        }
    }

}
