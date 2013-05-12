using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Tank_Control.Game_Objects
{
    public class RandomObject : GameObject, IQuadStorable
    {
        Texture2D tex;
        Model model;
        Matrix[] boneOriginTransforms;
        
        public RandomObject(Game g, Vector3 o) : base(g, o)
        {
            Random r = new Random();
            int type = r.Next(0, 4);
        }

        public override void LoadContent(ContentManager contentMan)
        {
            model = contentMan.Load<Model>("Cube");
            tex = contentMan.Load<Texture2D>("rocks");
            boneOriginTransforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                boneOriginTransforms[i] = model.Bones[i].Transform * Matrix.CreateScale(100) * Matrix.CreateTranslation(this.position);
            }
        }

        public override void Draw()
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = 4096;
                    effect.FogEnd = 5120;

                    effect.TextureEnabled = true;
                    effect.Texture = tex;

                    effect.World = boneOriginTransforms[mesh.ParentBone.Index];
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;
           
                }
                mesh.Draw();
            }
        }

        public override void Update(double elapsedMillis)
        {
            
        }

        public Rectangle Rect
        {
            get { return new Rectangle(0,0,0,0); }
        }
    }
}
