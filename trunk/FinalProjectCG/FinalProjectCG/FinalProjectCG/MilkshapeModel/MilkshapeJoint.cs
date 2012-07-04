namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeJoint
    {
        public byte flags; //SELECTED/DIRTY
        public char[] name; //numele joint-ului
        public char[] parentName; //numele parintelui ?!?!?!? la sto probabil
        public float[] rotation; //rotatia relativa
        public float[] position; //pozitia relativa
        public short numRotKeyFrames; //numarul de chei de rotatie
        public short numPosKeyFrames; //numarul de chei de pozitiez
        public MilkshapeTangent[] tangents; //tangentele pentru animatie
        public MilkshapeRotationKey[] rotKeyFrames; //cheile de rotatie
        public MilkshapePositionKey[] posKeyFrames;

        //pentru randare
        //
        public string parentNameS;
        public string nameS;
        public int parentIndex;
        public MilkshapeMatrix3x4 matLocalSkeleton;
        public MilkshapeMatrix3x4 matGlobalSkeleton;

        public MilkshapeMatrix3x4 matLocal;
        public MilkshapeMatrix3x4 matGlobal;
    }
}