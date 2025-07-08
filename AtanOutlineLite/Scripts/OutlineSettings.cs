using System;
using UnityEngine;

namespace AtanOutlineLite
{
    [Serializable]
    public class OutlineSettings
    {
        public Material outlineMaterial;

        [Space] 
        
        public float normalThreshold = 0.2f;
        public float outlineSize = 3.5f;
        public Color outlineColor = new Color(0f, 0f, 0f, 1f);
    }
}