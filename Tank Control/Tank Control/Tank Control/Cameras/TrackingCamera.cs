using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tank_Control.Cameras
{
    public abstract class TrackingCamera
    {
        protected Vector3 position;
        protected Vector3 focus;

        public TrackingCamera(Vector3 position, Vector3 focus)
        {
            this.position = position;
            this.focus = focus;
        }

        public Matrix getViewMatrix()
        {
            return Matrix.CreateLookAt(position, focus, Vector3.Up);
        }

        public abstract void UpdatePosition();

        public abstract void handleInput();
    }
}
