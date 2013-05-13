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
using C3.XNA;
using Tank_Control.Dbg_Drawers;
using Tank_Control.Collidables;

namespace Tank_Control
{

    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tank tank;
        Floor floor;
        FPSComponent fps;
        public QuadTree<RandomObject> quadTree;

        LineDrawer ldrawer2;

        RandomObject rob;

        CombinedCamera camera;

        // View and Projection Matrices
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;

            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            fps = new FPSComponent(this);
            tank = new Tank(this, new Vector3(0,0,0));
            floor = new Floor(this, new Vector3(0, 0, 0), 512, 40, 40);

            quadTree = new QuadTree<RandomObject>(-10240, -10240, 512 * 40, 512 * 40);

            ldrawer2 = new LineDrawer(this, new AARectangleCollidable(new Vector3(0, 0, 0), 512, 1024));

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

            ldrawer2.init();


            Random r = new Random();

            for (int i = 0; i < 100; i++)
            {

                int x = r.Next(-10240,10240);
                int z = r.Next(-10240,10240);

                if (Vector2.Distance(new Vector2(0, 0), new Vector2(x, z)) < 1000)
                {
                    i--;
                    continue;
                }

                RandomObject ro = new RandomObject(this, new Vector3(x, 0, z), r.Next(0, 3), r.Next(0, 4));
                ro.LoadContent(Content);
                quadTree.Add(ro);
            }

        }

        protected override void UnloadContent()
        {

        }

        public void addLightingToEffect(BasicEffect effect)
        {            
            effect.PreferPerPixelLighting = true;
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.2f, 0.5f);
            effect.DirectionalLight0.SpecularColor = new Vector3(0.2f, 0.2f, 1.0f);
            effect.DirectionalLight0.Direction = new Vector3(1, -1, 1);

            effect.DirectionalLight1.Enabled = true;

            effect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight1.Direction = new Vector3(-1, -0.1f, 0);

            effect.DirectionalLight2.Enabled = true;

            effect.DirectionalLight2.DiffuseColor = new Vector3(0.0f, 0.1f, 0.0f);
            effect.DirectionalLight2.SpecularColor = new Vector3(0.0f, 0.5f, 0.0f);
            effect.DirectionalLight2.Direction = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(tank.orientationAngle));

            effect.SpecularPower = 100f;
        }

        public void addFogToEffect(BasicEffect effect)
        {
            effect.FogEnabled = true;
            effect.FogColor = Vector3.Zero;
            effect.FogStart = 4096;
            effect.FogEnd = 8000;
        }

        protected override void Update(GameTime gameTime)
        {

            double m = gameTime.ElapsedGameTime.TotalMilliseconds;

            tank.handleInput();
            camera.handleInput();

            tank.Update(m);
            fps.Update(m);


            ldrawer2.update(tank.getCollidable());

            camera.UpdatePosition();

            viewMatrix = camera.getViewMatrix();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            floor.Draw();
            tank.Draw();
            List<RandomObject> ol = quadTree.GetAllObjects();
            foreach (var o in ol)
            {
                o.Draw();
            }

            ldrawer2.Draw();

            fps.Draw();

            base.Draw(gameTime);
        }
    }
}
