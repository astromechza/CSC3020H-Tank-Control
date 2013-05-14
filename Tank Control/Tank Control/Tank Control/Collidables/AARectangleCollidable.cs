using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Control.Collidables
{
    class AARectangleCollidable : Collidable
    {
        public int width;
        public int length;
        public Vector3 origin;

        public AARectangleCollidable(Vector3 origin, int width, int length)
        {
            this.width = width;
            this.length = length;
            this.origin = origin;
        }

        public AARectangleCollidable(Vector3 origin, int width)
        {
            this.width = width;
            this.length = width;
            this.origin = origin;
        }

        public override bool collidesWith(Collidable other)
        {
            throw new NotImplementedException();
        }

        public override VertexPositionColor[] getPolygonVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[4];
            vertices[0].Position = origin + new Vector3(-width / 2, 1, -length / 2);
            vertices[1].Position = origin + new Vector3(width / 2, 1, -length / 2);
            vertices[2].Position = origin + new Vector3(width / 2, 1, length / 2);
            vertices[3].Position = origin + new Vector3(-width / 2, 1, length / 2);
            return vertices;
        }

        public override short[] getPolygonLineList()
        {
            short[] indices = new short[8];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 1;
            indices[3] = 2;
            indices[4] = 2;
            indices[5] = 3;
            indices[6] = 3;
            indices[7] = 0;
            return indices;
        }
        
    }
}
