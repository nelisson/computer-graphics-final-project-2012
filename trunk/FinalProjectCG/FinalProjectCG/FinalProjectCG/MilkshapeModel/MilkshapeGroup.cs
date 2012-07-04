namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeGroup
    {
        public byte Flags; //SELECTED/HIDDEN
        public char[] Name; //denumirea grupului
        public short NumTriangles; //numarul de triunghiuri asociate
        public short[] TriangleIndices;
        public short MaterialIndex;

        public CustomVertexStruct[] Vertices; //aici stocam vertexurile 
        public CustomVertexStruct[] OriginalVertices; //aici stocam vertexurile 
        public int NumVertices; //numarul de puncte
    }
}