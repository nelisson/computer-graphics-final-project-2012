namespace FinalProjectCG.Ninja
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Ninja
    {
        public MilkshapeModel.MilkshapeModel Model { get; set; }

        public Dictionary<NinjaAnimation, Tuple<int, int>> AnimationsRange;

        public BasicEffect Effect
        {
            get { return Model.BasicEffect; }
        } 

        public Ninja(GraphicsDevice device)
        {
            AnimationsRange = new Dictionary<NinjaAnimation, Tuple<int, int>>
                                  {
                                      {NinjaAnimation.WalkNormal, new Tuple<int, int>(1, 14)},
                                      {NinjaAnimation.StealthWalk, new Tuple<int, int>(15, 31)},
                                      {NinjaAnimation.PunchSwipeSword, new Tuple<int, int>(32, 43)},
                                      {NinjaAnimation.SwipeSpinSword, new Tuple<int, int>(44, 59)},
                                      {NinjaAnimation.Overhead, new Tuple<int, int>(59, 68)},
                                      {NinjaAnimation.Block, new Tuple<int, int>(69, 72)},
                                      {NinjaAnimation.ForwardKick, new Tuple<int, int>(73, 83)},
                                      {NinjaAnimation.PickUpFloor, new Tuple<int, int>(84, 92)},
                                      {NinjaAnimation.Jump, new Tuple<int, int>(93, 102)},
                                      {NinjaAnimation.JumpWithoutHeight, new Tuple<int, int>(103, 110)},
                                      {NinjaAnimation.HighJumpKill, new Tuple<int, int>(111, 125)},
                                      {NinjaAnimation.SideKick, new Tuple<int, int>(125, 133)},
                                      {NinjaAnimation.SpinningSword, new Tuple<int, int>(134, 145)},
                                      {NinjaAnimation.Backflip, new Tuple<int, int>(146, 158)},
                                      {NinjaAnimation.Climbwall, new Tuple<int, int>(159, 165)},
                                      {NinjaAnimation.Death1, new Tuple<int, int>(166, 174)},
                                      {NinjaAnimation.Death2, new Tuple<int, int>(175, 183)},
                                      {NinjaAnimation.Idle1, new Tuple<int, int>(184, 204)},
                                      {NinjaAnimation.Idle2, new Tuple<int, int>(205, 250)},
                                      {NinjaAnimation.Idle3, new Tuple<int, int>(251, 300)}
                                  };

            Model = new MilkshapeModel.MilkshapeModel("Ninja\\ninja.ms3d", device);

            Model.StoppedAnimation += ModelOnStoppedAnimation;
        }

        private void ModelOnStoppedAnimation()
        {
            SetAnimation(NinjaAnimation.Idle2);
        }

        public void SetAnimation(NinjaAnimation animation)
        {
            var tuple = AnimationsRange[animation];

            Model.setAnimBounds(tuple.Item1, tuple.Item2);
        }

        public void Render(GameTime gameTime)
        {
            Model.Render();
            Model.advanceAnimation((float)gameTime.ElapsedGameTime.TotalMilliseconds / 100);   
        }
    }
}