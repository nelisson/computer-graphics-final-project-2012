namespace FinalProjectCG.Dwarf
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MilkshapeModel;
    using Stateless;
    using Utilities;

    public class Dwarf
    {
        public StateMachine<DwarfAnimation, DwarfAnimation> Animations =
            new StateMachine<DwarfAnimation, DwarfAnimation>(DwarfAnimation.Idle);

        public Dictionary<DwarfAnimation, Tuple<int, int>> AnimationsRange;

        public Vector3 Position = Vector3.Zero;
        public float Rotation;
        private int _currentHealth;
        public bool Walking;

        public Dwarf(GraphicsDevice device)
        {
            Walking = false;
            CurrentHealth = 100;
            ConfigureStateMachine();
            Velocity = 1;
            AnimationsRange = new Dictionary<DwarfAnimation, Tuple<int, int>>
                                  {
                                      {DwarfAnimation.Walk, new Tuple<int, int>(2, 14)},
                                      {DwarfAnimation.Run, new Tuple<int, int>(16, 26)},
                                      {DwarfAnimation.Idle, new Tuple<int, int>(75, 110)},
                                      {DwarfAnimation.Attack, new Tuple<int, int>(162, 180)},
                                      {DwarfAnimation.Stab, new Tuple<int, int>(182, 192)},
                                      {DwarfAnimation.Die, new Tuple<int, int>(212, 227)},
                                      {DwarfAnimation.Ground, new Tuple<int, int>(227, 227)},
                                  };


            Model = new Random().Next(2) == 0
                        ? new MilkshapeModel("Dwarf\\dwarf1.ms3d", device)
                        : new MilkshapeModel("Dwarf\\dwarf2.ms3d", device);

            Model.StoppedAnimation += ModelOnStoppedAnimation;
            SetAnimation(DwarfAnimation.Idle);
        }

        public bool IsAttacking { get; set; }

        public MilkshapeModel Model { get; set; }

        public float Velocity { get; set; }
        public bool IsRunning { get; set; }

        public bool IsAlive
        {
            get { return CurrentHealth > 0; }
        }

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
            Animations.Configure(DwarfAnimation.Idle)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(DwarfAnimation.Idle);
                             })
                .Permit(DwarfAnimation.Attack, DwarfAnimation.Attack)
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die)
                .Permit(DwarfAnimation.Stab, DwarfAnimation.Stab)
                .Permit(DwarfAnimation.Walk, DwarfAnimation.Walk)
                .Permit(DwarfAnimation.Run, DwarfAnimation.Run)
                .PermitReentry(DwarfAnimation.Idle);

            Animations.Configure(DwarfAnimation.Die)
                .OnEntry(() =>
                             {
                                 Velocity = 2;
                                 SetAnimation(DwarfAnimation.Die);
                             })
                .PermitReentry(DwarfAnimation.Die)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .Permit(DwarfAnimation.Ground, DwarfAnimation.Ground);

            Animations.Configure(DwarfAnimation.Ground)
                .PermitReentry(DwarfAnimation.Ground)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .Permit(DwarfAnimation.Walk, DwarfAnimation.Walk)
                .OnEntry(() =>
                             {
                                 Velocity = 1;
                                 SetAnimation(DwarfAnimation.Ground);
                             });

            Animations.Configure(DwarfAnimation.Walk)
                .PermitReentry(DwarfAnimation.Walk)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die)
                .Permit(DwarfAnimation.Run, DwarfAnimation.Run)
                .OnEntry(() =>
                             {
                                 Velocity = 1f;
                                 SetAnimation(DwarfAnimation.Walk);
                             });

            Animations.Configure(DwarfAnimation.Run)
                .PermitReentry(DwarfAnimation.Run)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die)
                .Permit(DwarfAnimation.Walk, DwarfAnimation.Walk)
                .OnEntry(() =>
                             {
                                 Velocity = 1f;
                                 SetAnimation(DwarfAnimation.Run);
                             });


            Animations.Configure(DwarfAnimation.Attack)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .OnEntry(() =>
                             {
                                 IsAttacking = true;
                                 Velocity = 2;
                                 SetAnimation(DwarfAnimation.Attack);
                             })
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die);

            Animations.Configure(DwarfAnimation.Stab)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .OnEntry(() =>
                             {
                                 IsAttacking = true;
                                 Velocity = 2;
                                 SetAnimation(DwarfAnimation.Stab);
                             })
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die);
        }

        private void ModelOnStoppedAnimation()
        {
            IsAttacking = false;
            IsRunning = false;


            if (Walking)
            {
                IsRunning = true;
                Animations.Fire(IsAlive ? Animations.State : DwarfAnimation.Ground);
            }
            else
            {
                if(IsAlive)
                    Animations.Fire(DwarfAnimation.Idle);
                else
                {
                    if (Animations.State != DwarfAnimation.Die && Animations.State != DwarfAnimation.Ground)
                    {
                        Animations.Fire(DwarfAnimation.Die);
                    }
                    else
                    {
                        Animations.Fire(DwarfAnimation.Ground);
                    }
                }
            }
        }

        public void SetAnimation(DwarfAnimation animation, bool reverse = false)
        {
            var tuple = AnimationsRange[animation];

            if (animation != DwarfAnimation.Idle)
                IsRunning = true;

            Model.SetAnimBounds(tuple.Item1, tuple.Item2, reverse);
        }

        public void Render(GameTime gameTime)
        {
            Model.Render();
            Model.AdvanceAnimation((float) gameTime.ElapsedGameTime.TotalMilliseconds/(100/Velocity));
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

            if (!IsAlive)
                if (Animations.State != DwarfAnimation.Ground)
                    if (Animations.State == DwarfAnimation.Idle ||Animations.State == DwarfAnimation.Run
                        || Animations.State == DwarfAnimation.Walk)
                        Animations.Fire(DwarfAnimation.Die);

            if (Walking)
            {
                if (!IsRunning && Animations.State == DwarfAnimation.Idle
                    && Animations.State != DwarfAnimation.Die)
                {
                    Animations.Fire(new Random().Next(2) == 0 ? DwarfAnimation.Walk : DwarfAnimation.Run);
                }
            }
            else
            {
                if (Animations.State == DwarfAnimation.Walk || Animations.State == DwarfAnimation.Run)
                {
                    Walking = false;
                    IsRunning = false;
                    Animations.Fire(DwarfAnimation.Idle);
                }
            }

            if (lista.Contains(Buttons.X) || keyboardState.IsKeyDown(Keys.X))
                if (!IsRunning && Animations.State == DwarfAnimation.Idle)
                    Animations.Fire(DwarfAnimation.Attack);

            if (lista.Contains(Buttons.A) || keyboardState.IsKeyDown(Keys.Z))
                if (!IsRunning && Animations.State == DwarfAnimation.Idle)
                    Animations.Fire(DwarfAnimation.Stab);
        }
    }
}