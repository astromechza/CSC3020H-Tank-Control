using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tank_Control.Collidables;
using Tank_Control.Dbg_Drawers;

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

        const float C_GUNMAXANGLE = 0.1f;
        const float C_GUNMINANGLE = -0.9f;
        const float C_GUNANGLESPEED = 0.01f;

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


        // Model angles
        public float turretAngle = 0.0f;
        public float gunAngle = 0.0f;
        float steerAngle = 0.0f;
        public float orientationAngle = 0.0f;
        float[] wheelrotations = new float[4];

        private Vector3 suboffset = new Vector3(0, 0, -25);
        private Vector3 lastGoodPosition = new Vector3(0, 0, 0);
        private float lastGoodOrientation = 0.0f;

        private ColliderDrawer cdrawer;

        public Tank(Game g, Vector3 p) : base(g, p)
        {
            controlState = new TankControlState();
            wheelrotations[0] = 0;
            wheelrotations[1] = 0;
            wheelrotations[2] = 0;
            wheelrotations[3] = 0;

            cdrawer = new ColliderDrawer(g, this.getCollidable());
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
                    game.addFogToEffect(effect);

                    effect.World = currentBoneTransforms[mesh.ParentBone.Index] * wM;
                    effect.View = game.viewMatrix;
                    effect.Projection = game.projectionMatrix;

                    game.addLightingToEffect(effect);                
                }
                mesh.Draw();
            }

            cdrawer.Draw();
        }

        public override void Update(double elapsedMillis)
        {

            // Rotate turret if it should be rotating
            if (controlState.turretRotating != 0.0f)
            {
                turretAngle += 0.05f * controlState.turretRotating;
            }
            // apply current turret rotation to turret bone
            bones[TURRET_GEO].Transform = Matrix.CreateRotationY(turretAngle) * boneOriginTransforms[TURRET_GEO];

            // Raise / Lower Gun depending on key
            if (controlState.gunTilting != 0.0f)
            {
                gunAngle = MathHelper.Clamp(gunAngle + C_GUNANGLESPEED * controlState.gunTilting, C_GUNMINANGLE, C_GUNMAXANGLE);
            }
            // Apply gun pitch to bone
            bones[CANON_GEO].Transform = Matrix.CreateRotationX(gunAngle) * boneOriginTransforms[CANON_GEO];

            // If a steering key is down, modify the steering angle
            if (controlState.steering != 0.0f)
            {
                steerAngle = MathHelper.Clamp(steerAngle + C_STEERSPEED * controlState.steering, -C_MAXSTEER, C_MAXSTEER);
            }
            else 
            {
                // Otherwise, decay the steering angle close to 0
                if (steerAngle < 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle + C_STEERSPEED, -C_MAXSTEER, 0);
                }
                else if (steerAngle > 0)
                {
                    steerAngle = MathHelper.Clamp(steerAngle - C_STEERSPEED, 0, C_MAXSTEER);
                }
            }

            // apply steering angle to both steering racks
            bones[R_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[R_STEER_GEO];
            bones[L_STEER_GEO].Transform = Matrix.CreateRotationY(steerAngle) * boneOriginTransforms[L_STEER_GEO];

            // If a movement key is being held down then accelerate
            if (controlState.moving != 0.0f)           // Accelerate forward
            {
                localVelocity.Z = MathHelper.Clamp(localVelocity.Z + C_ACCELERATION * controlState.moving, C_MAXBACKWARDSPEED, C_MAXFORWARDSPEED);
            }
            else // deccelerate to 0
            {
                if (localVelocity.LengthSquared() > 0.5)
                {
                    localVelocity = Vector3.SmoothStep(localVelocity, Vector3.Zero, C_DECCELERATION);
                }
                else
                {    
                    // clamp to zero
                    localVelocity = Vector3.Zero;
                }
            }
            // Only steer if the local velocity is greater than 1
            if (localVelocity.LengthSquared() > 0)
            {
                if (steerAngle != 0)
                {
                    orientationAngle += (steerAngle / 20) * localVelocity.Z / 20;
                }

                // transform velocity by new orientation
                velocity = Vector3.Transform(localVelocity, orientation);
            }
            else
            {
                velocity = Vector3.Zero;
            }
            
            if (velocity.LengthSquared() > 0)
            {

                // collision TIME!
                bool didCollide = false;
                List<RandomObject> possibles = game.quadTree.GetObjects(this.getRectangle());
                foreach( RandomObject ro in possibles)
                {
                    if (this.getCollidable().collidesWith(ro.getCollidable()))
                    {
                        didCollide = true;
                        break;
                    }
                }

                if (didCollide)
                {
                    velocity = Vector3.Zero;
                    localVelocity = Vector3.Zero;
                    position = lastGoodPosition;
                    orientationAngle = lastGoodOrientation;
                }
                else
                {
                    lastGoodPosition = position;
                    lastGoodOrientation = orientationAngle;
                    position += velocity;
                }
                orientation = Matrix.CreateRotationY(orientationAngle);

                cdrawer.update(this.getCollidable());
            }

            // back wheels
            if (localVelocity.Z > 0)
            {

                wheelrotations[1] += (localVelocity.Z + steerAngle * 5) / 150;
                wheelrotations[0] += (localVelocity.Z - steerAngle * 5) / 150;

                wheelrotations[2] += (localVelocity.Z + steerAngle * 5) / 200;
                wheelrotations[3] += (localVelocity.Z - steerAngle * 5) / 200;
            }
            else if (localVelocity.Z < 0)
            {
                wheelrotations[1] += (localVelocity.Z - steerAngle * 5) / 150;
                wheelrotations[0] += (localVelocity.Z + steerAngle * 5) / 150;

                wheelrotations[2] += (localVelocity.Z - steerAngle * 5) / 200;
                wheelrotations[3] += (localVelocity.Z + steerAngle * 5) / 200;
            }

            bones[L_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[0]) * boneOriginTransforms[L_FRONT_WHEEL_GEO];
            bones[R_FRONT_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[1]) * boneOriginTransforms[R_FRONT_WHEEL_GEO];
            bones[R_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[2]) * boneOriginTransforms[R_BACK_WHEEL_GEO];
            bones[L_BACK_WHEEL_GEO].Transform = Matrix.CreateRotationX(wheelrotations[3]) * boneOriginTransforms[L_BACK_WHEEL_GEO];

            currentTransformNeedsRebuild = true;
        }

        //Input handling for Keyboard
        public void handleInput(KeyboardState ks, MouseState ms)
        {
            controlState.reset();

            if (ks.IsKeyDown(Keys.A))
            {
                controlState.steering = 1.0f;
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                controlState.steering = -1.0f;
            }


            if (ks.IsKeyDown(Keys.W))
            {
                controlState.moving = 1.0f;
            }
            else if (ks.IsKeyDown(Keys.S))
            {
                controlState.moving = -1.0f;
            }


            if (ks.IsKeyDown(Keys.Left))
            {
                controlState.turretRotating = 1.0f;
            }
            else if (ks.IsKeyDown(Keys.Right))
            {
                controlState.turretRotating = -1.0f;
            }

            if (ms.X < 640)
            {
                controlState.turretRotating = (640 - ms.X) / 20.0f;
            }
            else if (ms.X > 640)
            {
                controlState.turretRotating = (640 - ms.X) / 20.0f;
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                controlState.gunTilting = 1.0f;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                controlState.gunTilting = -1.0f;
            }

            if (ms.Y < 360)
            {
                controlState.gunTilting = (360 - ms.Y) / -50.0f;
            }
            else if (ms.Y > 360)
            {
                controlState.gunTilting = (360 - ms.Y) / -50.0f;
            }

        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)this.position.X - 500, (int)this.position.Z - 500, 1000, 1000);
        }

        public Collidable getCollidable()
        {
            return new OARectangleCollidable(this.position, suboffset, this.orientationAngle, 600, 680);
        }

    }

    // Store tank control states in a seperate class
    public class TankControlState
    {
        public float steering = 0.0f;
        public float moving = 0.0f;

        public float turretRotating = 0.0f;
        public float gunTilting = 0.0f;

        public TankControlState()
        {
            reset();
        }

        public void reset()
        {
            steering = moving = 0.0f;
            turretRotating = gunTilting = 0.0f;
        }
    }
}
