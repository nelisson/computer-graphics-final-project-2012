namespace FinalProjectCG.MilkshapeModel
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public struct CustomVertexStruct
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord1;
        public Vector2 TexCoord2;
        public Vector4 BoneIDs;
        public Vector4 BoneWeights;
        public int BoneId;
        public static VertexElement[] VertexElements = {
                                                           new VertexElement(0, VertexElementFormat.Vector3,
                                                                             VertexElementUsage.Position, 0),
                                                           new VertexElement(sizeof (float)*3,
                                                                             VertexElementFormat.Vector3,
                                                                             VertexElementUsage.Normal, 0),
                                                           new VertexElement(sizeof (float)*6,
                                                                             VertexElementFormat.Vector2,
                                                                             VertexElementUsage.TextureCoordinate, 0),
                                                           new VertexElement(sizeof (float)*8,
                                                                             VertexElementFormat.Vector2,
                                                                             VertexElementUsage.TextureCoordinate, 1),
                                                           new VertexElement(sizeof (float)*10,
                                                                             VertexElementFormat.Vector4,
                                                                             VertexElementUsage.BlendIndices, 0),
                                                           new VertexElement(sizeof (float)*14,
                                                                             VertexElementFormat.Vector4,
                                                                             VertexElementUsage.BlendWeight, 0),
                                                           new VertexElement(sizeof (float)*18,
                                                                             VertexElementFormat.Short4,
                                                                             VertexElementUsage.BlendIndices, 1)
                                                       };
    }
}