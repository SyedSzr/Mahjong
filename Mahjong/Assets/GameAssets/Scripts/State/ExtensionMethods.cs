using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Extensions
{
    public static class ExtensionMethods
    {
        //private static string[] suffix = new string[] { "", "k", "M", "G", "T", "P", "E", "Z", "Y" };
        private static string[] suffix = new string[]
        { "", "k", "m", "b", "t", //e12
            "aa", "ab", "ac", "ad", "ae", "af", //e30
            "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", //e60
            "aq","ar","as","at","au","av","aw","ax","ay","az","ba","bb", //e90
            "bc","bd","be","bf","bg","bh","bi","bj","bk","bl","bm","bn", //e120
            "bo","bp","bq","br","bs","bt","bu","bv","bw","bx","by","bz", //e150

            "ca","cb","cc","cd","ce","cf","cg","ch","ci","cj","ck","cl", //e180
            "cm","cn","co","cp","cq","cr","cs","ct","cu","cv","cw","cx", //e210
            "cy", "cz",//e216

             "da","db","dc","dd","de","df","dg","dh","di","dj","dk","dl", //e246
            "dm","dn","do","dp","dq","dr","ds","dt","du","dv","dw","dx", //e276
            "dy", "dz", //e282

             "ea","eb","ec","ed","ee","ef","eg","eh","ei","ej","ek","el" //e312
        };


        public static string ToIncomeString(this double Value)
        {
            //return Value.ToString();
            return CurrencyToString(Value);

        }

        public static string ToPercentageString(this double Value)
        {
            return Value * 100D + "%";
        }

        public static string ToIncomeString(this int Value)
        {
            return CurrencyToString(Value);
        }

        public static string CurrencyToString(double valueToConvert)
        {
            int scale = 0;
            double v = valueToConvert;
            while (v >= 1000d)
            {
                v /= 1000d;
                scale++;
                if (scale >= suffix.Length)
                    return valueToConvert.ToString("e2"); // overflow, can't display number, fallback to exponential
            }
            return v.ToString("0.###") + suffix[scale];
        }

        public static string ToTimeString(this double Value)
        {
            return TimeSpan.FromSeconds(Value).ToReadableTimerString();
        }

        public static string ToFullSuffixTimerString(this double Value)
        {
            return TimeSpan.FromSeconds(Value).ToFullSuffixReadableTimerString();
        }


        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime.ToUniversalTime());
            return dateTimeOffset.ToUnixTimeSeconds();
        }

        public static string ToReadableTimerString(this TimeSpan Span)
        {
            string Formatted = string.Format("{0}{1}{2}{3}",
                Span.Days > 0 ? string.Format("{0:0}d ", Span.Days) : string.Empty,
                Span.Hours > 0 ? string.Format("{0:0}h ", Span.Hours) : string.Empty,
                Span.Minutes > 0 ? string.Format("{0:0}m ", Span.Minutes) : string.Empty,
                Span.Seconds > 0 ? string.Format("{0:0}s ", Span.Seconds) : string.Empty
            );

            if (string.IsNullOrEmpty(Formatted))
            {
                Formatted = "0s";
            }
            else
            {
                Formatted = Formatted.Substring(0, Formatted.Length - 1);
            }

            return Formatted;
        }

        public static string ToFullSuffixReadableTimerString(this TimeSpan Span)
        {
            string Formatted = string.Format("{0}{1}{2}{3}",
                Span.Days > 0 ? string.Format("{0:0} days ", Span.Days) : string.Empty,
                Span.Hours > 0 ? string.Format("{0:0} hours ", Span.Hours) : string.Empty,
                Span.Minutes > 0 ? string.Format("{0:0} minutes ", Span.Minutes) : string.Empty,
                Span.Seconds > 0 ? string.Format("{0:0} seconds ", Span.Seconds) : string.Empty
            );

            if (string.IsNullOrEmpty(Formatted))
            {
                Formatted = "0s";
            }
            else
            {
                Formatted = Formatted.Substring(0, Formatted.Length - 1);
            }

            return Formatted;
        }

        public static Vector3 ToAll(float Value)
        {
            return new Vector3(Value, Value, Value);

        }
        public static void Trigger(this Action Callback)
        {
            if (Callback != null)
            {
                Callback.Invoke();
            }
        }

        public static void Trigger(this UnityEvent Event)
        {
            if (Event != null)
            {
                Event.Invoke();
            }
        }

        public static void ResetTransform(this Transform Transform)
        {
            if (Transform != null)
            {
                Transform.position = Vector3.zero;
                Transform.eulerAngles = Vector3.zero;
                Transform.localScale = Vector3.one;
            }
        }

        public static void ResetLocalTransform(this Transform Transform)
        {
            if (Transform != null)
            {
                Transform.localPosition = Vector3.zero;
                Transform.localEulerAngles = Vector3.zero;
                Transform.localScale = Vector3.one;
            }
        }

        //public static SafeVector3 ToSafeVector3(this Vector3 Vector3)
        //{
        //    var SafeVector3 = new SafeVector3(Vector3);
        //    return SafeVector3;
        //}

        //public static Vector3 ToVector3(this SafeVector3 SafeVector3)
        //{
        //    Vector3 Vector3 = new Vector3(SafeVector3.X, SafeVector3.Y, SafeVector3.Z);
        //    return Vector3;
        //}

        public static void SetActiveOptimized(this GameObject GameObject, bool Status)
        {
            if (GameObject != null)
            {
                GameObject.SetActive(Status);
            }
        }

        public static void SetActiveOptimized(this List<GameObject> GameObjects, bool Status)
        {
            if (GameObjects != null)
            {
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObjects[i].SetActive(Status);
                }
            }
        }

        public static void SetActiveOptimzed<T>(this List<T> Items, bool Status)
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i] as GameObject;
                    Item.SetActive(Status);
                }
            }
        }

        public static T GetRandom<T>(this IEnumerable<T> Items)
        {
            List<T> Collection = Items as List<T> ?? Items.ToList();

            if (Items != null && Collection.Count > 0)
            {
                var Length = Collection.Count;
                int Random = UnityEngine.Random.Range(0, Length);
                var Result = Collection[Random];
                return Result;

            }
            else
            {
                return default(T);
            }

        }

        public static List<T> Randomize<T>(this IEnumerable<T> Items)
        {
            List<T> Collection = Items as List<T> ?? Items.ToList();

            if (Items != null && Collection.Count > 0)
            {
                var Length = Collection.Count;
                List<int> CountList = new List<int>();
                for (int i = 0; i < Length; i++)
                {
                    CountList.Add(i);
                }

                List<T> RandomizeList = new List<T>();
                int RandomValue = 0;
                for (int i = 0; i < Length; i++)
                {
                    RandomValue = UnityEngine.Random.Range(0, CountList.Count);
                    RandomizeList.Add(Collection[CountList[RandomValue]]);
                    CountList.RemoveAt(RandomValue);
                }
                return RandomizeList;

            }
            else
            {
                return new List<T>();
                //return default(List<T>);
            }

        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T retreivedComp = obj.GetComponent<T>();

            if (retreivedComp != null)
                return retreivedComp;

            return obj.AddComponent<T>();
        }

        public static T GetComponentInSibling<T>(this GameObject obj) where T : Component
        {
            var Root = obj.transform.root.gameObject;

            T retreivedComp = Root.GetComponent<T>();

            if (retreivedComp != null)
                return retreivedComp;

            return obj.AddComponent<T>();
        }

        public static List<T> GetComponentsInSibling<T>(this GameObject obj) where T : Component
        {
            var Root = obj.transform.root.gameObject;

            List<T> retreivedComp = Root.GetComponentsInChildren<T>().ToList();

            if (retreivedComp != null)
            {
                return retreivedComp;
            }
            else
            {
                return new List<T>();
            }
        }

        public static void CancelForces(this Rigidbody2D Rigidbody2D)
        {
            Rigidbody2D.isKinematic = true;
            Rigidbody2D.velocity = Vector2.zero;
            Rigidbody2D.angularVelocity = 0;
        }

        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static int GetLayerMask(this GameObject GameObject)
        {
            return 1 << GameObject.layer;
        }

        public static bool ValidateLayerMask(this GameObject GameObject, LayerMask LayerMask)
        {
            return LayerMask == (LayerMask | (1 << GameObject.layer));
        }

        public static bool IsNullOrEmpty<T>(this List<T> List)
        {
            if (List == null || List.Count <= 0)
                return true;

            return false;
        }

        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            return result;
        }
    }
}


