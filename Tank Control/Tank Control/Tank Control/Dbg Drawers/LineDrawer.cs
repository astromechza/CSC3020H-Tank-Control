using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Tank_Control.Collidables;
using Microsoft.Xna.Framework;

namespace Tank_Control.Dbg_Drawers
{
    public class LineDrawer
    {

        private VertexPositionColor[] vertices;
        private short[] indices;
        private BasicEffect effect;
        private Game game;
        private bool readyForDraw = false;

        public LineDrawer(Game g, Collidable source)
        {
            game = g;
            vertices = source.getPolygonVertices();
            indices = source.getPolygonLineList();
        }

        public void init()
        {
            effect = new BasicEffect(game.GraphicsDevice);
            effect.PreferPerPixelLighting = true;
            effect.World = Matrix.Identity;
            effect.DiffuseColor = Color.Red.ToVector3();
            readyForDraw = true;
        }

        public void update(Collidable source)
        {
            vertices = source.getPolygonVertices();
            indices = source.getPolygonLineList();
        }

        public void Draw()
        {
            if (!readyForDraw) return;
            effect.View = game.viewMatrix;
            effect.Projection = game.projectionMatrix;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
            }
        }

    }
}
