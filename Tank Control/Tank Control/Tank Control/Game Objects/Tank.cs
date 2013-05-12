using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Tank_Control.Game_Objects
{

    public class Tank : GameObject
    {
        #region BONE INDICES
        const int TANK_GEO = 0;
        const int R_ENGINE_GEO = 1;
        const int R_BACK_WHEEL_GEO = 2;
        const int R_STEER_GEO = 3;
        const int R_FRONT_WHEEL_GEO = 4;
        const int L_ENGINE_GEO = 5;
        const int L_BACK_WHEEL_GEO = 6;
        const int L_STEER_GEO = 7;
        const int L_FRONT_WHEEL_GEO = 8;
        const int TURRET_GEO = 9;
        const int CANON_GEO = 10;
        const int HATCH_GEO = 11;
        #endregion

        #region Speed Constants

        const float C_MAXFORWARDSPEED = 15.0f;
        const float C_MAXBACKWARDSPEED = -10.0f;
        const float C_ACCELERATION = 0.5f;
        const float C_DECCELERATION = 0.15f;

        const float C_MAXSTEER = 0.5f;
        const float C_STEERSPEED = 0.08f;

        const float C_GUNMAXANGLE = 0.0f;
        const float C_GUNMINANGLE = -0.6f;
        const float C_GUNANGLESPEED = 0.004f;

        #endregion

        Model model;

        // Bone lookup matrix
        ModelBone[] bones;

        // Store original transforms away from the origin
        Matrix[] boneOriginTransforms;

        // Keep last bone transforms
        Matrix[] currentBoneTransforms;
        bool currentTransformNeedsRebuild = true;

        // Control states
        TankControlState controlState;

        Matrix orientation = Matrix.Identity;
        Vector3 localVelocity = Vector3.Zero;
        Vector3 velocity = Vector3.Zero;

        public float turretAngle = 0.0f;
        public float gunAngle = 0.0f;

        // Model angles
        float steerAngle = 0.0f;
        public float orientationAngle = 0.0f;

        float[] wheelrotations = new float[4];
        // 

        public Tank(Game g, Vector3 p) : base(g, p)
        {
            controlState = new TankControlState();
            wheelrotations[0] = 0;
            wheelrotations[1] = 0;
            wheelrotations[2] = 0;
            wheelrotations[3] = 0;
        }

        /* LoadContent
         * Load the tank model and bones, initialise bones and bone transforms 
         */
        public override void LoadContent(ContentManager contentMan)
        {
            model = contentMan.Load<Model>("tank");

            // Store bones and original transform matrices
            bones = new ModelBone[model.Bones.Count];
            boneOriginTransforms = new Matrix[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                bones[i] = model.Bones[i];
                boneOriginTransforms[i] = model.Bones[i].Transform;
            }

        }
        
        public override void Draw()
        {            
            // Avoid rebuilding on every draw call
            if (currentTransformNeedsRebuild)
            {
                currentBoneTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(currentBoneTransforms);
                currentTransformNeedsRebuild = false;
            }

            Matrix wM = orientation * Matrix.CreateTranslation(this.position)  ;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.FogEnabled = true;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = 2048;
                    effect.FogEnd = 4096;

                    effect.World = currentBoneTransforms[mesh.ParentBone.Index] * wM;
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;                    
                }
                mesh.Draw();
            }
        }

        public override void Update(double elapsedMillis)
        {

            // TURRET

            if (controlState.turretRotatingLeft)
            {
                turretAngle += 0.05f;
            }
            else if (controlState.turretRotatingRight)
            {
                turretAngle -= 0.05f;
            }

            bones[TURRET_GEO].Transform = Matrix.CreateRotationY(turretAngle) * boneOriginTransforms[TURRET_GEO];

            if (controlState.gunMovingUp)
            {
                gunAngle = MathHelper.Clamp(gunAngle + C_GUNANGLESPEED, C_GUNMINANGLE, C_GUNMAXANGLE);
            }
            else if (controlState.gunMovingDown)
            {
                gunAngle = MathHelper.Clamp(gunAngle - C_GUNANGLESPEED, C_GUNMINANGLE, C_GUNMAXANGLE);
            }
            bones[CANON_GEO].Transform = Matrix.CreateRotationX(gunAngle) * boneOriginTransforms[CANON_GEO];

            // STEERAGE

            if (controlState.isSteeringLeft)
            {
                steerAngle = MathHelper.Clamp(steerAngle + C_STEERSPEED, -C_MAXSTEER, C_MAXSTEER);
            }
            else if (controlState.isSteeringRight)
            {
                steerAngle = MathHelper.Clamp(steerAngle - C_STEERSPEED, -C_MAXSTEER, C_MAXSTEER);
            } 
            else 
            {
                if (steerAngle < 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle + C_STEERSPEED, -C_MAXSTEER, 0);
                }
                else if (steerAngle > 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle - C_STEERSPEED, 0, C_MAXSTEER);
                }
            }

            bones[R_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[R_STEER_GEO];
            bones[L_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[L_STEER_GEO];


            if (controlState.isMovingForward)           // Accelerate forward
            {
                localVelocity.Z = MathHelper.Clamp(localVelocity.Z + C_ACCELERATION, C_MAXBACKWARDSPEED, C_MAXFORWARDSPEED);
            }
            else if (controlState.isMovingBackward)     // Accelerate backward
            {
                localVelocity.Z = MathHelper.Clamp(localVelocity.Z - C_ACCELERATION, C_MAXBACKWARDSPEED, C_MAXFORWARDSPEED);
            }
            else
            {
                localVelocity = Vector3.SmoothStep(localVelocity, Vector3.Zero, C_DECCELERATION);
            }

            if (localVelocity.LengthSquared() > 0 )
            {
                if (steerAngle != 0)
                {
                    orientationAngle += (steerAngle / 20) * localVelocity.Z / 20;
                    orientation = Matrix.CreateRotationY(orientationAngle);
                }

                velocity = Vector3.Transform(localVelocity, orientation);
            }
            
            if (velocity.LengthSquared() > 0)
            {
                position += velocity;
            }


            wheelrotations[0] += (localVelocity.Z + steerAngle*2) / 130;
            wheelrotations[1] += (localVelocity.Z - steerAngle*2) / 130;

            wheelrotations[2] += (localVelocity.Z + steerAngle) / 200;
            wheelrotations[3] += (localVelocity.Z - steerAngle) / 200;

            bones[L_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[0]) * boneOriginTransforms[L_FRONT_WHEEL_GEO];
            bones[R_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[1]) * boneOriginTransforms[R_FRONT_WHEEL_GEO];
            bones[R_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[2]) * boneOriginTransforms[R_BACK_WHEEL_GEO];
            bones[L_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[3]) * boneOriginTransforms[L_BACK_WHEEL_GEO];

            currentTransformNeedsRebuild = true;
        }

        public void handleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            controlState.reset();
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                controlState.isSteeringLeft = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                controlState.isSteeringRight = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                controlState.isMovingForward = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                controlState.isMovingBackward = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                controlState.turretRotatingLeft = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                controlState.turretRotatingRight = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                controlState.gunMovingUp = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                controlState.gunMovingDown = true;
            }


        }
    }

    public class TankControlState
    {
        // movement
        public bool isSteeringLeft, isSteeringRight, isMovingForward, isMovingBackward;
        // turret
        public bool turretRotatingLeft, turretRotatingRight, gunMovingUp, gunMovingDown = false;

        public TankControlState()
        {
            isSteeringLeft = isSteeringRight = isMovingBackward = isMovingForward = false;
            turretRotatingLeft = turretRotatingRight = gunMovingUp = gunMovingDown = false;
        }

        public void reset()
        {
            isSteeringLeft = isSteeringRight = isMovingBackward = isMovingForward = false;
            turretRotatingLeft = turretRotatingRight = gunMovingUp = gunMovingDown = false;
        }
    }
}
