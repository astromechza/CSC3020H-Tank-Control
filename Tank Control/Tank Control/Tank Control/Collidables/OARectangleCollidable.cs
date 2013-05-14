using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Control.Collidables
{
    class OARectangleCollidable : Collidable
    {

        private int width;
        private int length;
        private Vector3 origin;
        private float angle;
        private Vector3 suboffset;

        public OARectangleCollidable(Vector3 origin, float angle, int width, int length)
        {
            this.width = width;
            this.length = length;
            this.origin = origin;
            this.angle = angle;
            this.suboffset = Vector3.Zero;
        }

        public OARectangleCollidable(Vector3 origin, Vector3 suboffset, float angle, int width, int length)
        {
            this.width = width;
            this.length = length;
            this.origin = origin;
            this.angle = angle;
            this.suboffset = suboffset;
        }

        public override bool collidesWith(Collidable other)
        {
            VertexPositionColor[] selfvertices = this.getPolygonVertices();            

            if (other is CircleCollidable)
            {
                CircleCollidable o = (CircleCollidable)other;

                Vector2 last = Utils.castVector3XZ(selfvertices[0].Position);

                for (int i = 0; i <= selfvertices.Length; i++)
                {
                    Vector2 current = Utils.castVector3XZ(selfvertices[i%selfvertices.Length].Position);
                    if (Utils.pointInCircle(current, Utils.castVector3XZ(o.origin), o.radius)) return true;

                    // check midpoint
                    Vector2 mid = (current + last) / 2;
                    if (Utils.pointInCircle(mid, Utils.castVector3XZ(o.origin), o.radius)) return true;

                    // chekc mid1
                    Vector2 mid1 = (mid + current) / 2;
                    if (Utils.pointInCircle(mid1, Utils.castVector3XZ(o.origin), o.radius)) return true;

                    // chekc mid2
                    Vector2 mid2 = (mid + last) / 2;
                    if (Utils.pointInCircle(mid2, Utils.castVector3XZ(o.origin), o.radius)) return true;
                    
                    last = current;
                }
                return false;
            }
            else if (other is AARectangleCollidable)
            {
                AARectangleCollidable o = (AARectangleCollidable)other;

                Vector2 last = Utils.castVector3XZ(selfvertices[0].Position);

                for (int i = 0; i <= selfvertices.Length; i++)
                {
                    Vector2 current = Utils.castVector3XZ(selfvertices[i % selfvertices.Length].Position);
                    if (Utils.pointInAARectangle(current, Utils.castVector3XZ(o.origin), o.width, o.length)) return true;

                    // check midpoint
                    Vector2 mid = (current + last) / 2;
                    if (Utils.pointInAARectangle(mid, Utils.castVector3XZ(o.origin), o.width, o.length)) return true;

                    // chekc mid1
                    Vector2 mid1 = (mid + current) / 2;
                    if (Utils.pointInAARectangle(mid1, Utils.castVector3XZ(o.origin), o.width, o.length)) return true;

                    // chekc mid2
                    Vector2 mid2 = (mid + last) / 2;
                    if (Utils.pointInAARectangle(mid2, Utils.castVector3XZ(o.origin), o.width, o.length)) return true;
                    
                    last = current;
                }
                return false;

            }
            return false;
        }

        public override VertexPositionColor[] getPolygonVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[4];

            Matrix r = Matrix.CreateRotationY(angle);

            // 0 
            Vector3 t = new Vector3(width / 2, 1, length / 2) + suboffset;
            t = Vector3.Transform(t, r);
            vertices[0].Position = t + origin;

            // 1 
            t = new Vector3(width / 2, 1, -length / 2) + suboffset;
            t = Vector3.Transform(t, r);
            vertices[1].Position = t + origin;

            // 2
            t = new Vector3(-width / 2, 1, -length / 2) + suboffset;
            t = Vector3.Transform(t, r);
            vertices[2].Position = t + origin;

            // 3
            t = new Vector3(-width / 2, 1, length / 2) + suboffset;
            t = Vector3.Transform(t, r);
            vertices[3].Position = t + origin;

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
