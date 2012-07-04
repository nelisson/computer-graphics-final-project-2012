namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeJoint
    {
        public byte Flags; //SELECTED/DIRTY
        public char[] Name;
        public char[] ParentName;
        public float[] Rotation;
        public float[] Position;
        public short NumRotKeyFrames;
        public short NumPosKeyFrames;
        public MilkshapeTangent[] Tangents;
        public MilkshapeRotationKey[] RotKeyFrames;
        public MilkshapePositionKey[] PosKeyFrames;

        public string ParentNameS;
        public string NameS;
        public int ParentIndex;
        public MilkshapeMatrix3X4 MatLocalSkeleton;
        public MilkshapeMatrix3X4 MatGlobalSkeleton;

        public MilkshapeMatrix3X4 MatLocal;
        public MilkshapeMatrix3X4 MatGlobal;
    }
}