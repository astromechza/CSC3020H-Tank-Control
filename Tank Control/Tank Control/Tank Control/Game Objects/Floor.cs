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
        int verticesWidth;
        int verticesLong;
        VertexPositionNormalTexture[] vertices;
        short[] indices;

        Texture2D texture;
        BasicEffect quadEffect;

        public Floor(Game g, Vector3 origin, int cellSize, int width, int length) : base(g, origin)
        {
            verticesWidth = width + 1;
            verticesLong = length + 1;
            vertices = new VertexPositionNormalTexture[verticesWidth * verticesLong];

            // First calculate top left corner
            Vector3 tl = origin + (Vector3.Left * (width / 2) * cellSize) + (Vector3.Forward * (length / 2) * cellSize);

            // first loop construct vertices
            for (int y = 0; y <= length; y++)
            {
                Vector3 rowstart = tl + (Vector3.Backward * cellSize * y);

                for (int x = 0; x <= width; x++)
                {
                    vertices[y * verticesWidth + x].Position = rowstart + (Vector3.Right * cellSize * x);
                    vertices[y * verticesWidth + x].Normal = Vector3.Up;
                    vertices[y * verticesWidth + x].TextureCoordinate = new Vector2(x , y );

                }
            }

            // now create indices
            indices = new short[width*length*6];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    indices[(y * width + x) * 6 + 0] = (short)(y * verticesWidth + x + 0);
                    indices[(y * width + x) * 6 + 1] = (short)(y * verticesWidth + x + verticesWidth + 1);
                    indices[(y * width + x) * 6 + 2] = (short)(y * verticesWidth + x + verticesWidth);
                    indices[(y * width + x) * 6 + 3] = (short)(y * verticesWidth + x + 0);
                    indices[(y * width + x) * 6 + 4] = (short)(y * verticesWidth + x + 1);
                    indices[(y * width + x) * 6 + 5] = (short)(y * verticesWidth + x + verticesWidth + 1);
                }
            }

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

            quadEffect.FogEnabled = true;
            quadEffect.FogColor = Vector3.Zero;
            quadEffect.FogEnd = 5120;
            quadEffect.FogStart = 4096;

        }

        public override void Draw()
        {
            quadEffect.View = game.viewMatrix;
            quadEffect.Projection = game.projectionMatrix;
            

            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {                
                pass.Apply();
                game.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture> (PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length/3); 
            }
        }

        public override void Update(double elapsedMillis)
        {
            // nothing to do here
        }

    }
}
