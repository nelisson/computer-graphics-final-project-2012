namespace FinalProjectCG.Ninja
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MilkshapeModel;
    using Stateless;
    using Utilities;

    public class Ninja
    {
        private readonly Dictionary<object, NinjaAnimation> _actions = new Dictionary<object, NinjaAnimation>();

        public StateMachine<NinjaAnimation, NinjaAnimation> Animations =
            new StateMachine<NinjaAnimation, NinjaAnimation>(NinjaAnimation.Idle2);

        public Dictionary<NinjaAnimation, Tuple<int, int>> AnimationsRange;
        private int _currentHealth;
        private bool _walking;

        public Vector3 Position = Vector3.Zero;
        public float Rotation;

        public Ninja(GraphicsDevice device)
        {
            CurrentHealth = 100;
            ConfigureActions();
            ConfigureStateMachine();
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
                                      {NinjaAnimation.Death1, new Tuple<int, int>(166, 173)},
                                      {NinjaAnimation.Death2, new Tuple<int, int>(175, 183)},
                                      {NinjaAnimation.Idle1, new Tuple<int, int>(184, 204)},
                                      {NinjaAnimation.Idle2, new Tuple<int, int>(205, 250)},
                                      {NinjaAnimation.Idle3, new Tuple<int, int>(251, 300)},
                                      {NinjaAnimation.GroundFront, new Tuple<int, int>(182, 182)},
                                      {NinjaAnimation.GroundBack, new Tuple<int, int>(173, 173)},
                                      {NinjaAnimation.Blocked, new Tuple<int, int>(72, 72)},
                                      {NinjaAnimation.ReverseBlock, new Tuple<int, int>(69, 72)}
                                  };

            Model = new MilkshapeModel("Ninja\\ninja.ms3d", device);

            Model.StoppedAnimation += ModelOnStoppedAnimation;
        }

        public MilkshapeModel Model { get; set; }

        public float Velocity { get; set; }
        public bool IsRunning { get; set; }

        public bool IsAlive
        {
            get { return CurrentHealth > 0; }
        }

        public bool Blocked { get; set; }

        public int CurrentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = (int) MathHelper.Clamp(value, 0, 100); }
        }

        public BasicEffect Effect
        {
            get { return Model.BasicEffect; }
        }

        private void ConfigureStateMachine()
        {
            Animations.Configure(NinjaAnimation.Idle2)
                .Permit(NinjaAnimation.Overhead, NinjaAnimation.Overhead)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.Idle2);
                             })
                .Permit(NinjaAnimation.Death1, NinjaAnimation.Death1)
                .Permit(NinjaAnimation.ForwardKick, NinjaAnimation.ForwardKick)
                .Permit(NinjaAnimation.SideKick, NinjaAnimation.SideKick)
                .Permit(NinjaAnimation.SpinningSword, NinjaAnimation.SpinningSword)
                .Permit(NinjaAnimation.Block, NinjaAnimation.Block)
                .Permit(NinjaAnimation.WalkNormal, NinjaAnimation.WalkNormal)
                .PermitReentry(NinjaAnimation.Idle2);

            Animations.Configure(NinjaAnimation.Death1)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.Death1);
                             })
                .PermitReentry(NinjaAnimation.Death1)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .Permit(NinjaAnimation.GroundBack, NinjaAnimation.GroundBack);

            Animations.Configure(NinjaAnimation.GroundBack)
                .PermitReentry(NinjaAnimation.GroundBack)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .Permit(NinjaAnimation.WalkNormal, NinjaAnimation.WalkNormal)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.GroundBack);
                             });

            Animations.Configure(NinjaAnimation.WalkNormal)
                .PermitReentry(NinjaAnimation.WalkNormal)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .Permit(NinjaAnimation.Death1, NinjaAnimation.Death1)
                .OnEntry(() =>
                             {
                                 Velocity = 1.5f;
                                 SetAnimation(NinjaAnimation.WalkNormal);
                             });

            Animations.Configure(NinjaAnimation.Block)
                .Permit(NinjaAnimation.Blocked, NinjaAnimation.Blocked)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.Block);
                             });

            Animations.Configure(NinjaAnimation.Blocked)
                .Permit(NinjaAnimation.ReverseBlock, NinjaAnimation.ReverseBlock)
                .PermitReentry(NinjaAnimation.Blocked)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.Blocked);
                             });

            Animations.Configure(NinjaAnimation.ReverseBlock)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(NinjaAnimation.ReverseBlock, true);
                             });


            Animations.Configure(NinjaAnimation.Overhead)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() =>
                             {
                                 Velocity = 2;
                                 SetAnimation(NinjaAnimation.Overhead);
                             })
                .Permit(NinjaAnimation.Death1, NinjaAnimation.Death1);

            Animations.Configure(NinjaAnimation.SpinningSword)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() =>
                             {
                                 Velocity = 3;
                                 SetAnimation(NinjaAnimation.SpinningSword);
                             })
                .Permit(NinjaAnimation.Death1, NinjaAnimation.Death1);

            Animations.Configure(NinjaAnimation.ForwardKick)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() =>
                             {
                                 Velocity = 1.5f;
                                 SetAnimation(NinjaAnimation.ForwardKick);
                             });

            Animations.Configure(NinjaAnimation.SideKick)
                .Permit(NinjaAnimation.Idle2, NinjaAnimation.Idle2)
                .OnEntry(() =>
                             {
                                 Velocity = 1.5f;
                                 SetAnimation(NinjaAnimation.SideKick);
                             });
        }

        private void ConfigureActions()
        {
            _actions[Buttons.X] = NinjaAnimation.Overhead;
            _actions[Keys.X] = NinjaAnimation.Overhead;

            _actions[Buttons.A] = NinjaAnimation.SideKick;
            _actions[Keys.Z] = NinjaAnimation.SideKick;

            _actions[Buttons.B] = NinjaAnimation.ForwardKick;
            _actions[Keys.C] = NinjaAnimation.ForwardKick;

            _actions[Buttons.Y] = NinjaAnimation.SpinningSword;
            _actions[Keys.V] = NinjaAnimation.SpinningSword;

            _actions[Buttons.RightShoulder] = NinjaAnimation.Block;
            _actions[Keys.LeftShift] = NinjaAnimation.Block;
        }

        private void ModelOnStoppedAnimation()
        {
            IsRunning = false;

            if (Animations.State == NinjaAnimation.Block)
            {
                Animations.Fire(NinjaAnimation.Blocked);
            }
            else if (Animations.State == NinjaAnimation.Blocked)
            {
                Animations.Fire(Blocked ? NinjaAnimation.Blocked : NinjaAnimation.ReverseBlock);
            }
            else
            {
                Blocked = false;

                if (_walking)
                {
                    IsRunning = true;
                    Animations.Fire(IsAlive ? NinjaAnimation.WalkNormal : NinjaAnimation.GroundBack);
                }
                else
                    Animations.Fire(IsAlive ? NinjaAnimation.Idle2 : NinjaAnimation.GroundBack);
            }
        }

        public void SetAnimation(NinjaAnimation animation, bool reverse = false)
        {
            var tuple = AnimationsRange[animation];

            if (animation != NinjaAnimation.Idle2)
                IsRunning = true;

            Model.setAnimBounds(tuple.Item1, tuple.Item2, reverse);
        }

        public void Render(GameTime gameTime)
        {
            Model.Render();
            Model.advanceAnimation((float) gameTime.ElapsedGameTime.TotalMilliseconds/(100/Velocity));
        }

        public void SetTransformation(Transformation trans)
        {
            Model.MaterialIndex = (short) trans;
        }

        public void Update(GamePadState getState)
        {
            var keyboardState = Keyboard.GetState(PlayerIndex.One);
            var lista = GamePadUtilities.GetPressed(getState);

            // In case you get lost, press A to warp back to the center.
            if (lista.Contains(Buttons.Start))
            {
                Position = Vector3.Zero;
                Rotation = 0.0f;
            }

           // Find out what direction we should be thrusting, using rotation.

            if (!IsAlive && Animations.State != NinjaAnimation.GroundBack)
                Animations.Fire(NinjaAnimation.Death1);

            if (getState.ThumbSticks.Left != Vector2.Zero ||
                (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.Up)
                 || keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Right)))
            {
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2 && Animations.State != NinjaAnimation.Death1)
                {
                    _walking = true;
                    Animations.Fire(NinjaAnimation.WalkNormal);
                }
            }
            else
            {
                if (Animations.State == NinjaAnimation.WalkNormal)
                {
                    _walking = false;
                    IsRunning = false;
                    Animations.Fire(NinjaAnimation.Idle2);
                }
            }

            if (lista.Contains(Buttons.X) || keyboardState.IsKeyDown(Keys.X))
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2)
                    Animations.Fire(_actions[Buttons.X]);

            if (lista.Contains(Buttons.A) || keyboardState.IsKeyDown(Keys.Z))
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2)
                    Animations.Fire(_actions[Buttons.A]);

            if (lista.Contains(Buttons.B) || keyboardState.IsKeyDown(Keys.C))
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2)
                    Animations.Fire(_actions[Buttons.B]);

            if (lista.Contains(Buttons.Y) || keyboardState.IsKeyDown(Keys.V))
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2)
                    Animations.Fire(_actions[Buttons.Y]);

            if (lista.Contains(Buttons.RightShoulder) || keyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (!IsRunning && Animations.State == NinjaAnimation.Idle2)
                {
                    Animations.Fire(_actions[Buttons.RightShoulder]);
                    Blocked = true;
                }
            }
            else
            {
                Blocked = false;
            }

            Vector2 leftStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                leftStick.Y = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                leftStick.Y = 1;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                leftStick.X = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                leftStick.X = 1;
            }

            leftStick.Normalize();
            if ((leftStick.X >= 0 || leftStick.X <= 0) || (leftStick.Y >= 0 || leftStick.Y <= 0))
            {
                Rotation = (float)Math.Acos(-leftStick.Y);

                if (leftStick.X < 0.0f)
                    Rotation = -Rotation;
            }


            if (!(float.IsNaN(leftStick.X) || float.IsNaN(leftStick.Y)))
            {
                // Rotate the model using the left thumbstick, and scale it down.
                Position.X += -leftStick.X*1.5f;
                Position.Z += leftStick.Y*1.5f;
            }

            if (keyboardState.IsKeyDown(Keys.A))
                SetTransformation(Transformation.Blue);

            if (keyboardState.IsKeyDown(Keys.S))
                SetTransformation(Transformation.Brown);

            if (keyboardState.IsKeyDown(Keys.D))
                SetTransformation(Transformation.Green);

            if (keyboardState.IsKeyDown(Keys.F))
                SetTransformation(Transformation.White);

            if (keyboardState.IsKeyDown(Keys.G))
                SetTransformation(Transformation.Red);
        }
    }
}