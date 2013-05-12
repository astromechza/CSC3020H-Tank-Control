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

    class Tank : GameObject
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

        #region Transform Constants
        Matrix scale = Matrix.CreateScale(0.1f);
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

        // Model angles
        float steerAngle = 0.0f;
        float orientationAngle = 0.0f;
        float speed = 0f;

        float[] wheelrotations = new float[4];

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

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = currentBoneTransforms[mesh.ParentBone.Index] * scale;
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

            if (controlState.isSteeringLeft)
            {
                steerAngle = MathHelper.Clamp(steerAngle + 0.08f, -0.79f, 0.79f);
            }
            else if (controlState.isSteeringRight)
            {
                steerAngle = MathHelper.Clamp(steerAngle - 0.08f, -0.79f, 0.79f);
            } 
            else 
            {
                if (steerAngle < 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle + 0.1f, -0.79f, 0);
                }
                else if (steerAngle > 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle - 0.1f, 0, 0.79f);
                }
            }

            bones[R_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[R_STEER_GEO];
            bones[L_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[L_STEER_GEO];

            if (controlState.isMovingForward)           // Accelerate forward
            {
                speed = MathHelper.Clamp(speed + 0.5f, -10f, 10f);
            }
            else if (controlState.isMovingBackward)     // Accelerate backward
            {
                speed = MathHelper.Clamp(speed - 0.5f, -10f, 10f);
            }
            else                                        // Deccelerate
            {
                if (speed > 0)
                {
                    speed = MathHelper.Clamp(speed - 1f, 0, 10f);
                }
                else if (speed < 0)
                {
                    speed = MathHelper.Clamp(speed + 1f, -10f, 0);
                }
            }

            if (speed > 0)
            {
                orientationAngle = orientationAngle + steerAngle / 20;

                Quaternion q = Quaternion.CreateFromAxisAngle(Vector3.Up, orientationAngle);

                Vector3 add = Vector3.Transform(new Vector3(0f, 0f, speed*1.2f), q) ;

                position += add;

                bones[TANK_GEO].Transform =  Matrix.CreateRotationY(orientationAngle) * boneOriginTransforms[TANK_GEO] * Matrix.CreateTranslation(position);

                wheelrotations[0] += speed / 150;
                wheelrotations[1] += speed / 150;
                wheelrotations[2] += speed / 150;
                wheelrotations[3] += speed / 150;

                bones[L_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[0]) * boneOriginTransforms[L_FRONT_WHEEL_GEO];
                bones[R_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[1]) * boneOriginTransforms[R_FRONT_WHEEL_GEO];

                bones[L_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[3]) * boneOriginTransforms[L_BACK_WHEEL_GEO];
                bones[R_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[2]) * boneOriginTransforms[R_BACK_WHEEL_GEO];

            }
            else if (speed < 0)
            {
                orientationAngle = orientationAngle - steerAngle / 20;

                Quaternion q = Quaternion.CreateFromAxisAngle(Vector3.Up, orientationAngle);

                Vector3 add = Vector3.Transform(new Vector3(0f, 0f, speed * 1.5f), q);

                position += add;

                bones[TANK_GEO].Transform =  Matrix.CreateRotationY(orientationAngle) * boneOriginTransforms[TANK_GEO] * Matrix.CreateTranslation(position);
            }





            currentTransformNeedsRebuild = true;
        }

        public void handleInput()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();

            controlState.reset();
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                controlState.isSteeringLeft = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                controlState.isSteeringRight = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                controlState.isMovingForward = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                controlState.isMovingBackward = true;
            }



        }
    }

    public class TankControlState
    {
        public bool isSteeringLeft, isSteeringRight, isMovingForward, isMovingBackward;

        public TankControlState()
        {
            isSteeringLeft = isSteeringRight = isMovingBackward = isMovingForward = false;
        }

        public void reset()
        {
            isSteeringLeft = isSteeringRight = isMovingBackward = isMovingForward = false;
        }
    }
}
