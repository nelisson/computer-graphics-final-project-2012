namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeTriangle
    {
        public short flags; //SELECTED / SELECTED2 / HIDDEN
        public short[] vertexIndices; //indicii punctelor din care sunt alcatuite
        public float[][] vertexNormals; //normalele pentru fiecare vertex in parte
        public float[] s; //textura S-coord
        public float[] t; //textura T-coord
        public byte smoothingGroup; //1-32 ????
        public byte groupIndex; //asta nu stiu ce face, dar suna bine
    }
}