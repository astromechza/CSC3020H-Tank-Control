using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Control.Collidables
{
    public class CircleCollidable : Collidable
    {

        public Vector3 origin;
        public int radius;
        public int segments;

        public CircleCollidable(Vector3 origin, int radius, int segments)
        {
            this.origin = origin;
            this.radius = radius;
            this.segments = segments;
        }

        public CircleCollidable(Vector3 origin, int radius)
        {
            this.origin = origin;
            this.radius = radius;
            this.segments = 16;
        }

        public override bool collidesWith(Collidable other)
        {
            throw new NotImplementedException();
        }

        public override VertexPositionColor[] getPolygonVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[segments];

            float a = 0.0f;
            float d = (float)((Math.PI * 2) / segments);

            for (int i = 0; i < segments; i++)
            {

                float x = (float)Math.Cos(a) * radius;
                float y = (float)Math.Sin(a) * radius;

                vertices[i].Position = new Vector3(origin.X + x, 0, origin.Z + y);
                a += d;
            }

            return vertices;
        }

        public override short[] getPolygonLineList()
        {
            short[] indices = new short[segments * 2];

            for (short i = 0; i < segments; i++)
            {
                indices[i * 2] = i;
                indices[i * 2 + 1] = (short)(i + 1);
            }
            indices[segments * 2 - 1] = 0;

            return indices;
        }

    }
}
