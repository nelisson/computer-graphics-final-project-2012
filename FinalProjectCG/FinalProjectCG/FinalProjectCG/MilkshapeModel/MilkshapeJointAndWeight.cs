namespace FinalProjectCG.MilkshapeModel
{
    public class MilkshapeJointAndWeight
    {
        public int[] JointIndices;
        public int[] JointWeights;

        public MilkshapeJointAndWeight()
        {
            JointIndices = new int[4];
            JointWeights = new int[4];
        }
    }
}