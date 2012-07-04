namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeMatrix3x4
    {
        public float[][] v;

        public MilkshapeMatrix3x4()
        {
            v = new float[3][];
            for (int i = 0; i < 3; i++)
            {
                v[i] = new float[4];
            }
        }
    }
}