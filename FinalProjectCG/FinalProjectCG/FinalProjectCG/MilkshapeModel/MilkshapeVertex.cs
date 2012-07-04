namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeVertex
    {
        public short Flags; // SELECTED / SELECTED2 / HIDDEN
        public float[] Vertex;
        public char BoneId;
        public byte ReferenceCount;
        public int[] BoneIDs;
        public int[] BoneWeights;
    }
}