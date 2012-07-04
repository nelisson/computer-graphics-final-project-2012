namespace FinalProjectCG.Ninja
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Stateless;
    using Utilities;

    public class Ninja
    {
        public MilkshapeModel.MilkshapeModel Model { get; set; }
        public Dictionary<NinjaAnimation, Tuple<int, int>> AnimationsRange;
        public float Velocity { get; set; }
        public bool IsRunning { get; set; }

        private readonly Dictionary<object, NinjaAnimation> _actions = new Dictionary<object, NinjaAnimation>();

        public StateMachine<NinjaAnimation, NinjaAnimation> Animations = 
            new StateMachine<NinjaAnimation, NinjaAnimation>(NinjaAnimation.Idle2);

        public BasicEffect Effect
        {
            get { return Model.BasicEffect; }
        } 
        
        public Ninja(GraphicsDevice device)
        {
            _actions[Buttons.X] = NinjaAnimation.Overhead;
            _actions[Keys.X] = NinjaAnimation.Overhead;

            Animations.Configure(NinjaAnimation.Idle2).Permit(NinjaAnimation.Overhead, NinjaAnimation.Overhead)
                .OnEntry(() => { Velocity = 1; SetAnimation(NinjaAnimation.Idle2); })
                .PermitReentry(NinjaAnimation.Idle2);
            
            Animations.Configure(NinjaAnimation.Overhead).Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() => { Velocity = 2; SetAnimation(NinjaAnimation.Overhead); });

            Velocity = 1;
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
            IsRunning = false;
            Animations.Fire(NinjaAnimation.Idle2);
        }

        public void SetAnimation(NinjaAnimation animation)
        {
            var tuple = AnimationsRange[animation];

            if(animation != NinjaAnimation.Idle2)
                IsRunning = true;

            Model.setAnimBounds(tuple.Item1, tuple.Item2);
        }

        public void Render(GameTime gameTime)
        {
            Model.Render();
            Model.advanceAnimation((float)gameTime.ElapsedGameTime.TotalMilliseconds / (100/Velocity));   
        }

        public void SetTransformation(Transformation trans)
        {
            Model.MaterialIndex = (short)trans;
        }

        public void Update(GamePadState getState)
        {
            var lista = GamePadUtilities.GetPressed(getState);

            if (lista.Contains(Buttons.X))
            {
                if (!IsRunning)
                {
                    Animations.Fire(_actions[Buttons.X]);
                }
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.X))
            {
                if (!IsRunning)
                {
                    Animations.Fire(_actions[Keys.X]);
                }
            }

            //if ((GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) ||
            //    Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.F))
            //{
            //    if (!IsRunning)
            //    {
            //        Velocity = 2;
            //        SetAnimation(NinjaAnimation.SpinningSword);
            //    }
            //}

            //if (lista.Contains(Buttons.A) ||
            //    Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
            //{
            //    if (!IsRunning)
            //        SetAnimation(NinjaAnimation.Jump);
            //}

            //if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) ||
            //    Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
            //{
            //    if (!IsRunning)
            //    {
            //        Velocity = 2;
            //        SetAnimation(NinjaAnimation.SideKick);
            //    }
            //}

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
            {
                SetTransformation(Transformation.Blue);
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
            {
                SetTransformation(Transformation.Brown);
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
            {
                SetTransformation(Transformation.Green);
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.F))
            {
                SetTransformation(Transformation.White);
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.G))
            {
                SetTransformation(Transformation.Red);
            }
        }
    }
}