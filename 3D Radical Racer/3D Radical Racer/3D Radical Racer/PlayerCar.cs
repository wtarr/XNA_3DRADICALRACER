using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3D_Radical_Racer
{
    public class PlayerCar : Car
    {
        
        internal bool spin = false; // test
        private int spinTimer; // test
        private float zPositionShift = 0.2f;

        private Boolean shouldGoFaster = false, shouldPoison = false;
        private int goFasterTimer, poisonTimer;
        private int gameTimer;
        private bool shouldFireAMissile;

        public PlayerCar(String carName, Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {
            this.carName = carName;
        }

        public override void Initialize()
        {

            
            forward = new Vector3(0, 0, 1); // for all tracks


            boundingRadius.Radius = 1f;

            Camera.relativeToCar = new Vector2(-50, 30);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {

            model = Game.Content.Load<Model>("Models\\F1MODELStraight");
            
            
            base.LoadContent();
        }

        
        

        public override void Update(GameTime gameTime)
        {

           //   Console.WriteLine(position);
            
            gameTimer++;

            if (gameTimer > 1)
                checkIfOffTrack();
            
            performLapCountForCars();
            
            boundingRadius.Center = position;
            
            accelleration = Vector3.Zero;

            // ACCELLERATE
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                accelerate(gameTime);
            

            if (Keyboard.GetState().IsKeyDown(Keys.A) && this.currentSpeed >= 0 )
                steerLeftGoingForward(gameTime);
            

            if (Keyboard.GetState().IsKeyDown(Keys.A) && this.currentSpeed < 0)
                steerLeftGoingInReverse(gameTime);
            

            if (Keyboard.GetState().IsKeyDown(Keys.D) && this.currentSpeed >= 0 )
                steerRightGoingForward(gameTime);
           
            if (Keyboard.GetState().IsKeyDown(Keys.D) && this.currentSpeed < 0)
                steerRightGoingInReverse(gameTime);
            

            // BRAKE 
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                brake(gameTime);
            
            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D)) // Reset after Steer Left
                resetAfterLeftAndRightKeysReleased();
                      

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                useBonusItem();

            if (Keyboard.GetState().IsKeyUp(Keys.B))
                normalCameraView();
            
            if (Keyboard.GetState().IsKeyDown(Keys.B)) // look behind
                lookBehind();

            //if (Keyboard.GetState().IsKeyDown(Keys.F))
            //{
            //   if (canFireMissile == true)
            //        fireAMissile();

            //}

            if (spin == false)
                checkForCollison();




            if (spin == true && spinTimer < 100)
            {
                spinCar(gameTime, zPositionShift);
                spinTimer++;
            }
            else
            {
                spin = false;
                spinTimer = 0;

            }
            // end 

            if (shouldGoFaster == true)
                applyPowerUpSpeedBoost();
            
            if (shouldPoison == true)
                applyPowerUpPoison();

            if (shouldFireAMissile == true)
                fireAMissile();
                                    
            
            forward = Vector3.Normalize(forward);
            up = Vector3.Normalize(up);
            right = Vector3.Normalize(right);
            GlobalGameData.playerCarSteerLeft.forward = this.forward;
            GlobalGameData.playerCarSteerLeft.up = this.up;
            GlobalGameData.playerCarSteerLeft.right = this.right;
            GlobalGameData.playerCarSteerLeft.position = this.position;
           
            updateCamera();
            
            base.Update(gameTime);
        }

        private void fireAMissile()
        {
            String caller = "player";
            Missile mymissile = new Missile(caller , game, graphics, spriteBatch);
            mymissile.position = this.position;
            mymissile.forward = this.forward;
            mymissile.up = this.up;
            shouldFireAMissile = false;
        }

        private void checkIfOffTrack()
        {
            if (this.OffTrackCollisonOccurred(this.position.X, this.position.Z, offTrack) == false)
            {
                viscocity = 0.6f;
                //Console.WriteLine("on track");

            }
            else
            {
                viscocity = 2f;
                //Console.WriteLine("off track");
            }
        }

        private void applyPowerUpPoison()
        {
            if (poisonTimer < 180) // 3 second hit
            {
                this.accelerationValue = 50; //twice as slow
            }
            else
            {
                // reset everything
                shouldPoison = false;
                poisonTimer = 0;
                this.accelerationValue = 100;
            }

            poisonTimer++;
        }

        private void applyPowerUpSpeedBoost()
        {
            if (goFasterTimer < 120) // 3 second burst
            {
                this.accelerationValue = 200; //twice as fast
            }
            else
            {
                // reset everything
                shouldGoFaster = false;
                goFasterTimer = 0;
                this.accelerationValue = 100;
            }

            goFasterTimer++;
        }

        private static void lookBehind()
        {
            Camera.relativeToCar = new Vector2(70, 40);
            if (Camera.horizonDistance > 0)
                Camera.horizonDistance = Camera.horizonDistance * -1;
        }

        private static void normalCameraView()
        {
            Camera.relativeToCar = new Vector2(-60, 30);
            if (Camera.horizonDistance < 0)
                Camera.horizonDistance = Camera.horizonDistance * -1;
        }

        private static void resetAfterLeftAndRightKeysReleased()
        {
            GlobalGameData.playerCarSteerLeft.Visible = false;
            GlobalGameData.playerCarSteerRight.Visible = false;
            GlobalGameData.playerCar.Visible = true;
        }

        private void steerRightGoingInReverse(GameTime gameTime)
        {
            steerRight(gameTime);

            GlobalGameData.playerCarSteerLeft.Visible = true;
            GlobalGameData.playerCarSteerRight.Visible = false;
            GlobalGameData.playerCar.Visible = false;
        }

        private void steerRightGoingForward(GameTime gameTime)
        {
            steerRight(gameTime);

            GlobalGameData.playerCarSteerRight.Visible = true;
            GlobalGameData.playerCar.Visible = false;
        }

        private void steerLeftGoingInReverse(GameTime gameTime)
        {
            steerLeft(gameTime);

            GlobalGameData.playerCarSteerRight.Visible = true;
            GlobalGameData.playerCarSteerLeft.Visible = false;
            GlobalGameData.playerCar.Visible = false;
        }

        private void steerLeftGoingForward(GameTime gameTime)
        {
            steerLeft(gameTime);

            GlobalGameData.playerCarSteerLeft.Visible = true;
            GlobalGameData.playerCar.Visible = false;
        }

       
        
       
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

       


        private void updateCamera()
        {
            Camera.position = position + forward * Camera.relativeToCar.X + up * Camera.relativeToCar.Y;

            Camera.target = position + forward * Camera.horizonDistance;

            Camera.up = up;
        }


         private void checkForCollison()
        {
              
                foreach (Car b in GlobalGameData.cpuCarList) // check it against the rest of the list
                {
                    if (this.boundingRadius.Intersects(b.boundingRadius))
                    {
                       spin = true;
                    }
                
                }

                               
        } // Check for collison

         public void useBonusItem()
         {
            
             // these two items will be on a timer and so will be handled in the update method
             if (GlobalGameData.playerCar.currentBonusItem.Equals("Go Faster"))
                 shouldGoFaster = true;
             else if (GlobalGameData.playerCar.currentBonusItem.Equals("Poison"))
                 shouldPoison = true;
             else if (GlobalGameData.playerCar.currentBonusItem.Equals("Missile"))
                 shouldFireAMissile = true;
             
             GlobalGameData.playerCar.currentBonusItem = "None";

         }
    }
}

