namespace FinalProjectCG.Dwarf
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using MilkshapeModel;
    using Nine.Navigation;
    using Stateless;

    public class Dwarf
    {
        public StateMachine<DwarfAnimation, DwarfAnimation> Animations =
            new StateMachine<DwarfAnimation, DwarfAnimation>(DwarfAnimation.Idle);

        public readonly List<int> Path = new List<int>();

        public PathGrid PathGraph;

        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                    OnIndexChanged();

                _index = value;
            }
        }


        public delegate void IndexChage();

        public event IndexChage IndexChanged;

        public void OnIndexChanged()
        {
            IndexChage handler = IndexChanged;
            if (handler != null) handler();
        }

        public Dictionary<DwarfAnimation, Tuple<int, int>> AnimationsRange;

        private Vector3 _position;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Index = PathGraph.PositionToIndex(_position.X, _position.Y);
            }
        }

        public float Rotation;
        private float _currentHealth;
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

        public float CurrentHealth
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
                .Permit(DwarfAnimation.Walk, DwarfAnimation.Walk)
                .Permit(DwarfAnimation.Run, DwarfAnimation.Run)
                .PermitReentry(DwarfAnimation.Attack)
                .OnEntry(() =>
                             {
                                 IsAttacking = true;
                                 Velocity = 2;
                                 SetAnimation(DwarfAnimation.Attack);
                             })
                .Permit(DwarfAnimation.Die, DwarfAnimation.Die);

            Animations.Configure(DwarfAnimation.Stab)
                .Permit(DwarfAnimation.Idle, DwarfAnimation.Idle)
                .PermitReentry(DwarfAnimation.Stab)
                .Permit(DwarfAnimation.Walk, DwarfAnimation.Walk)
                .Permit(DwarfAnimation.Run, DwarfAnimation.Run)
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

            Walking = Walking && IsAlive;

            if (Walking)
            {
                IsRunning = true;
                if(Animations.State != DwarfAnimation.Run || Animations.State != DwarfAnimation.Walk)
                    Animations.Fire(new Random().Next(2) == 0 ? DwarfAnimation.Walk : DwarfAnimation.Run);
                else
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

        public Vector2 TopLimit;
        public Vector2 BottomLimit;

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

        public void Update(GamePadState getState, Vector3 enemyPosition, bool enemyIsAlive)
        {
            if (getState.Buttons.Start == ButtonState.Pressed)
            {
                CurrentHealth = 100;
                Position = Vector3.Zero;
            }

            Vector3 direction = Vector3.Zero;
            if (IsAlive)
            {
                if (Path.Count > 1)
                {
                    var point1 =
                        new Vector3(
                            PathGraph.SegmentToPosition(Path[Path.Count - 1]%PathGraph.SegmentCountX,
                                                        Path[Path.Count - 1]/PathGraph.SegmentCountX),
                            0);
                    var point2 =
                        new Vector3(
                            PathGraph.SegmentToPosition(Path[Path.Count - 2]%PathGraph.SegmentCountX,
                                                        Path[Path.Count - 2]/PathGraph.SegmentCountX), 0);

                    direction = point2 - point1;
                    direction.Normalize();

                    Rotation = (float) Math.Acos(direction.Y);

                    if (direction.X > 0.0f)
                        Rotation = -Rotation;

                    Walking = true;
                }

                if (PathGraph.PositionToIndex(enemyPosition.X, enemyPosition.Y) == Index)
                {
                    Walking = false;
                    if (enemyIsAlive)
                    {
                        direction = enemyPosition - Position;
                        direction.Normalize();

                        Rotation = (float) Math.Acos(direction.Y);

                        if (direction.X > 0.0f)
                            Rotation = -Rotation;

                        if (!IsRunning)
                            Animations.Fire(new Random().Next(2) == 0 ? DwarfAnimation.Attack : DwarfAnimation.Stab);
                    }
                }
            }

            // Find out what direction we should be thrusting, using rotation.

            if (!IsAlive)
                if (Animations.State != DwarfAnimation.Ground)
                    if (Animations.State == DwarfAnimation.Idle ||Animations.State == DwarfAnimation.Run
                        || Animations.State == DwarfAnimation.Walk)
                        Animations.Fire(DwarfAnimation.Die);

            if (Walking && IsAlive)
            {
                Position += (direction * (Animations.State == DwarfAnimation.Walk ? 0.01f : 0.03f));

                _position.X = MathHelper.Clamp(Position.X, BottomLimit.X, TopLimit.X);
                _position.Y = MathHelper.Clamp(Position.Y, BottomLimit.Y, TopLimit.Y);

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
        }
    }
}