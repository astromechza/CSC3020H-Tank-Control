using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Tank_Control.Dbg_Drawers
{
    class RectangleDrawer
    {
        private Game game;
        private Rectangle rectangle;
        private BasicEffect effect;

        private VertexPositionColor[] vertices;
        private short[] indices;

        public RectangleDrawer(Game g, Rectangle r)
        {
            this.game = g;
            this.rectangle = r;
        }

        public void LoadContent(ContentManager content)
        {
            effect = new BasicEffect(game.GraphicsDevice);
            //e.EnableDefaultLighting();
            effect.PreferPerPixelLighting = true;
            effect.World = Matrix.Identity;
            effect.DiffuseColor = Color.Red.ToVector3();

            vertices = new VertexPositionColor[4];
            vertices[0].Position = new Vector3(this.rectangle.Left, 0, this.rectangle.Top);
            vertices[0].Color = Color.Red;
            vertices[1].Position = new Vector3(this.rectangle.Right, 0, this.rectangle.Top);
            vertices[1].Color = Color.Red;
            vertices[2].Position = new Vector3(this.rectangle.Right, 0, this.rectangle.Bottom);
            vertices[2].Color = Color.Red;
            vertices[3].Position = new Vector3(this.rectangle.Left, 0, this.rectangle.Bottom);
            vertices[3].Color = Color.Red;

            indices = new short[8];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;

            
        }


        public void DrawRect()
        {

            effect.View = game.viewMatrix;
            effect.Projection = game.projectionMatrix;
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
            }


        }

        public void ChangeRect(Rectangle r)
        {
            vertices[0].Position = new Vector3(r.Left, 1, r.Top);
            vertices[1].Position = new Vector3(r.Right, 1, r.Top);
            vertices[2].Position = new Vector3(r.Right, 1, r.Bottom);
            vertices[3].Position = new Vector3(r.Left, 1, r.Bottom);
        }

        

    }
}
