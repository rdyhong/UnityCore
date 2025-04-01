using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RR95
{
    public static class Vector3Ex
    {
        /// <summary>
        /// 해당 축의 값을 0으로 변환합니다.
        /// </summary>
        /// <param name="v"></param>
        public static void FlattenX(this ref Vector3 v)
        {
            v.x = 0;
        }
        public static void FlattenY(this ref Vector3 v)
        {
            v.y = 0;
        }
        public static void FlattenZ(this ref Vector3 v)
        {
            v.z = 0;
        }

        /// <summary>
        /// 길이를 1로 정규화 합니다.
        /// </summary>
        /// <param name="v"></param>
        public static Vector3 Normalized(this Vector3 v)
        {
            v = v.normalized;
            return v;
        }
        public static Vector3 NormalizedIgnoreY(this Vector3 v)
        {
            v.y = 0;
            v = v.normalized;
            return v;
        }
        public static Vector3 NormalizedIgnoreZ(this Vector3 v)
        {
            v.z = 0;
            v = v.normalized;
            return v;
        }

        /// <summary>
        /// 특정 축을 원하는 값으로 설정합니다.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        public static void SetX(this ref Vector3 v, float x)
        {
            v.x = x;
        }

        public static void SetY(this ref Vector3 v, float y)
        {
            v.y = y;
        }

        public static void SetZ(this ref Vector3 v, float z)
        {
            v.z = z;
        }

        /// <summary>
        /// 특정 축에 값을 추가합니다.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        public static void AddX(this ref Vector3 v, float x)
        {
            v.x += x;
        }

        public static void AddY(this ref Vector3 v, float y)
        {
            v.y += y;
        }

        public static void AddZ(this ref Vector3 v, float z)
        {
            v.z += z;
        }
    }
}
