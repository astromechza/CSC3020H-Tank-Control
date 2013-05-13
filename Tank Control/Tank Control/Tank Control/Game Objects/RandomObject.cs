using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Tank_Control.Collidables;

namespace Tank_Control.Game_Objects
{
    public class RandomObject : GameObject, IQuadStorable
    {
        Texture2D tex;
        Model model;
        Matrix[] boneOriginTransforms;
        int type;
        int textype;
        Collidable collide;
        
        public RandomObject(Game g, Vector3 o, int type, int textype) : base(g, o)
        {
            this.textype = textype;
            this.type = type;
        }

        public override void LoadContent(ContentManager contentMan)
        {

            Matrix shapeTransform = Matrix.Identity;
            
            switch (type)
            {
                case 0:
                    model = contentMan.Load<Model>("Cube");
                    collide = new AARectangleCollidable(this.position, 480);
                    this.position.Y += 220;
                    shapeTransform =  Matrix.CreateScale(130) * Matrix.CreateTranslation(this.position);
                    break;
                case 1:
                    model = contentMan.Load<Model>("Cone");
                    collide = new CircleCollidable(this.position, 100);
                    this.position.Y += 133;
                    shapeTransform = Matrix.CreateScale(50) * Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(this.position);
                    break;
                case 2:
                    model = contentMan.Load<Model>("Cylinder");
                    collide = new CircleCollidable(this.position, 160);
                    this.position.Y += 190;
                    shapeTransform = Matrix.CreateScale(80) * Matrix.CreateRotationX((float)Math.PI / 2) * Matrix.CreateTranslation(this.position);
                    break;
            }

            boneOriginTransforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                boneOriginTransforms[i] = model.Bones[i].Transform * shapeTransform;
            }

            switch (textype)
            {
                case 0:
                    tex = contentMan.Load<Texture2D>("hazard");
                    break;
                case 1:
                    tex = contentMan.Load<Texture2D>("rocks");
                    break;
                case 2:
                    tex = contentMan.Load<Texture2D>("crate");
                    break;
                case 3:
                    tex = contentMan.Load<Texture2D>("chess");
                    break;
            }

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

    }
}
