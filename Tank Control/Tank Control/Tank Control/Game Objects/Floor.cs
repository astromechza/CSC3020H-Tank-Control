using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Control.Game_Objects
{
    class Floor : GameObject
    {

        VertexPositionNormalTexture[] vertices;
        short[] indices;

        Texture2D texture;
        BasicEffect quadEffect;

        public Floor(Game g, Vector3 origin, float width, float length) : base(g, origin)
        {
            /*
             * 1--3
             * |  |
             * 0--2
             */
            vertices = new VertexPositionNormalTexture[4];
            Vector3 n = Vector3.Backward;
            Vector3 up = Vector3.Up;
            Vector3 l = Vector3.Cross(n, up);
            Vector3 uc = (Vector3.Forward * length / 2) + origin;

            vertices[1].Position = uc + (Vector3.Left * width / 2);
            vertices[1].TextureCoordinate = new Vector2(0, 0);
            vertices[1].Normal = n;

            vertices[3].Position = uc - (Vector3.Left * width / 2);
            vertices[3].TextureCoordinate = new Vector2(8, 0);
            vertices[3].Normal = n;

            vertices[0].Position = vertices[1].Position - (Vector3.Forward * length);
            vertices[0].TextureCoordinate = new Vector2(0, 8);
            vertices[0].Normal = n;

            vertices[2].Position = vertices[3].Position - (Vector3.Forward * length);
            vertices[2].TextureCoordinate = new Vector2(8, 8);
            vertices[2].Normal = n;

            indices = new short[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;
             
        }

        public override void LoadContent(ContentManager contentMan)
        {
            texture = contentMan.Load<Texture2D>("sand");

            quadEffect = new BasicEffect(this.game.graphics.GraphicsDevice);
            quadEffect.EnableDefaultLighting();
            quadEffect.PreferPerPixelLighting = true;
            quadEffect.World = Matrix.Identity;
            quadEffect.TextureEnabled = true;
            quadEffect.Texture = texture;
        }

        public override void Draw()
        {
            quadEffect.View = game.viewMatrix;
            quadEffect.Projection = game.projectionMatrix;
            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>
                    (PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2); 
            }
        }

        public override void Update(double elapsedMillis)
        {
            // nothing to do here
        }

    }
}
