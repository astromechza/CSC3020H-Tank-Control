using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tank_Control.Game_Objects;
using Microsoft.Xna.Framework;

namespace Tank_Control.Cameras
{
    public class FirstPCamera : TrackingCamera
    {
        private Tank tank;

        public FirstPCamera(Tank t) : base (Vector3.Zero, Vector3.Zero)
        {
            tank = t;
            UpdatePosition();

        }
        
        public override void UpdatePosition()
        {
            // Calculate position
            Vector3 pdiff = new Vector3(0, 380, 64f);
            Matrix po = Matrix.CreateTranslation(0, 0, 36.5f) * Matrix.CreateRotationY(tank.turretAngle) * Matrix.CreateTranslation(0, 0, -36.5f) * Matrix.CreateRotationY(tank.orientationAngle);
            pdiff = Vector3.Transform(pdiff, po);
            this.position = tank.getPosition() + pdiff;

            // Calculate focus
            Vector3 fdiff = new Vector3(0, 340, 1024f);
            Matrix fo = Matrix.CreateRotationY(tank.orientationAngle + tank.turretAngle) * Matrix.CreateRotationX(tank.gunAngle);
            fdiff = Vector3.Transform(fdiff, fo);
            this.focus = tank.getPosition() + fdiff;


        }

        public override void handleInput()
        {
            
        }



    }
}
