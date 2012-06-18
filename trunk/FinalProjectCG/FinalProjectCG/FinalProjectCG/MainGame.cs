namespace FinalProjectCG
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    ///   This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        // Cache information about the model size and position.
        private Matrix[] _boneTransforms;
        private Model _model;
        private Vector3 _modelCenter;
        private float _modelRadius;


        /// <summary>
        ///   Gets or sets the current model.
        /// </summary>
        public Model Model
        {
            get { return _model; }

            set
            {
                _model = value;

                if (_model != null)
                {
                    MeasureModel();
                }
            }
        }

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            Model = Content.Load<Model>("Ninjas\\GreenNinja");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_model != null)
            {

                float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

                float nearClip = _modelRadius / 100;
                float farClip = _modelRadius * 100;

                Matrix world = Matrix.Identity;
                Matrix view = Matrix.CreateLookAt(new Vector3(_modelCenter.X, _modelCenter.Y, _modelCenter.Z*10), _modelCenter, Vector3.Up);
                Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio,
                                                                        nearClip, farClip);

                // Draw the model.
                foreach (var mesh in _model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = _boneTransforms[mesh.ParentBone.Index] * world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        effect.SpecularPower = 20;
                    }
                    mesh.Draw();
                }
            }
           
            base.Draw(gameTime);
        }

        /// <summary>
        ///   Whenever a new model is selected, we examine it to see how big it is and where it is centered. This lets us automatically zoom the display, so we can correctly handle models of any scale.
        /// </summary>
        private void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            _boneTransforms = new Matrix[_model.Bones.Count];

            _model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.
            _modelCenter = Vector3.Zero;

            foreach (var meshCenter in _model.Meshes.Select(mesh => Vector3.Transform(mesh.BoundingSphere.Center, _boneTransforms[mesh.ParentBone.Index])))
            {
                _modelCenter += meshCenter;
            }

            _modelCenter /= _model.Meshes.Count;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            _modelRadius = 0;

            foreach (var mesh in _model.Meshes)
            {
                Matrix transform = _boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(mesh.BoundingSphere.Center, transform);

                float transformScale = transform.Forward.Length();

                float meshRadius = (meshCenter - _modelCenter).Length() +
                                   (mesh.BoundingSphere.Radius * transformScale);

                _modelRadius = Math.Max(_modelRadius, meshRadius);
            }
        }
    }
}