using System.Collections;
using VibePack.Math;
using UnityEngine;

namespace VibePack.Utility
{
    public class SquashAndStretch : MonoBehaviour
    {
        Coroutine stretchCoroutine;
        Vector3 originalSize;

        private IEnumerator Squash(Vector3 size, float seconds, bool scaling)
        {
            yield return null;
            Vector3 originalScale = transform.localScale;

            if (scaling)
                size = new Vector3(size.x * originalScale.x, size.y * originalScale.y, size.z * originalScale.z);

            for (float time = 0; time < seconds; time += Time.fixedDeltaTime)
            {
                transform.localScale = Mathv.Eerp(originalScale, size, Mathf.Sin(time / seconds * Mathf.PI));
                yield return new WaitForFixedUpdate();
            }

            transform.localScale = originalScale;
            stretchCoroutine = null;
        }

        public Coroutine Stretch(Vector3 size, float seconds, bool scaling = false)
        {
            if (stretchCoroutine != null)
            {
                transform.localScale = originalSize;
                StopCoroutine(stretchCoroutine);
            }

            originalSize = transform.localScale;
            return stretchCoroutine = StartCoroutine(Squash(size, seconds, scaling));
        }

        public static implicit operator Coroutine(SquashAndStretch d) => d.stretchCoroutine;
    }
}