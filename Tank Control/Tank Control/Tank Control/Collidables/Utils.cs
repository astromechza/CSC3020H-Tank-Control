using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tank_Control.Collidables
{
    class Utils
    {

        public static bool pointInCircle(Vector2 point, Vector2 center, float radius)
        {
            return Vector2.Distance(point, center) < radius;
        }

        public static bool pointInAARectangle(Vector2 point, Vector2 origin, int width, int height)
        {
            if (point.X > (origin.X + width / 2)) return false;
            if (point.X < (origin.X - width / 2)) return false;
            if (point.Y > (origin.Y + height / 2)) return false;
            if (point.Y < (origin.Y - height / 2)) return false;
            return true;
        }

        public static Vector2 castVector3XZ(Vector3 input)
        {
            return new Vector2(input.X, input.Z);
        }


    }
}
