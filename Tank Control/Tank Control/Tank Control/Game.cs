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
using Tank_Control.Dbg_Drawers;
using Tank_Control.Collidables;

namespace Tank_Control
{

    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PlatformID platform;
        Tank tank;
        Floor floor;

        public List<RandomObject> randomObjects;

        HudOverlay hud;
        
        public CombinedCamera camera;

        // View and Projection Matrices
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            hud = new HudOverlay(this);
            tank = new Tank(this, new Vector3(0,0,0));
            floor = new Floor(this, new Vector3(0, 0, 0), 512, 40, 40);
            
            camera = new CombinedCamera(tank, CameraMode.ThirdPerson, new Vector3(0,10000,-10000f));

            randomObjects = new List<RandomObject>();

            OperatingSystem os = Environment.OSVersion;
            platform = os.Platform;
        }

        protected override void Initialize()
        {
            // Create Projection Matrix
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio, 1f, 10000);

            viewMatrix = camera.getViewMatrix();

            Mouse.SetPosition(1280 / 2, 720 / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            System.Diagnostics.Debug.WriteLine("Started LoadContent");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tank.LoadContent(Content);
            floor.LoadContent(Content);
            hud.LoadContent(Content);
            
            RandomObject.PreLoad(Content);
            ColliderDrawer.Init(this);

            Random r = new Random();

            int numbObjs = 100;
            for (int i = 0; i < numbObjs; i++)
            {

                int x = r.Next(-10240,10240);
                int z = r.Next(-10240,10240);

                if (Vector2.Distance(new Vector2(0, 0), new Vector2(x, z)) < 1000)
                {
                    i--;
                    continue;
                }

                RandomObject ro = new RandomObject(this, new Vector3(x, 0, z));
                ro.LoadContent(Content);
                randomObjects.Add(ro);
            }
            System.Diagnostics.Debug.WriteLine("Finished LoadContent");

        }

        protected override void UnloadContent()
        {

        }

        public void addLightingToEffect(BasicEffect effect)
        {            
            effect.PreferPerPixelLighting = true;
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.5f);
            effect.DirectionalLight0.SpecularColor = new Vector3(0.2f, 0.2f, 0.5f);
            effect.DirectionalLight0.Direction = new Vector3(1, -0.3f, 0);

            effect.DirectionalLight1.Enabled = true;

            effect.DirectionalLight1.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
            effect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight1.Direction = new Vector3(-0.5f, -1, 0.86f);

            effect.DirectionalLight2.Enabled = true;

            effect.DirectionalLight2.DiffuseColor = new Vector3(0.5f, 0.0f, 0.0f);
            effect.DirectionalLight2.SpecularColor = new Vector3(0.5f, 0.0f, 0.0f);
            effect.DirectionalLight2.Direction = new Vector3(-0.5f, 0, -0.86f);

            effect.SpecularPower = 100f;
        }

        public void addFogToEffect(BasicEffect effect)
        {
            effect.FogEnabled = true;
            effect.FogColor = Color.Black.ToVector3() ;
            effect.FogStart = 6000;
            effect.FogEnd = 8000;
        }

        private bool tildeInKeyPress = false;
        private void HandleInput()
        {
            if (platform == PlatformID.Xbox)
            {
                GamePadState gs = GamePad.GetState(PlayerIndex.One);
                tank.handleInput(gs);
                camera.handleInput(gs);

                if (gs.IsButtonDown(Buttons.B))
                {
                    if (!tildeInKeyPress)
                    {
                        ColliderDrawer.Toggle();
                        tildeInKeyPress = true;
                    }
                }
                else
                {
                    tildeInKeyPress = false;
                }

                if (gs.IsButtonDown(Buttons.Back)) this.Exit();
            }
            else
            {
                KeyboardState ks = Keyboard.GetState();
                MouseState ms = Mouse.GetState();

                tank.handleInput(ks, ms);
                camera.handleInput(ks);
                
                if (ks.IsKeyDown(Keys.OemTilde))
                {
                    if (!tildeInKeyPress)
                    {
                        ColliderDrawer.Toggle();
                        tildeInKeyPress = true;
                    }
                }
                else
                {
                    tildeInKeyPress = false;
                }

                if (ks.IsKeyDown(Keys.Escape)) this.Exit();

                Mouse.SetPosition(1280 / 2, 720 / 2);
            }
        }

        protected override void Update(GameTime gameTime)
        {

            HandleInput();            

            // Update sector
            double m = gameTime.ElapsedGameTime.TotalMilliseconds;

            tank.Update(m);
            hud.Update(m);
            
            // update camera and get new view matrix
            camera.UpdatePosition();
            viewMatrix = camera.getViewMatrix();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            // clear to balck
            GraphicsDevice.Clear(Color.Black);



            floor.Draw();
            tank.Draw();
            foreach (var ro in randomObjects)
            {
                ro.Draw();
            }

            hud.Draw();

            base.Draw(gameTime);
        }

    }
}
