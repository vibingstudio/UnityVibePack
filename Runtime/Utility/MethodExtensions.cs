using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using VibePack.Utility;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

namespace VibePack.Extensions
{
    public static class MethodExtensions
    {
        private static readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        public static IEnumerator OnEnd(this IEnumerator coroutine, Action callback = null)
        {
            yield return coroutine;
            callback?.Invoke();
        }

        public static IEnumerator Await(this Animator animator, string animationName, Action callback = null)
        {
            animator.Play(animationName);
            yield return null;
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || !animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
            callback?.Invoke();
        }

        public static IEnumerator Await(this Animator animator, Action callback = null)
        {
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
            callback?.Invoke();
        }

        public static Coroutine MoveTo(this MonoBehaviour mono, Vector3 position, float seconds)
        {
            return mono.StartCoroutine(Sequence());

            IEnumerator Sequence()
            {
                Vector3 startingPosition = mono.transform.position;

                for (float secondsElapsed = 0; secondsElapsed < seconds; secondsElapsed += Time.deltaTime)
                {
                    mono.transform.position = Vector3.Lerp(startingPosition, position, secondsElapsed / seconds);
                    yield return null;
                }

                mono.transform.position = startingPosition;
            }
        }

        public static Coroutine ScaleTo(this MonoBehaviour mono, Vector3 scale, float seconds = 0.25f)
        {
            return mono.StartCoroutine(Sequence());

            IEnumerator Sequence()
            {
                Vector3 startingPosition = mono.transform.localScale;

                for (float secondsElapsed = 0; secondsElapsed < seconds; secondsElapsed += Time.deltaTime)
                {
                    mono.transform.localScale = Vector3.Lerp(startingPosition, scale, secondsElapsed / seconds);
                    yield return null;
                }

                mono.transform.localScale = startingPosition;
            }
        }

        public static IEnumerator FadeTo(this SpriteRenderer spriteRenderer, float target, float seconds = 0.25f)
        {
            float start = spriteRenderer.color.a;
            Color col = spriteRenderer.color;

            for (float secondsElapsed = 0; secondsElapsed < seconds; secondsElapsed += Time.deltaTime)
            {
                col.a = Mathf.Lerp(start, target, secondsElapsed / seconds);
                spriteRenderer.color = col;
                yield return null;
            }

            col.a = target;
            spriteRenderer.color = col;
        }

        public static IEnumerator FadeTo(this CanvasGroup canvasGroup, float target, float seconds = 0.25f)
        {
            float start = canvasGroup.alpha;

            for (float secondsElapsed = 0; secondsElapsed < seconds; secondsElapsed += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(start, target, secondsElapsed / seconds);
                yield return null;
            }

            canvasGroup.alpha = target;
        }

        public static IEnumerator ZoomTo(this Camera camera, float zoom, float seconds = 0.25f)
        {
            float startingZoom = camera.orthographicSize;

            for (float secondsElapsed = 0; secondsElapsed < seconds; secondsElapsed += Time.deltaTime)
            {
                camera.orthographicSize = Mathf.Lerp(startingZoom, zoom, secondsElapsed / seconds);
                yield return null;
            }

            camera.orthographicSize = zoom;
        }

        /// <summary>
        /// Returns true if number is within range, false otherwise.
        /// Min is inclusive, max is exclusive.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min">Inclusive</param>
        /// <param name="max">Exclusive</param>
        /// <returns></returns>
        public static bool InRange(this int number, float min, float max) => number >= min && number < max;

        /// <summary>
        /// Returns true if number is within range, false otherwise.
        /// Min is inclusive, max is exclusive.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min">Inclusive</param>
        /// <param name="max">Exclusive</param>
        /// <returns></returns>
        public static bool InRange(this float number, float min, float max) => number >= min && number < max;

        /// <summary>
        /// Normalizes the given number between the given range.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Normalize(this float number, float min, float max) => number < min ? max - (Mathf.Abs(number) % max) : number % max;

        public static float Normalize(this int number, int min, int max) => number < min ? max - (Mathf.Abs(number) % max) : number % max;

        //TODO: Make this function better
        public static int Wrap(this int number, int min, int max) => number < min ? max : number > max ? min : number;

        public static object Invoke(this object p_target, string p_method, params object[] p_args)
        {
            if (p_target.GetType().GetMethod(p_method, flags) is not null and MethodInfo mi && mi is not null)
                return mi.Invoke(p_target, p_args);
            return null;
        }

        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
                arr[a] = arr[a + 1];

            Array.Resize(ref arr, arr.Length - 1);
        }

        public static T GetRandom<T>(this IList<T> list) => list[Random.Range(0, list.Count)];

        public static T GetRandom<T>(this IEnumerable<T> list) => list.ElementAt(Random.Range(0, list.Count()));

        public static T GetRandom<T>(this IList<T> list, out int index) => list[index = Random.Range(0, list.Count)];

        public static T GetRandom<T>(this IEnumerable<T> list, out int index) => list.ElementAt(Random.Range(0, index = list.Count()));

        public static IEnumerator Await<T>(this IList<T> list)
        {
            foreach (T c in list)
                yield return c;
        }

        public static IEnumerator Add(this IEnumerator lhs, params IEnumerator[] rhs)
        {
            yield return lhs;
            foreach (IEnumerator item in rhs)
                yield return item;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            if (n == 2)
            {
                T temp = list[0];
                list[0] = list[1];
                list[1] = temp;
                return;
            }

            while (n > 1)
            {
                n--;
                int k = new System.Random().Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static void Shuffle(this Transform transform)
        {
            List<Transform> childs = new List<Transform>();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                childs.Add(child);
                child.SetParent(null);
            }

            childs.Shuffle();

            foreach (Transform child in childs)
                child.SetParent(transform);
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                GameObject.Destroy(child.gameObject);
        }

        public static void ClearImmediateChildren(this Transform transform)
        {
            for(int i = transform.childCount - 1; i >= 0; i--)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }

        public static IList<T> GetChildren<T>(this Transform transform)
        {
            List<T> children = new List<T>();

            for (int i = 0; i < transform.childCount; i++)
                children.Add(transform.GetChild(i).GetComponent<T>());

            return children;
        }

        public static Coroutine GoTo(this TextMeshProUGUI text, int value, int target)
        {
            IEnumerator LerpText()
            {
                int difference = target - value;
                int speed = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(difference) / 30f));

                if (difference > 0)
                {
                    while (value < target)
                    {
                        value += speed;
                        text.text = value.ToString();
                        yield return new WaitForFrames(5 - speed);
                    }
                }
                else
                {
                    while (value > target)
                    {
                        value -= speed;
                        text.text = value.ToString();
                        yield return new WaitForFrames(5 - speed);
                    }
                }

                text.text = target.ToString();
            }

            return text.StartCoroutine(LerpText());
        }

        public static Coroutine GoTo(this TextMeshPro text, int value, int target)
        {
            IEnumerator LerpText()
            {
                int difference = target - value;
                int speed = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(difference) / 30f));

                if (difference > 0)
                {
                    while (value < target)
                    {
                        value += speed;
                        text.text = value.ToString();
                        yield return new WaitForFrames(5 - speed);
                    }
                }
                else
                {
                    while (value > target)
                    {
                        value -= speed;
                        text.text = value.ToString();
                        yield return new WaitForFrames(5 - speed);
                    }
                }

                text.text = target.ToString();
            }

            return text.StartCoroutine(LerpText());
        }

        public static Texture2D DrawCircle(this Texture2D tex, Color color, int x, int y, int radius = 3)
        {
            float rSquared = radius * radius;

            for (int u = x - radius; u < x + radius + 1; u++)
                for (int v = y - radius; v < y + radius + 1; v++)
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                        tex.SetPixel(u, v, color);

            return tex;
        }

        public static Vector2Int? GetUV(this SpriteRenderer renderer, Vector2 position)
        {
            if (!renderer.bounds.Contains(position))
                return null;

            Vector2 localPos = position - renderer.transform.position.xy();
            localPos += Vector2.right * renderer.bounds.extents.x;
            Vector2 textureSize = new Vector2Int(renderer.sprite.texture.width, renderer.sprite.texture.height);
            return (localPos / renderer.bounds.size * textureSize).Round();
        }

        public static Vector3 Rotate(this Vector3 v, float theta)
        {
            float newTheta = Mathf.Atan2(v.y , v.x) + theta;
            return new Vector3(Mathf.Cos(newTheta), Mathf.Sin(newTheta)) * v.magnitude;
        }

        public static Vector2 Rotate(this Vector2 v, float theta)
        {
            float newTheta = Mathf.Atan2(v.y , v.x) + theta;
            return new Vector2(Mathf.Cos(newTheta), Mathf.Sin(newTheta)) * v.magnitude;
        }

        public static Vector2Int Round(this Vector2 v) => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        public static Vector2Int Ceil(this Vector2 v) => new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

        public static Vector2Int Floor(this Vector2 v) => new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

        public static float GetRandom(this ValueRange<float> range) => Random.Range(range.min, range.max);

        public static int GetRandom(this ValueRange<int> range) => Random.Range(range.min, range.max);

        public static Vector2 GetRandom(this ValueRange<Vector2> range) => new Vector2(Random.Range(range.min.x, range.max.x), Random.Range(range.min.y, range.max.y));

        public static Vector3 GetRandom(this ValueRange<Vector3> range) => new Vector3(Random.Range(range.min.x, range.max.x), Random.Range(range.min.y, range.max.y), Random.Range(range.min.z, range.max.z));

        public static Transform GetRandomChild(this Transform transform) => transform.GetChild(Random.Range(0, transform.childCount));
    }
}
