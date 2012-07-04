namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeGroup
    {
        public byte Flags; //SELECTED/HIDDEN
        public char[] Name;
        public short NumTriangles;
        public short[] TriangleIndices;
        public short MaterialIndex;

        public CustomVertexStruct[] Vertices;
        public CustomVertexStruct[] OriginalVertices;
        public int NumVertices;
    }
}