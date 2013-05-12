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
        RandomObject rob1, rob2;

        CombinedCamera camera;

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
            floor = new Floor(this, new Vector3(0, 0, 0), 512, 40, 40);
            rob1 = new RandomObject(this, new Vector3(2048, 140, 0));
            rob2 = new RandomObject(this, new Vector3(2048, 140, 2048));

            camera = new CombinedCamera(tank, CameraMode.ThirdPerson, new Vector3(0,10000,-10000f));
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
            rob1.LoadContent(Content);
            rob2.LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {

            double m = gameTime.ElapsedGameTime.TotalMilliseconds;

            tank.handleInput();
            camera.handleInput();

            tank.Update(m);
            fps.Update(m);

            camera.UpdatePosition();

            viewMatrix = camera.getViewMatrix();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            rob1.Draw();
            rob2.Draw();
            floor.Draw();
            tank.Draw();

            fps.Draw();

            base.Draw(gameTime);
        }
    }
}
