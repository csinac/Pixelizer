using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Pixelization {
    public static class ColorOp
    {
        public static Vector3 Add(Color a, Color b)
        {
            return new Vector3
            (
                Mathf.Clamp(a.r + b.r, 0, 1),
                Mathf.Clamp(a.g + b.g, 0, 1),
                Mathf.Clamp(a.b + b.b, 0, 1)
            );
        }

        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return new Vector3
            (
                Mathf.Clamp(a.x + b.x, 0, 1),
                Mathf.Clamp(a.y + b.y, 0, 1),
                Mathf.Clamp(a.z + b.z, 0, 1)
            );
        }

        public static Vector3 Subtract(Color a, Color b)
        {
            return new Vector3(a.r - b.r, a.g - b.g, a.b - b.b);
        }

        public static Vector3 Multiply(Color a, float b)
        {
            return new Vector3 (a.r * b, a.g * b, a.b * b);
        }

        public static Vector3 ColorToVector(Color color)
        {
            return new Vector3 (color.r, color.g, color.b );
        }

        public static Color VectorToColor(Vector3 value)
        {
            return new Color (value.x, value.y, value.z, 1);
        }
    }
}
