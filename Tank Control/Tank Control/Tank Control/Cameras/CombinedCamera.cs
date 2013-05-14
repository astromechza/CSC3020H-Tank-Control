using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tank_Control.Game_Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tank_Control.Cameras
{

    public enum CameraMode { FirstPerson, ThirdPerson, Orbitting };

    public class CombinedCamera
    {
        /* MOVEMENT CONSTANTS */
        const float C_MAXHEIGHT = 8196;
        const float C_MINHEIGHT = 512;
        const float C_DELTAHEIGHT = 16;

        const float C_MAXDISTANCE = 8196;
        const float C_MINDISTANCE = 512;
        const float C_DELTADISTANCE = 16;

        const float C_ORBITSPEED = 0.01f;


        /* MOVEMENT INITIAL */
        public float height = 1024f;
        public float distance = 1536;
        public float looseness = 0.2f;
        float orbitAngle = 0f;

        Vector3 focus;
        Vector3 position;
        Tank tank;
        public CameraMode mode;
        bool posTweanActive = true;
        bool focTweanActive = true;

        public CombinedCamera(Tank target, CameraMode initialMode, Vector3 initialPosition)
        {
            this.mode = initialMode;
            this.tank = target;
            this.focus = target.getPosition();
            this.position = initialPosition;
        }

        /**
         * UpdatePosition: calculate new position and focus
         */
        public void UpdatePosition()
        {
            // different things need to be done depending on the mode
            switch (mode)
            {
                case CameraMode.FirstPerson:
                    {
                        // Calculate position
                        Vector3 pdiff = new Vector3(0, 375, 64f);
                        Matrix po = Matrix.CreateTranslation(0, 0, 36f) * Matrix.CreateRotationY(tank.turretAngle) * Matrix.CreateTranslation(0, 0, -36f) * Matrix.CreateRotationY(tank.orientationAngle);
                        pdiff = Vector3.Transform(pdiff, po);
                        Vector3 targetPosition = tank.getPosition() + pdiff;

                        // First person must not use tweaning, BUT needs tweaning when transitioning between cameras
                        if (posTweanActive && Vector3.DistanceSquared(this.position, targetPosition) > 32f)
                        {
                            this.position = Vector3.SmoothStep(this.position, targetPosition, looseness);
                        }
                        else
                        {
                            posTweanActive = false;
                            this.position = targetPosition;
                        }
                            

                        // Calculate focus
                        Vector3 fdiff = new Vector3(0, 340, 1024f);
                        Matrix fo = Matrix.CreateTranslation(0, 0, 36f) * Matrix.CreateRotationX(tank.gunAngle) * Matrix.CreateRotationY(tank.turretAngle) * Matrix.CreateTranslation(0, 0, -36f) * Matrix.CreateRotationY(tank.orientationAngle);
                        
                        Vector3 focusTarget = tank.getPosition() + Vector3.Transform(fdiff, fo);
                        if (focTweanActive && Vector3.DistanceSquared(this.focus, focusTarget) > 32f)
                        {
                            this.focus = Vector3.SmoothStep(this.focus, focusTarget, looseness);
                        }
                        else
                        {
                            focTweanActive = false;
                            this.focus = focusTarget;
                        }

                        break;
                    }
                case CameraMode.ThirdPerson:
                    {
                        // Calculate position
                        Vector3 tdiff = new Vector3(0, height, -distance);
                        Matrix o = Matrix.CreateRotationY(tank.orientationAngle);
                        Vector3 targetPosition = tank.getPosition() + Vector3.Transform(tdiff, o);
                        this.position = Vector3.SmoothStep(this.position, targetPosition, looseness);

                        // FOCUS

                        Vector3 focusTarget = tank.getPosition() + new Vector3(0, 256f, 0);
                        if (focTweanActive && Vector3.DistanceSquared(this.focus, focusTarget) > 32f)
                        {
                            this.focus = Vector3.SmoothStep(this.focus, focusTarget, looseness);
                        }
                        else
                        {
                            focTweanActive = false;
                            this.focus = focusTarget;
                        }

                        break;
                    }
                case CameraMode.Orbitting:
                    {
                        // Calculate position
                        Vector3 odiff = new Vector3(0, height, -distance);
                        Matrix oo = Matrix.CreateRotationY(orbitAngle);
                        Vector3 orbitPosition = tank.getPosition() + Vector3.Transform(odiff, oo);
                        this.position = Vector3.SmoothStep(this.position, orbitPosition, looseness);

                        // FOCUS
                        Vector3 focusTarget = tank.getPosition() + new Vector3(0, 256f, 0);
                        if (focTweanActive && Vector3.DistanceSquared(this.focus, focusTarget) > 32f)
                        {
                            this.focus = Vector3.SmoothStep(this.focus, focusTarget, looseness);
                        }
                        else
                        {
                            focTweanActive = false;
                            this.focus = focusTarget;
                        }

                        // orbit
                        orbitAngle += C_ORBITSPEED;

                        break;
                    }
                    
            }

        }

        /**
         * GetViewMatrix: produce the view matrix from the current position and focus
         */
        public Matrix getViewMatrix()
        {
            return Matrix.CreateLookAt(position, focus, Vector3.Up);
        }

        /**
         * HandleInput: switch camera modes and activate tweaning between them
         */
        public void handleInput(KeyboardState currentKeyboardState)
        {

            // SWITCH camera
            if (currentKeyboardState.IsKeyDown(Keys.D1))
            {
                this.mode = CameraMode.FirstPerson;
                posTweanActive = true;                      // reactivate tweaning
                focTweanActive = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D2))
            {
                this.mode = CameraMode.ThirdPerson;
                posTweanActive = true;                      // reactivate tweaning
                focTweanActive = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D3))
            {
                this.mode = CameraMode.Orbitting;
                this.orbitAngle = tank.orientationAngle;    // orbit must start at the 3rd person angle
                posTweanActive = true;                      // reactivate tweaning
                focTweanActive = true;
            }

            // Height and distance of camera
            if (currentKeyboardState.IsKeyDown(Keys.L))
            {
                distance = MathHelper.Clamp(distance + C_DELTADISTANCE, C_MINDISTANCE, C_MAXDISTANCE);
            }
            if (currentKeyboardState.IsKeyDown(Keys.O))
            {
                distance = MathHelper.Clamp(distance - C_DELTADISTANCE, C_MINDISTANCE, C_MAXDISTANCE);
            }

            if (currentKeyboardState.IsKeyDown(Keys.I))
            {
                height = MathHelper.Clamp(height + C_DELTAHEIGHT, C_MINHEIGHT, C_MAXHEIGHT);
            }
            if (currentKeyboardState.IsKeyDown(Keys.K))
            {
                height = MathHelper.Clamp(height - C_DELTAHEIGHT, C_MINHEIGHT, C_MAXHEIGHT);
            }

            if (currentKeyboardState.IsKeyDown(Keys.U))
            {
                looseness = MathHelper.Clamp(looseness + 0.005f, 0.05f, 0.9f);
            }
            if (currentKeyboardState.IsKeyDown(Keys.J))
            {
                looseness = MathHelper.Clamp(looseness - 0.005f, 0.05f, 0.9f);
            }


        }


    }
}
