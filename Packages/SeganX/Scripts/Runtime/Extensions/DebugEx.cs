﻿#if SX_EXDBG
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace SeganX
{
    public static class DebugEx
    {
        [System.Flags]
        public enum MemberType
        {
            Field = 1 << 1,
            Property = 1 << 2
        }

        public static string GetStringDebug(this object self, MemberType memberTypes = MemberType.Field | MemberType.Property, int levels = 3)
        {
            if (self == null) return "[null]";
            System.Type type = self.GetType();

            if (type.IsEnum || type.IsPrimitive || type == typeof(string))
            {
                return "[" + (self.ToString() == "\0" ? "null" : self.ToString()) + "]";
            }
            else if (type.IsArray)
            {
                string res = "{";
                var arr = self as System.Array;
                foreach (var a in arr)
                    res += GetStringDebug(a, memberTypes, levels - 1) + " ";
                return (res.Length > 3 ? res.Remove(res.Length - 1) : res) + "}";
            }
            else if (type.IsGenericType && !type.IsValueType)
            {
                string res = "{";
                var arr = self as ICollection;
                foreach (var a in arr)
                    res += GetStringDebug(a, memberTypes, levels - 1) + " ";
                return (res.Length > 3 ? res.Remove(res.Length - 1) : res) + "}";
            }
            else if (type.IsClass || type.IsValueType)
            {
                if (levels > 0)
                {
                    string res = "{";
                    var members = type.GetMembers();
                    foreach (var member in members)
                    {
                        if (member.MemberType == MemberTypes.Field && (memberTypes & MemberType.Field) != 0)
                        {
                            var field = member as FieldInfo;
                            res += field.Name + GetStringDebug(field.GetValue(self), memberTypes, levels - 1) + " ";
                        }
                        if (member.MemberType == MemberTypes.Property && (memberTypes & MemberType.Property) != 0)
                        {
                            var prop = member as PropertyInfo;
                            if (prop.CanRead)
                                res += prop.Name + GetStringDebug(prop.GetValue(self, null), memberTypes, levels - 1) + " ";
                        }
                    }
                    res = res.Remove(res.Length - 1) + "}";

                    if (res.Length > 100)
                        return "\n" + res;
                    else
                        return res;
                }
                else return "{}";
            }

            return "";
        }

        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DrawBox(bottomBox, color);
            DrawBox(topBox, color);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color);
        }
        public static void DrawBox(Box box, Color color)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }
            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }
}
#endif