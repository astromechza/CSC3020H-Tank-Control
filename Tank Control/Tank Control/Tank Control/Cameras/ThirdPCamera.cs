using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tank_Control.Game_Objects;

namespace Tank_Control.Cameras
{
    public class ThirdPCamera : TrackingCamera
    {

        private float distanceFromTarget;
        private float heightAboveGround;
        private float looseness;
        private Tank tank;

        public ThirdPCamera(Vector3 position, Tank tank, float distance, float height, float looseness) : base(position, tank.getPosition())
        {
            this.tank = tank;
            this.distanceFromTarget = distance;
            this.heightAboveGround = height;
            this.looseness = looseness;
        }

        public override void UpdatePosition()
        {
            Vector3 p = getTargetPosition();
            this.position = Vector3.SmoothStep(this.position, p, looseness);
            this.focus = tank.getPosition();
            this.focus.Y += 256f;
        }

        public Vector3 getTargetPosition()
        {

            Vector3 diff = new Vector3(0, heightAboveGround, -distanceFromTarget);

            Matrix o = Matrix.CreateRotationY(tank.orientationAngle);

            diff = Vector3.Transform(diff, o);

            Vector3 t = tank.getPosition();
            t += diff;



            return t;

        }



    }
}
