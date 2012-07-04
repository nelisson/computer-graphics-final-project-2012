namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeJointAndWeight
    {
        public int[] jointIndices;
        public int[] jointWeights;

        public MilkshapeJointAndWeight()
        {
            jointIndices = new int[4];
            jointWeights = new int[4];
        }
    }
}