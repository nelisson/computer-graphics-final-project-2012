namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeMatrix3X4
    {
        public float[][] V;

        public MilkshapeMatrix3X4()
        {
            V = new float[3][];
            for (var i = 0; i < 3; i++)
            {
                V[i] = new float[4];
            }
        }
    }
}