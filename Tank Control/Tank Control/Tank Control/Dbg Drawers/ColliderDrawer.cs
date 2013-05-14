using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Tank_Control.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tank_Control.Dbg_Drawers
{
    public class ColliderDrawer
    {

        // we only want one effect drawer, thus it is static
        private static BasicEffect effect;
        private static bool readyForDraw = false;
        private static bool enabled = false;

        private VertexPositionColor[] vertices;
        private short[] indices;
        private Game game;

        public ColliderDrawer(Game g, Collidable source)
        {
            game = g;
            vertices = source.getPolygonVertices();
            indices = source.getPolygonLineList();
        }

        public static void Init(Game game)
        {
            effect = new BasicEffect(game.GraphicsDevice);
            effect.World = Matrix.Identity;
            effect.DiffuseColor = Color.Yellow.ToVector3();
            readyForDraw = true;
        }

        public void update(Collidable source)
        {
            vertices = source.getPolygonVertices();
            indices = source.getPolygonLineList();
        }

        public void Draw()
        {
            if (!readyForDraw || !enabled) return;
            effect.View = game.viewMatrix;
            effect.Projection = game.projectionMatrix;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length, indices, 0, indices.Length / 2);
            }
        }

        public static void Enable()
        {
            enabled = true;
        }

        public static void Disable()
        {
            enabled = false;
        }

        public static void Toggle()
        {
            enabled = !enabled;
        }


    }
}
