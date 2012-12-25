/*
 * human car and cpu car are going to have common
 * methods and attributes so preemptively creating a
 * class to cater for this
 * */

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
    public class Car:GameObject
    {
        internal String carName;
        internal int driverPoints;
        internal int numberOfLapsCompletedOnCurrentTrack;
        internal String currentBonusItem = "None";
        internal float turningSpeed = 0.9f;
        internal float currentSpeed = 0.0f;
        internal float accelerationValue = 80.0f;
        internal float decelerationValue = 50.0f; // braking
        internal float viscocity = 0.6f; // to decellerate the car i.e wind resistance / friction
        private float spinSpeed;
        Random random = new Random();
        int randomNumber;
        int timer2, timer3; // random steer left / steer right timer  
        internal float driftSpeed = 0.0000005f;

        internal Boolean checkPoint1Yellow = false;
        internal Boolean checkPoint2Blue = false;
        internal Boolean checkPoint3Green = false;
        internal Boolean lapComplete = true;

        internal int checkPointCounter;

        internal int whiteCounter;

        internal int yellowHit, blueHit, greenHit, whiteHit;

        internal Boolean allowedToMove = false;

        internal BoundingSphere spinAvoidanceBoundingSphere;
        
        
        public Car(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {
            
        } // Constructor

        
        public override void Initialize()
        {

            this.boundingRadius.Radius = 4f;
            base.Initialize();

        } // Initialize

       protected override void LoadContent()
        {
  
            base.LoadContent();

        } // Load Content

       public override void Update(GameTime gameTime)
        {

            if (carLeavesLevelBounds()) // upgrade this later for checking list of cars !!!
                respawnCar();
                      
           currentSpeed -=  viscocity * currentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f; //road and Wind resistance
           velocity = currentSpeed * forward;
           position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
           
           randomiseCPUcarMovement(gameTime);
           
        } //Update

       private void randomiseCPUcarMovement(GameTime gameTime)
       {
           if (timer2 < 30)
           {

               timer2++;

           }
           else
           {

               timer2 = 0;

               randomNumber = random.Next(0, GlobalGameData.cpuCarList.Count);
               if (GlobalGameData.cpuCarList[randomNumber].currentSpeed > 40)
                   GlobalGameData.cpuCarList[randomNumber].steerLeft(gameTime);

           }
           // end 

           if (timer3 < 30)
           {

               timer3++;

           }
           else
           {

               timer3 = 0;

               randomNumber = random.Next(0, GlobalGameData.cpuCarList.Count);
               if (GlobalGameData.cpuCarList[randomNumber].currentSpeed > 40)
                   GlobalGameData.cpuCarList[randomNumber].steerRight(gameTime);

           }
       } // randomiseCPUcarMovement

       private void respawnCar()
       {
           resetPosition();

           if (checkPoint1Yellow == true)
               checkPointCounter--;
           if (checkPoint1Yellow == true && checkPoint2Blue == true)
               checkPointCounter = checkPointCounter - 2;
           if (checkPoint1Yellow == true && checkPoint2Blue == true && checkPoint3Green == true)
               checkPointCounter = checkPointCounter - 3;

           checkPointCounter = checkPointCounter - whiteCounter;
           whiteCounter = 0;
       } // respawnCar

       private bool carLeavesLevelBounds()
       {
           return this.position.X > 400 || this.position.X < -400 || this.position.Z > 400 || this.position.Z < -400;
       } // carLeavesBounds

      

       internal void accelerate(GameTime gameTime)
       {
           if (allowedToMove == true)
                currentSpeed += accelerationValue * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
           
       } // accelerate


        public void steerLeft(GameTime gameTime)
        {
            //play screech sound()
            if (allowedToMove == true)
            {
                if (currentSpeed > 2 || currentSpeed < -2)
                {
                    Matrix m = Matrix.CreateFromAxisAngle(up, turningSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    forward = Vector3.Transform(forward, m);

                    right = Vector3.Transform(right, m);
                }

                
            }
            
            
        } // steerLeft

        public void steerRight(GameTime gameTime)
        {
            if (allowedToMove == true)
            {
                if (currentSpeed > 2 || currentSpeed < -2)
                {
                    
                    Matrix m = Matrix.CreateFromAxisAngle(up, -turningSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    forward = Vector3.Transform(forward, m);

                    right = Vector3.Transform(right, m);
                }
            }

        } // steerRight

        

        public void brake(GameTime gameTime)
        {
            if (allowedToMove == true)
                currentSpeed -= decelerationValue * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

        } // brake

        

        public void spinCar(GameTime gameTime, float zPositionShift)
        {

            if (allowedToMove == true)
            {
                spinSpeed = 4f;

                velocity = Vector3.Zero;
                Matrix m = Matrix.CreateFromAxisAngle(up, -spinSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                forward = Vector3.Transform(forward, m);
                right = Vector3.Transform(right, m);


                position.Z += zPositionShift;

            }
        } // spinCar

        public void resetPosition()
        {
            checkPoint1Yellow = false;
            checkPoint2Blue = false;
            checkPoint3Green = false;


            this.forward = new Vector3(0, 0, 1);
            this.right = Vector3.Right;
            this.position = new Vector3(-244, 0, 47); // if pos no 1

            this.velocity = Vector3.Zero;
            this.currentSpeed = 0.0f;
        } // resetPosition

        public void removeCar()
        {
            Game.Components.Remove(this);
        } // removeCar


        public void performLapCountForCars()
        {
            
            // Checkpoint 1 YELLOW
            if (this.boundingRadius.Intersects(GlobalGameData.checkPoint1))
            {

                this.yellowHit++;

                this.checkPoint1Yellow = true;
                if (this.yellowHit == 1)
                {
                    //this.whiteCounter = 0;
                    this.checkPointCounter++;
                }


            }


            // Checkpoint 2 BLUE
            if (this.boundingRadius.Intersects(GlobalGameData.checkPoint2))
            {
                if (this.checkPoint1Yellow == false)
                {
                    //this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                    this.whiteCounter = 0;
                    this.resetPosition();
                    this.checkPointCounter = this.checkPointCounter - 2;
                }
                else
                {
                    this.blueHit++;
                    this.checkPoint2Blue = true;
                    if (this.blueHit == 1)
                    {
                        //this.whiteCounter = 0;
                        this.checkPointCounter++;
                    }
                }
            }



            // Checkpoint 3 GREEN
            if (this.boundingRadius.Intersects(GlobalGameData.checkPoint3))
            {
                if (this.checkPoint1Yellow == false && this.checkPoint2Blue == false)
                {
                    this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                    this.whiteCounter = 0;
                    this.resetPosition();
                    this.checkPointCounter = this.checkPointCounter - 3;
                }
                else
                {
                    this.greenHit++;
                    this.checkPoint3Green = true;
                    if (this.greenHit == 1)
                    {

                        //this.whiteCounter = 0;
                        this.checkPointCounter++;
                    }

                }
            }


            // Skipped Check point 2
            if (this.checkPoint1Yellow == true &&
                this.checkPoint3Green == true &&
                this.checkPoint2Blue == false)
            {
                this.resetPosition();
                this.checkPointCounter--;
                //this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                //this.whiteCounter = 0;
            }

            // Hit first checkpoint reversed direction and traveled towards start finish
            if (this.boundingRadius.Intersects(GlobalGameData.currentTrackModel.startFinish) &&
                this.checkPoint1Yellow == true &&
                this.checkPoint2Blue == false &&
                this.checkPoint3Green == false)
            {
                this.resetPosition();
                this.checkPointCounter--;
                //this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                //this.whiteCounter = 0;
            }
            // Hit first and second checkpoint and reversed direction and traveled towards start finish
            if (this.boundingRadius.Intersects(GlobalGameData.currentTrackModel.startFinish) &&
                this.checkPoint1Yellow == true &&
                this.checkPoint2Blue == true &&
                this.checkPoint3Green == false)
            {
                this.resetPosition();
                this.checkPointCounter = this.checkPointCounter - 2;
                //this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                //this.whiteCounter = 0;
            }

            // Hit all 3 checkpoints but travelled back to Checkpoint 2
            if (this.boundingRadius.Intersects(GlobalGameData.checkPoint2) &&
                this.checkPoint1Yellow == true &&
                this.checkPoint3Green == true)
            {
                this.resetPosition();
                this.checkPointCounter = this.checkPointCounter - 3;
                //this.checkPointCounter = this.checkPointCounter - this.whiteCounter;
                //this.whiteCounter = 0;
            }


            // All conditions satisfied then a lap can be counted
            if (this.boundingRadius.Intersects(GlobalGameData.currentTrackModel.startFinish) &&
                this.checkPoint1Yellow == true &&
                this.checkPoint2Blue == true &&
                this.checkPoint3Green == true)
            {

                this.checkPoint1Yellow = false;
                this.checkPoint2Blue = false;
                this.checkPoint3Green = false;
                this.lapComplete = true;
                this.numberOfLapsCompletedOnCurrentTrack++;
                this.yellowHit = 0;
                this.blueHit = 0;
                this.greenHit = 0;

                //this.whiteCounter = 0;
                //}
                if (isTheFirstCarToCrossLine())
                    GlobalGameData.racePosition.Add(this);  // allow very first car to cross the line to be added to the list

                if (carHasCompletedSameNumberOfLapsAsLeader()) // the car has completed the same number of laps as the leader
                    GlobalGameData.racePosition.Add(this);

                if (GlobalGameData.racePosition.Count > 0 && 
                    GlobalGameData.racePosition[0].Equals(this) &&
                    GlobalGameData.racePosition[0].numberOfLapsCompletedOnCurrentTrack != GlobalGameData.currentTrackModel.numberOfLaps &&
                    GlobalGameData.racePosition.Contains(this)) 
                {
                    GlobalGameData.racePosition.Clear();
                    GlobalGameData.racePosition.Add(this);

                }


                foreach (Car c in GlobalGameData.racePosition)
                {
                    if (this.numberOfLapsCompletedOnCurrentTrack > c.numberOfLapsCompletedOnCurrentTrack &&
                        GlobalGameData.racePosition.Contains(this)) // this suggests we have a new leader
                    {
                        GlobalGameData.racePosition.Clear();
                        GlobalGameData.racePosition.Add(this);
                        break;
                    }

                }

               
            }


        }

        private bool carHasCompletedSameNumberOfLapsAsLeader()
        {
            return this.numberOfLapsCompletedOnCurrentTrack == GlobalGameData.racePosition[0].numberOfLapsCompletedOnCurrentTrack;
        }

        private bool isTheFirstCarToCrossLine()
        {
            return GlobalGameData.racePosition.Count == 0 && !GlobalGameData.racePosition.Contains(this);
        }

    } // Class
}
