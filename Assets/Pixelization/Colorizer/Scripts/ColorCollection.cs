using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CreateAssetMenu(fileName = "ColorCollection", menuName = "AngryKoala/Colorizer/ColorCollection")]
    public class ColorCollection : ScriptableObject
    {
        [SerializeField] private List<Color> colors = new List<Color>();
        public List<Color> Colors => colors;
    }
}