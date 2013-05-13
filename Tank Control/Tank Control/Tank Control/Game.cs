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

        LineDrawer ldrawer1;
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

            ldrawer1 = new LineDrawer(this, new CircleCollidable(new Vector3(0,0,0),10));
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

            ldrawer1.init();
            ldrawer2.init();


            Random r = new Random();

            for (int i = 0; i < 100; i++)
            {

                int x = r.Next(-10240,10240);
                int z = r.Next(-10240,10240);

                RandomObject ro = new RandomObject(this, new Vector3(x, 183, z));
                ro.LoadContent(Content);
                quadTree.Add(ro);
            }

            rob = new RandomObject(this, new Vector3(3000, 183, 3000));
            rob.LoadContent(Content);

            ldrawer1.update(rob.getCollidable());

            quadTree.Add(rob);
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

            ldrawer1.Draw();
            ldrawer2.Draw();

            fps.Draw();

            base.Draw(gameTime);
        }
    }
}
