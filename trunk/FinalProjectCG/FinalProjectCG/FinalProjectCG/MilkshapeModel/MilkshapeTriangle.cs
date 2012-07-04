namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeTriangle
    {
        public short Flags; //SELECTED / SELECTED2 / HIDDEN
        public short[] VertexIndices;
        public float[][] VertexNormals;
        public float[] S;
        public float[] T;
        public byte SmoothingGroup;
        public byte GroupIndex;
    }
}