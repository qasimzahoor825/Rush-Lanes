using UnityEngine;

namespace RushLanes
{
    public static class MaterialFactory
    {
        static Shader _lit;

        public static Material ColorMat(Color color, bool emission = false)
        {
            var mat = new Material(FindLit());
            mat.color = color;
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (emission && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 1.4f);
            }
            return mat;
        }

        static Shader FindLit()
        {
            if (_lit != null)
                return _lit;

            _lit = Shader.Find("Universal Render Pipeline/Lit")
                   ?? Shader.Find("Standard")
                   ?? Shader.Find("Mobile/Diffuse")
                   ?? Shader.Find("Unlit/Color")
                   ?? Shader.Find("Sprites/Default")
                   ?? Shader.Find("Hidden/InternalErrorShader");
            return _lit;
        }
    }
}
