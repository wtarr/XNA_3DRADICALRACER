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
    public class CPUCar : Car
    {

        internal bool spin = false; // test
        private int spinTimer; // test
        private float zPositionShift = -0.5f;
       
        internal Vector3 feelerFront_Pos, feelerLeft_Pos, feelerRight_Pos;
        internal Vector3 feelerFront_Forward, feelerLeft_Forward, feelerRight_Forward;
        internal Vector3 feelerFront_Up, feelerLeft_Up, feelerRight_Up;
        internal Vector3 feelerFront_Right, feelerLeft_Right, feelerRight_Right;
        internal bool spin2 = false;
        
        internal bool shouldSteerLeft, shouldSteerRight;


        private Game game;
       



        Boolean shouldGoFaster = false, shouldPoison = false, shouldFireMissile = false;
        int goFasterTimer, poisonTimer;
        
        //Constructor
        public CPUCar(String carName, Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Vector3 position)
            : base(game, graphics, spriteBatch)
        {

            this.carName = carName;
            this.position = position;
            this.game = game;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
           
        }

        public override void Initialize()
        {

           
            forward = new Vector3(0, 0, 1); // for track 2

            turningSpeed = 3f;

            boundingRadius.Radius = 1f;

            accelerationValue = 130;
            

            spinAvoidanceBoundingSphere = new BoundingSphere();
            spinAvoidanceBoundingSphere.Center = this.position;
            spinAvoidanceBoundingSphere.Radius = 2f;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            model = Game.Content.Load<Model>("Models\\F1MODELStraight");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            boundingRadius.Center = position;
            spinAvoidanceBoundingSphere.Center = position;

            performLapCountForCars();

            updateFrontFeelerPosition();
            updateRightFeelerPosition();
            updateLeftFeelerPosition();
                        
            if (spin == false)
                checkForCollison(gameTime);

            manageCarSpinManeuver(gameTime);
            
            checkFrontFeelersState(gameTime);
            checkLeftFeelerState();
            checkRightFeelerState();


            handleCarTrackOverRun(gameTime);
                

            if (shouldSteerLeft == true)
                steerLeft(gameTime);
            if (shouldSteerRight == true)
                steerRight(gameTime);

            //generate random times at which to check and if holds an Bonus item and then use it
            shouldUseBonusItem();

            applyPowerUpIfApplicable();

            
                        
            base.Update(gameTime);
        } // Update

        private void checkRightFeelerState()
        {
            // Right feeler
            if (this.OffTrackCollisonOccurred(feelerRight_Pos.X, feelerRight_Pos.Z, offTrack) == true)
            {
                
                shouldSteerLeft = true;
            }
            else
            {
                viscocity = 0.9f;
                shouldSteerLeft = false;
            }
            
        }

        private void checkLeftFeelerState()
        {
            // Left feeler
            if (this.OffTrackCollisonOccurred(feelerLeft_Pos.X, feelerLeft_Pos.Z, offTrack) == true)
            {
                viscocity = 2f;
                shouldSteerRight = true;

            }
            else
            {
                viscocity = 0.9f;
                shouldSteerRight = false;
            }
        }

        private void checkFrontFeelersState(GameTime gameTime)
        {
            
            if (this.OffTrackCollisonOccurred(feelerFront_Pos.X, feelerFront_Pos.Z, offTrack) == false)
            {
                viscocity = 0.9f;
                
            }
            else
            {
                
                brake(gameTime);
              
            }
        }

        private void handleCarTrackOverRun(GameTime gameTime)
        {
            // all three feelers
            if ((this.OffTrackCollisonOccurred(feelerRight_Pos.X, feelerRight_Pos.Z, offTrack) == true) &&
                (this.OffTrackCollisonOccurred(feelerLeft_Pos.X, feelerLeft_Pos.Z, offTrack) == true) &&
                (this.OffTrackCollisonOccurred(feelerFront_Pos.X, feelerFront_Pos.Z, offTrack) == true))
            {
                viscocity = 0.1f; // reverse fast
                brake(gameTime);
                turningSpeed = 0.5f;
                steerLeft(gameTime);
            }
            else
            {
                turningSpeed = 2.1f;
                viscocity = 0.9f;
                accelerate(gameTime);
            }
        }

        private void manageCarSpinManeuver(GameTime gameTime)
        {
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
        }

        private void applyPowerUpIfApplicable()
        {
            if (shouldGoFaster == true)
            {
                if (goFasterTimer < 120) // 2 second burst
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

            if (shouldPoison == true)
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

            if (shouldFireMissile == true)
            {
                
                String caller = "cpu";
                Missile mymissile = new Missile(caller, game, graphics, spriteBatch);
                mymissile.position = this.position;
                mymissile.forward = this.forward;
                mymissile.up = this.up;
                shouldFireMissile = false;
        
            }
        }

        private void updateLeftFeelerPosition()
        {
            // Feeler Left
            feelerLeft_Pos = this.feelerFront_Pos + this.right * 30 + this.feelerFront_Forward * 5;
            feelerLeft_Forward = this.feelerFront_Forward;
            feelerLeft_Up = this.feelerFront_Up;
            feelerLeft_Right = this.feelerFront_Right;

            GlobalGameData.feelerLeft_Pos = this.feelerLeft_Pos;
            GlobalGameData.feelerLeft_Forward = this.feelerLeft_Forward;
            GlobalGameData.feelerLeft_Up = this.feelerLeft_Up;
            GlobalGameData.feelerLeft_Right = this.feelerLeft_Right;
        }

        private void updateRightFeelerPosition()
        {
            // Feeler Right
            feelerRight_Pos = feelerFront_Pos - this.right * 30 + this.feelerFront_Forward * 5;
            feelerRight_Forward = this.feelerFront_Forward;
            feelerRight_Up = this.feelerFront_Up;
            feelerRight_Right = this.feelerFront_Right;

            GlobalGameData.feelerRight_Pos = this.feelerRight_Pos;
            GlobalGameData.feelerRight_Forward = this.feelerRight_Forward;
            GlobalGameData.feelerRight_Up = this.feelerRight_Up;
            GlobalGameData.feelerRight_Right = this.feelerRight_Right;
        }

        private void updateFrontFeelerPosition()
        {
            // Feeler Front
            feelerFront_Pos = this.position + this.forward * 50;
            feelerFront_Forward = this.forward;
            feelerFront_Up = this.up;
            feelerFront_Right = this.right;

            GlobalGameData.feelerFront_Pos = this.feelerFront_Pos;
            GlobalGameData.feelerFront_Forward = this.feelerFront_Forward;
            GlobalGameData.feelerFront_Up = this.feelerFront_Up;
            GlobalGameData.feelerFront_Right = this.feelerFront_Right;
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void shouldUseBonusItem()
        {
           
            if (this.currentBonusItem.Equals("Go Faster"))
            {
                shouldGoFaster = true;
                this.currentBonusItem = "None";
            }
            else if (this.currentBonusItem.Equals("Poison"))
            {
                shouldPoison = true;
                this.currentBonusItem = "None";
            }
            else if (this.currentBonusItem.Equals("Missile"))
            {
                shouldFireMissile = true;
                this.currentBonusItem = "None";
            }
      
        }


        private void checkForCollison(GameTime gameTime)
        {
            

             if (this.boundingRadius.Intersects(GlobalGameData.playerCar.boundingRadius))
                {
                    spin = true;
                }

         

            foreach (Car x in GlobalGameData.cpuCarList)
            {
               
                if (this.spinAvoidanceBoundingSphere.Intersects(x.spinAvoidanceBoundingSphere) && !this.Equals(x))
                {
                    x.brake(gameTime);
                    x.steerRight(gameTime);
                    this.steerLeft(gameTime);
                    this.accelerate(gameTime);
                }
            }


        } // Check for collison


    }
}
