namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeVertex
    {
        public short flags; // SELECTED / SELECTED2 / HIDDEN
        public float[] vertex; // coordinates 3
        public char boneId; // -1=nu este atasat de nici un os
        public byte referenceCount;
        public int[] boneIDs; //pentru influenta multipla pe vertex
        public int[] boneWeights; //gradul de influenta
    }
}