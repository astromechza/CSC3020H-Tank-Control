using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Tank_Control.Collidables;
using Tank_Control.Dbg_Drawers;

namespace Tank_Control.Game_Objects
{
    public class RandomObject : GameObject
    {
        Texture2D tex;
        Model model;
        Matrix[] boneOriginTransforms;
        int type;
        int textype;
        Collidable collide;
        ColliderDrawer colliderDrawer;
        
        public RandomObject(Game g, Vector3 o) : base(g, o)
        {
            this.textype = rsource.Next(0,loadedTextures.Length);
            this.type = rsource.Next(0, loadedModels.Length);
        }

        public override void LoadContent(ContentManager contentMan)
        {

            Matrix shapeTransform = Matrix.Identity;
            model = loadedModels[type];
            
            switch (type)
            {
                case 0:
                    collide = new AARectangleCollidable(this.position, 485);
                    this.position.Y += 220;
                    shapeTransform =  Matrix.CreateScale(130) * Matrix.CreateTranslation(this.position);
                    break;
                case 1:
                    collide = new CircleCollidable(this.position, 265);
                    this.position.Y += 253;
                    shapeTransform = Matrix.CreateScale(100) * Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(this.position);
                    break;
                case 2:
                    collide = new CircleCollidable(this.position, 165);
                    this.position.Y += 190;
                    shapeTransform = Matrix.CreateScale(80) * Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(this.position);
                    break;
            }

            boneOriginTransforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                boneOriginTransforms[i] = model.Bones[i].Transform * shapeTransform;
            }

            tex = loadedTextures[textype];
            
            colliderDrawer = new ColliderDrawer(this.game, this.getCollidable());

        }

        public override void Draw()
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    game.addLightingToEffect(effect);

                    game.addFogToEffect(effect); 

                    effect.TextureEnabled = true;
                    effect.Texture = tex;
                    effect.World = boneOriginTransforms[mesh.ParentBone.Index];
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;
           
                }
                mesh.Draw();
            }

            colliderDrawer.Draw();
        }

        public override void Update(double elapsedMillis) { }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)this.position.X - 250, (int)this.position.Z - 250, 500, 500);
        }

        public Collidable getCollidable()
        {
            return collide;
        }

        //// STATIC SECTION
        private static String[] modelNames = new String[] { "Cube", "Cone", "Cylinder",  };
        private static String[] textureNames = new String[] { "chess", "crate", "hazard", "rocks" };

        private static Model[] loadedModels;
        private static Texture2D[] loadedTextures;
        private static Random rsource; 

        public static void PreLoad(ContentManager Content)
        {
            System.Diagnostics.Debug.WriteLine("Start PreLoad");
            // Load Models
            loadedModels = new Model[modelNames.Length];

            for (int i = 0; i < modelNames.Length; i++)
            {
                loadedModels[i] = Content.Load<Model>(modelNames[i]);
            }

            // Load Textures            
            loadedTextures = new Texture2D[textureNames.Length];

            for (int i = 0; i < textureNames.Length; i++)
            {
                loadedTextures[i] = Content.Load<Texture2D>(textureNames[i]);
            }

            // make random source
            rsource = new Random();
            System.Diagnostics.Debug.WriteLine("End PreLoad");
        }
    }

    
}
