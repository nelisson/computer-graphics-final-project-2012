namespace FinalProjectCG.MilkshapeModel
{
    using Microsoft.Xna.Framework.Graphics;

    public class MilkshapeMaterial
    {
        public char[] name; //denumirea materialului
        public float[] ambient; //lumina ambienta
        public float[] diffuse; //lumina difuza
        public float[] specular; //lumina speculara
        public float[] emissive; //lumina emisiva
        public float shininess; //0.0f - 128.0f stralucirea
        public float transparency; //0.0f - 1.0f	transparenta
        public char mode; //modul??????
        public byte[] textureFN; //fisierul textura
        public byte[] spheremapFN; //fisierul alphamap - folosit in cadrul grizle pentru reflexie(spheremap);
        public string textureS; //denumirea string
        public string sphereMapS; //denumirea string
        //implementarea DirectX
        //public Grap material;				//materialul DirectX
        //public Material material;
        public Texture texture; //textura principala
        public Texture spheremap; //reflexia simulata
    }
}