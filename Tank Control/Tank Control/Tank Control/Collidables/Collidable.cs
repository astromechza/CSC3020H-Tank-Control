using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Tank_Control.Collidables
{
    public abstract class Collidable
    {
        public abstract bool collidesWith(Collidable other);
        public abstract VertexPositionColor[] getPolygonVertices();
        public abstract short[] getPolygonLineList();
    }
}
