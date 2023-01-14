using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Pixelization
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "AngryKoala/Colorizer/ColorPalette")]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] private List<Color> colors = new List<Color>();
        public List<Color> Colors => colors;
    }
}