namespace FinalProjectCG.MilkshapeModel
{
    using Microsoft.Xna.Framework.Graphics;

    public class MilkshapeMaterial
    {
        public char[] Name;
        public float[] Ambient;
        public float[] Diffuse;
        public float[] Specular;
        public float[] Emissive;
        public float Shininess;
        public float Transparency;
        public char Mode;
        public byte[] TextureFN;
        public byte[] SpheremapFN;
        public string TextureS;
        public string SphereMapS;
        public Texture Texture;
        public Texture Spheremap;
    }
}