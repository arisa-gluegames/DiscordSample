using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// This class allows us to start Coroutines from non-Monobehaviour classes
/// </summary>
namespace GlueGames.Utilities
{
    public class CoroutineHandler : SingletonPersistent<CoroutineHandler>
    {
        public CancellationToken DestroyCancellationToken => destroyCancellationToken;

        public static Coroutine StartStaticCoroutine(IEnumerator coroutine, System.Action onComplete = null)
        {
            Coroutine startedCoroutine = Instance.StartCoroutine(coroutine);
            return onComplete == null ?
                startedCoroutine :
                Instance.StartCoroutine(CallbackOnComplete(startedCoroutine, onComplete));
        }

        public static void StopStaticCoroutine(IEnumerator coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static void StopStaticCoroutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static IEnumerator CallbackOnComplete(Coroutine coroutine, System.Action onComplete)
        {
            yield return coroutine;
            onComplete?.Invoke();
        }

        public static IEnumerator CallbackOnComplete(IEnumerator coroutine, System.Action onComplete)
        {
            return CallbackOnComplete(StartStaticCoroutine(coroutine), onComplete);
        }
    }

}
