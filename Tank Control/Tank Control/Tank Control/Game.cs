using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Tank_Control.Game_Objects;
using Tank_Control.Cameras;

namespace Tank_Control
{

    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tank tank;
        Floor floor;
        FPSComponent fps;

        TrackingCamera camera;

        // View and Projection Matrices
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            fps = new FPSComponent(this);
            tank = new Tank(this, new Vector3(0,0,0));
            floor = new Floor(this, new Vector3(0, 0, 0), 8192, 8192);
            camera = new ThirdPCamera(new Vector3(128f, 128f, 128f), tank, 2048f, 1024f, 0.2f);
        }

        protected override void Initialize()
        {
            // Create Projection Matrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio, 1f, 10000);

            viewMatrix = camera.getViewMatrix();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tank.LoadContent(Content);
            floor.LoadContent(Content);
            fps.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {

            tank.handleInput();

            double m = gameTime.ElapsedGameTime.TotalMilliseconds;
            tank.Update(m);
            fps.Update(m);

            camera.UpdatePosition();
            viewMatrix = camera.getViewMatrix();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            floor.Draw();
            tank.Draw();

            fps.Draw();

            base.Draw(gameTime);
        }
    }
}
