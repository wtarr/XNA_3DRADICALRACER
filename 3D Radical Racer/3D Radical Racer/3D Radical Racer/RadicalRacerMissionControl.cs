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
    
    public class RadicalRacerMissionControl : Microsoft.Xna.Framework.Game
    {
        public enum GameState { mainMenu, raceStartCountDown, running, raceFinished, paused, gameOver }
        public GameState gameState;
        
        public GraphicsDeviceManager graphics;
       
        SpriteBatch spriteBatch;
        
        SpriteFont basicFont, HUDfont, raceResultsFont, countDownFont;
        
        Texture2D startScreenImage, winImage, podiumImage;
        //private int luckyBoxTimer = 0;
        private int slickTimer = 0;
        private  int colorAreaToCheck_Width = 2;
        private  int colorAreaToCheck_Height = 2;
        private RenderTarget2D trackRender;  // allows to make use of your own back buffer (double buffering is used normally allow fluid game motion) good explaination -> http://geekswithblogs.net/mikebmcl/archive/2011/02/18/xna-rendertarget2d-sample.aspx
        private Color cp2blue = new Color (0, 0, 255, 255);
        private Color cp3green = new Color(0, 255, 0, 255);
        private Color cp1yellow = new Color(255, 255, 0, 255);
        private int myX , myZ; // used for finding checkpoints before race starts
        private Rectangle rect1;
        private bool CheckPointsTestedForAndFound = false;
        private float mappedModelX, mappedModelZ;
        private bool canAssignPoints;
        private bool canFillRemainingPositions; // for use if player car finished before others
        private bool startCountDown = true;
        private int countDown = 3;
        private int countDownTickTimer;
        private String driverStandingAsAsString = "";
        private bool sortAndExtractDriversChampionshipStandings = true; // sort only needs to happen once

        private Video splashVideo;
        private VideoPlayer myPlayer;
        private Texture2D videoSplashTexture;
        private bool playAgain = true;

        
      
       
        public RadicalRacerMissionControl()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";

            // http://msdn.microsoft.com/en-us/library/bb195024.aspx
            //this.graphics.PreferredBackBufferWidth = 480;
            //this.graphics.PreferredBackBufferHeight = 800;
            //this.graphics.IsFullScreen = true;
            //Console.WriteLine(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            //Console.WriteLine(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
            //Console.WriteLine(Window.ClientBounds.Height);
            //Console.WriteLine(Window.ClientBounds.Width);

            gameState = GameState.mainMenu;

            rect1 = new Rectangle(0, 0, colorAreaToCheck_Width, colorAreaToCheck_Height);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GlobalGameData.currentTrackModel = new Track(this, graphics, spriteBatch);
            GlobalGameData.currentTestTexture = Content.Load<Texture2D>("Textures\\track1trackonly");
            

            GlobalGameData.cpuCarList = new List<CPUCar>();
            //GlobalGameData.carList = new List<Car>(); // for sorting track positions
            
            GlobalGameData.playerCar = new PlayerCar("Player One" ,this, graphics, spriteBatch);  // Car 1
            GlobalGameData.playerCar.position = new Vector3(-213, 0, 0);

            //GameData.carList.Add(GameData.playerCar);

            GlobalGameData.playerCarSteerLeft = new PlayerCarSteerLeft(this, graphics, spriteBatch);
            GlobalGameData.playerCarSteerLeft.position = new Vector3(-213, 0, 0);

            GlobalGameData.playerCarSteerRight = new PlayerCarSteerRight(this, graphics, spriteBatch);
            GlobalGameData.playerCarSteerRight.position = new Vector3(-213, 0, 0);
            

            GlobalGameData.cpuCar = new CPUCar("Car 2", this, graphics, spriteBatch, new Vector3(-213, 0, 30));
            GlobalGameData.cpuCarList.Add(GlobalGameData.cpuCar);
            
            GlobalGameData.cpuCar = new CPUCar("Car 3", this, graphics, spriteBatch, new Vector3(-246, 0, 45));
            GlobalGameData.cpuCarList.Add(GlobalGameData.cpuCar);
            
            GlobalGameData.cpuCar = new CPUCar("Car 4",  this, graphics, spriteBatch, new Vector3(-246, 0, 19));
            GlobalGameData.cpuCarList.Add(GlobalGameData.cpuCar);
                                    
            Hoarding hoarding = new Hoarding(this, graphics, spriteBatch);
            Horizon horizon = new Horizon(this, graphics, spriteBatch);
            StartFinishSignage startFinishSignage = new StartFinishSignage(this, graphics, spriteBatch);
            Shed shed = new Shed(this, graphics, spriteBatch);

            GlobalGameData.luckybox = new LuckyBox(this, graphics, spriteBatch);

            GlobalGameData.slick = new Obstacle(this, graphics, spriteBatch);

            GlobalGameData.checkPoint1 = new BoundingSphere(Vector3.Zero, 40f);

            GlobalGameData.checkPoint2 = new BoundingSphere(Vector3.Zero, 40f);

            GlobalGameData.checkPoint3 = new BoundingSphere(Vector3.Zero, 40f);

            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            // TODO: use this.Content to load your game content here
            basicFont = Content.Load<SpriteFont>("LapsCompletes");
            HUDfont = Content.Load<SpriteFont>("HUDfont");
            raceResultsFont = Content.Load<SpriteFont>("ResultsFont");
            countDownFont = Content.Load<SpriteFont>("CountDownFont");
            startScreenImage = Content.Load<Texture2D>("Textures\\racingStartScreen");
            winImage = Content.Load<Texture2D>("Textures\\winner");
            podiumImage = Content.Load<Texture2D>("Textures\\podium");
        
            trackRender = new RenderTarget2D(graphics.GraphicsDevice, colorAreaToCheck_Width, colorAreaToCheck_Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            splashVideo = Content.Load<Video>("Textures\\introSplash");
            //introVideo = Content.Load<Video>("Textures\\introVideo");
            myPlayer = new VideoPlayer();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //if (gameTime.IsRunningSlowly) Console.WriteLine("slow");
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            if (gameState == GameState.running)
            {
                UpdateGameRunning();
                base.Update(gameTime);
            }
            else if (gameState == GameState.mainMenu)
            {
                UpdateGameMainMenu();
            }
            else if (gameState == GameState.raceFinished)
            {
                UpdateGameRaceFinished();
            }
            else if (gameState == GameState.gameOver)
            { 
                // Nothing to do here but leave in place anyway
            }
        }

        private void UpdateGameRaceFinished()
        {
            FillRemainingPositions();
            AssignPoints();

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter))
            {

                if (GlobalGameData.nextTrack == 4)
                {
                    gameState = GameState.gameOver;
                    //GameData.playerCar.numberOfLapsCompletedOnCurrentTrack = 0;
                }
                else
                {
                    goToNextTrack();
                }
            }
        }

        private void UpdateGameMainMenu()
        {
            if (CheckPointsTestedForAndFound == false)
            {
                FindCheckPoints();
                CheckPointsTestedForAndFound = true;
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter))
            {
                gameState = GameState.running;

                AllowCarsToMove(true);
            }

            // myPlayer.Play(introVideo);
            if (myPlayer.State == MediaState.Stopped && playAgain == true)
            {
                myPlayer.IsLooped = false;
                myPlayer.Play(splashVideo);
                playAgain = false;
            }
        }

        private void UpdateGameRunning()
        {
            GlobalGameData.luckybox.generatePosition();
            GlobalGameData.slick.generatePosition();
            ManageOilSlickObstaclePositioning();
            StartCountdown();
            CheckForRaceOver();
            canFillRemainingPositions = true;
            canAssignPoints = true;
            //myPlayer.Stop(); //stop the video
        }

        private void goToNextTrack()
        {
            CheckPointsTestedForAndFound = false;
            LoadNextTrack();
            if (CheckPointsTestedForAndFound == false)
            {
                FindCheckPoints();
                CheckPointsTestedForAndFound = true;
            }
            ResetAllCars();
            AllowCarsToMove(true);
            //GameData.lb.generatePosition();
            GlobalGameData.racePosition.Clear();
            gameState = GameState.running;
        }

        private void ManageOilSlickObstaclePositioning()
        {
            // Every 600 cycles the Oil slick is positioned to
            // a different position on the track.
            if (slickTimer > 600)
            {
                slickTimer = 0;
                GlobalGameData.slick.xPos = 0;
                GlobalGameData.slick.zPos = 0;
                GlobalGameData.slick.generatePosition();
            }

            slickTimer++;
        }

        private void StartCountdown()
        {
            //Start the race countdown
            if (startCountDown == true)
            {

                AllowCarsToMove(false);

                countDownTickTimer++;

                if (countDownTickTimer > 60)
                {
                    countDown--;
                    countDownTickTimer = 0;
                }



                if (countDown < 0)
                {
                    startCountDown = false;
                    countDown = 3;
                    countDownTickTimer = 0;
                    AllowCarsToMove(true);
                }

            }
        }

       
        private void AllowCarsToMove( Boolean yesNo )
        {
            // This method is used to set the cars to be able to move or not
            GlobalGameData.playerCar.allowedToMove = yesNo;
            GlobalGameData.cpuCarList[0].allowedToMove = yesNo;
            GlobalGameData.cpuCarList[1].allowedToMove = yesNo;
            GlobalGameData.cpuCarList[2].allowedToMove = yesNo;
            
        }


        private void CheckForRaceOver()
        {
            // checks if any of the cars have completed the required number of laps and if so 
            // determines their actions and updates the game state

            if (GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack ||
               GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[0].numberOfLapsCompletedOnCurrentTrack ||
               GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[1].numberOfLapsCompletedOnCurrentTrack ||
               GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[2].numberOfLapsCompletedOnCurrentTrack)
            {

                if (GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack)
                {
                    
                    GlobalGameData.playerCar.velocity = Vector3.Zero;
                    GlobalGameData.playerCar.allowedToMove = false;
                }
                if (GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[0].numberOfLapsCompletedOnCurrentTrack)
                {
                    
                    GlobalGameData.cpuCarList[0].velocity = Vector3.Zero;
                    GlobalGameData.cpuCarList[0].allowedToMove = false;
                }
                if (GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[1].numberOfLapsCompletedOnCurrentTrack)
                {
                    
                    GlobalGameData.cpuCarList[1].velocity = Vector3.Zero;
                    GlobalGameData.cpuCarList[1].allowedToMove = false;
                }
                if (GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[2].numberOfLapsCompletedOnCurrentTrack)
                {
                    
                    GlobalGameData.cpuCarList[2].velocity = Vector3.Zero;
                    GlobalGameData.cpuCarList[2].allowedToMove = false;
                }
            }

            // wait for P1 (you) to finish before moving on.
            if ((GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack &&
                GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[0].numberOfLapsCompletedOnCurrentTrack &&
                GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[1].numberOfLapsCompletedOnCurrentTrack &&
                GlobalGameData.currentTrackModel.numberOfLaps == GlobalGameData.cpuCarList[2].numberOfLapsCompletedOnCurrentTrack) || 
                (GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack == GlobalGameData.currentTrackModel.numberOfLaps) 
                )
            {

                gameState = GameState.raceFinished;
                GlobalGameData.nextTrack++;
            }
            
        } //checkForRaceOver

        private void FillRemainingPositions()
        {
            // This method is a cheat really, rather than wait for each other car to
            // finish, if player finishes before all other cars have finished then
            // populate the list with other cars that have not finished yet randomly.

            if (GlobalGameData.racePosition.Count != 4 && canFillRemainingPositions == true)
            { 
                //Its going to contain player car for sure
                //so populate the rest with random cpu cars
                //taking car to avoid duplicates
                int positionsToBeFilled = 4 - GlobalGameData.racePosition.Count;
                for (int i = GlobalGameData.racePosition.Count; i < 4; i++)
                {
                    Random random = new Random();

                    Car temp = GlobalGameData.cpuCarList[random.Next(0, 3)];

                    while (GlobalGameData.racePosition.Contains(temp))
                    {
                        temp = GlobalGameData.cpuCarList[random.Next(0, 3)];
                    }

                    GlobalGameData.racePosition.Add(temp);
                    
                }
            }

            canFillRemainingPositions = false;
        
        } //fillRemainingPositions

       

        private void LoadNextTrack()
        {
            
            
            if (GlobalGameData.nextTrack == 2)
            {
                GlobalGameData.currentTestTexture = Content.Load<Texture2D>("Textures\\track2trackonly");
                GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack = 0;
                GlobalGameData.currentTrackModel.RemoveTrack();
                Track track = new Track(this, graphics, spriteBatch);
                GlobalGameData.currentTrackModel = track;
               
                
            }
            else if (GlobalGameData.nextTrack == 3)
            {
                GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack = 0;
                GlobalGameData.currentTrackModel.RemoveTrack();
                GlobalGameData.currentTestTexture = Content.Load<Texture2D>("Textures\\track3trackonly");
                GlobalGameData.currentTrackModel = new Track(this, graphics, spriteBatch);
               
            }
           

        } // Load Next Track

        protected void ResetAllCars()
        {
            // reset car positions here
            GlobalGameData.playerCar.position = new Vector3(-213, 0, 0);
            GlobalGameData.playerCar.forward = new Vector3(0, 0, 1);
            GlobalGameData.playerCar.right = Vector3.Right;
            GlobalGameData.playerCar.velocity = Vector3.Zero;
            GlobalGameData.playerCar.currentSpeed = 0.0f;

            GlobalGameData.playerCarSteerLeft.position = new Vector3(-213, 0, 0);
            GlobalGameData.playerCarSteerLeft.right = Vector3.Right;
            GlobalGameData.playerCarSteerLeft.forward = new Vector3(0, 0, 1);

            GlobalGameData.playerCarSteerRight.position = new Vector3(-213, 0, 0);
            GlobalGameData.playerCarSteerRight.right = Vector3.Right;
            GlobalGameData.playerCarSteerRight.forward = new Vector3(0, 0, 1);


            GlobalGameData.cpuCarList[0].position = new Vector3(-213, 0, 30);
            GlobalGameData.cpuCarList[0].forward = new Vector3(0, 0, 1);
            GlobalGameData.cpuCarList[0].velocity = Vector3.Zero;
            GlobalGameData.cpuCarList[0].currentSpeed = 0.0f;
            GlobalGameData.cpuCarList[0].right = Vector3.Right;

            GlobalGameData.cpuCarList[1].position = new Vector3(-246, 0, 45);
            GlobalGameData.cpuCarList[1].forward = new Vector3(0, 0, 1);
            GlobalGameData.cpuCarList[1].velocity = Vector3.Zero;
            GlobalGameData.cpuCarList[1].currentSpeed = 0.0f;
            GlobalGameData.cpuCarList[1].right = Vector3.Right;

            GlobalGameData.cpuCarList[2].position = new Vector3(-246, 0, 19);
            GlobalGameData.cpuCarList[2].forward = new Vector3(0, 0, 1);
            GlobalGameData.cpuCarList[2].velocity = Vector3.Zero;
            GlobalGameData.cpuCarList[2].currentSpeed = 0.0f;
            GlobalGameData.cpuCarList[2].right = Vector3.Right;

            foreach (CPUCar c in GlobalGameData.cpuCarList)
            {
                c.checkPoint1Yellow = false;
                c.checkPoint2Blue = false;
                c.checkPoint3Green = false;
                c.checkPointCounter = 0;
                c.numberOfLapsCompletedOnCurrentTrack = 0;
                c.whiteHit = 0;
                c.whiteCounter = 0;

            }

            GlobalGameData.playerCar.checkPoint1Yellow = false;
            GlobalGameData.playerCar.checkPoint2Blue = false;
            GlobalGameData.playerCar.checkPoint3Green = false;
            GlobalGameData.playerCar.checkPointCounter = 0;
            GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack = 0;
            GlobalGameData.playerCar.whiteHit = 0;
            GlobalGameData.playerCar.whiteCounter = 0;

            startCountDown = true;

        } // reset all cars


        private void FindCheckPoints()
        {
            // runs before the start of each race.
            // start at 0 0 (top left) and sweep to end (bottom right) of texture
            // and find check point locations and place a bounding sphere there, 
            // to improve performance, track texture is divided up into hops
            // of 128 which reduces the call to the color checking method.
            

            int i = 0;

            while ((myX + 128) <= 1024 && (myZ + 128) <= 1024) 
            {
                Console.WriteLine("X = " + myX + " Y = " + myZ);

                if (CheckPointFound(myX, myZ) == true)
                {
                    Console.WriteLine("Found @ " + myX + ", " + myZ);

                    mappedModelX = ((float)myX / 1.28f);

                    mappedModelX = (mappedModelX * -1f) + 402.312988281250000f;



                    mappedModelZ = ((float)myZ / 1.28f);

                    if (mappedModelZ >= 0 && mappedModelZ <= 402.312988281250000f)
                        mappedModelZ =  402.312988281250000f - mappedModelZ;
                    else
                        mappedModelZ = (mappedModelZ * -1f) + 402.312988281250000f;

                    Console.WriteLine("Mapped X " + mappedModelX + "\nMapped Z " + mappedModelZ);

                    

                    switch (i)
                    {
                        case 0:
                            GlobalGameData.checkPoint1.Center = new Vector3(mappedModelX, 0, mappedModelZ);
                            break;
                        case 1:
                            GlobalGameData.checkPoint2.Center = new Vector3(mappedModelX, 0, mappedModelZ);
                            break;
                        case 2:
                            GlobalGameData.checkPoint3.Center = new Vector3(mappedModelX, 0, mappedModelZ);
                            break;
                        
                    }

                    i++;


                    myX = myX + 128;
                }
                else
                {
                    myX = myX + 128;
                }

                if ((myX + 128) == 1024 && (myZ + 128) != 1024)
                {
                    myZ = myZ + 128;
                    myX = 0;
                }
            } // while

            i = 0;
            myX = 0;
            myZ = 0;
        } 



        //-------------------Modified Method from "The Road not taken"-------------------
        //http://www.xnadevelopment.com/tutorials/theroadnottaken/theroadnottaken.shtml
        internal bool CheckPointFound(int x, int z)
        {
            
            /* This method samples a patch of the texture determines if it contains a 
             * user defined color             
             */
            
              
            //Create a texture of the area i am testing by passing my new co-ords and applying scale
            //so that it corresponds correctly to the texture I am testing.
            Texture2D aCollisionCheck = CreateCollisonTexture(x, z);
                        
            //Use GetData to fill in an array with all of the colors of the Pixels in the area of the collison Texture
            int numberOfPixelsToStore = colorAreaToCheck_Width * colorAreaToCheck_Height;
                        
            Color[] myColors = new Color[numberOfPixelsToStore];
            aCollisionCheck.GetData<Color>(0, rect1, myColors, 0, numberOfPixelsToStore);

           // Interate thru array and test for off track colors, in my case alpha.
            bool aCollisonOccurred = false;
            foreach (Color pixel in myColors)
            {
                
                if (pixel == cp1yellow || pixel == cp2blue || pixel == cp3green)
                {
                    
                    aCollisonOccurred = true;
                    break;
                }
            }

            return aCollisonOccurred;
        }


        //-------------------Modified Method from "The Road not taken"-------------------
        //http://www.xnadevelopment.com/tutorials/theroadnottaken/theroadnottaken.shtml
        private Texture2D CreateCollisonTexture(float x, float z)
        {
            
            /*This method complements the previous method in that it is here that 
             * the texture sample is created.
             */


            //Grab a square of the Track image that is around the car
            graphics.GraphicsDevice.SetRenderTarget(trackRender);
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Red, 0, 0);

            spriteBatch.Begin();
            spriteBatch.Draw(
                GlobalGameData.currentTestTexture, //A texture.
                new Rectangle(0, 0, colorAreaToCheck_Width, colorAreaToCheck_Height), //A rectangle that specifies (in screen coordinates)
                //the destination for drawing the sprite. If this rectangle is not the same size as the source rectangle, the sprite will be scaled to fit.
                new Rectangle((int)(x - (colorAreaToCheck_Height / 2)), (int)(z - (colorAreaToCheck_Width / 2)), colorAreaToCheck_Width + (colorAreaToCheck_Width / 2), colorAreaToCheck_Height + (colorAreaToCheck_Height / 2)), //A rectangle that specifies (in texels)
                //the source texels from a texture. Use null to draw the entire texture.
                Color.White); //The color to tint a sprite. Use Color.White for full color with no tinting.
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            graphics.GraphicsDevice.SetRenderTarget(null);
                        
            return trackRender;

        }

        //-------------------------------END-----------------------------------
    

        

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            if (gameState == GameState.running)
            {
                base.Draw(gameTime);

                DrawGameRunning();
            }
            else if (gameState == GameState.mainMenu)
            {

                DrawGameMainMenu();
            }
            else if (gameState == GameState.raceFinished)
            {
                DrawGameRaceFinished();

                
            }
            else if (gameState == GameState.gameOver)
            {
                DrawGameOver();
            
            }

        } // Draw

        private void DrawGameOver()
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(podiumImage, new Vector2((Window.ClientBounds.Width / 2) - (startScreenImage.Width / 2), (Window.ClientBounds.Height / 2)
            - (startScreenImage.Height / 2)), Color.White);

            spriteBatch.DrawString(basicFont, "GAME OVER", new Vector2(100, 100), Color.White);
            spriteBatch.DrawString(basicFont, "Drivers Championship\n" + DisplayDriversTable(), new Vector2(100, 120), Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void DrawGameRaceFinished()
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(winImage, new Vector2((Window.ClientBounds.Width / 2) - (startScreenImage.Width / 2), (Window.ClientBounds.Height / 2)
            - (startScreenImage.Height / 2)), Color.White);

            spriteBatch.DrawString(raceResultsFont, "Race Finished, " + GlobalGameData.racePosition[0].carName + " won that one, Hit enter to continue to race " + GlobalGameData.nextTrack, new Vector2(100, 70), Color.Yellow);
            spriteBatch.DrawString(raceResultsFont, "Race Results\n" + RacePosition(), new Vector2(100, 90), Color.Yellow);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void DrawGameMainMenu()
        {
            GraphicsDevice.Clear(Color.Black);

            videoSplashTexture = myPlayer.GetTexture();

            Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X,
           GraphicsDevice.Viewport.Y,
           GraphicsDevice.Viewport.Width,
           GraphicsDevice.Viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(videoSplashTexture, screen, Color.White);

            //spriteBatch.Draw(startScreenImage, new Vector2((Window.ClientBounds.Width / 2) - (startScreenImage.Width / 2), (Window.ClientBounds.Height / 2)
            //- (startScreenImage.Height / 2)), Color.White);
            //spriteBatch.DrawString(raceResultsFont, "Press enter to start", new Vector2(100, 70), Color.Yellow);                
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void DrawGameRunning()
        {
            // Draw text with a sprite tutorial http://msdn.microsoft.com/en-us/library/bb447673.aspx
            //then 2d followed by resetting some values
            spriteBatch.Begin();
            if (startCountDown == true && countDown > 0)
                spriteBatch.DrawString(countDownFont, "" + countDown, new Vector2((Window.ClientBounds.Width / 2) - 50, (Window.ClientBounds.Height / 2) - 100), Color.Red);
            if (startCountDown == true && countDown == 0)
                spriteBatch.DrawString(countDownFont, "GO!", new Vector2((Window.ClientBounds.Width / 2) - 50, (Window.ClientBounds.Height / 2) - 100), Color.Green);
            spriteBatch.DrawString(HUDfont, "Laps Completed " + GlobalGameData.playerCar.numberOfLapsCompletedOnCurrentTrack, new Vector2(10, 10), Color.Black);
            spriteBatch.DrawString(HUDfont, "Bonus Item: " + GlobalGameData.playerCar.currentBonusItem, new Vector2(10, 30), Color.Black);
            spriteBatch.DrawString(HUDfont, "Current Speed: " + GlobalGameData.playerCar.currentSpeed.ToString("#.#"), new Vector2(10, 50), Color.Black);
            spriteBatch.DrawString(HUDfont, "km/h", new Vector2(170, 50), Color.Black);
            if (GlobalGameData.racePosition.Count == 0)
                spriteBatch.DrawString(basicFont, "Track positions for lap\n" + RacePosition(), new Vector2(10, 70), Color.Black);
            else
                spriteBatch.DrawString(basicFont, "Track positions for lap " + GlobalGameData.racePosition[0].numberOfLapsCompletedOnCurrentTrack + "\n" + RacePosition(), new Vector2(10, 70), Color.Black);
            spriteBatch.End();

            //mixing 2d with 3d causes model to be drawn incorrectly, to fix this implement the following 
            //solution from http://blogs.msdn.com/b/shawnhar/archive/2010/06/18/spritebatch-and-renderstates-in-xna-game-studio-4-0.aspx
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private string DisplayDriversTable()
        {
            

            if (sortAndExtractDriversChampionshipStandings == true)
            {
                int index = 1;

                //http://blog.codewrench.net/2009/04/14/sorting-a-generic-list-on-arbitrary-property/

                //var sortedCarList = from p in GlobalGameData.racePosition
                //                    orderby p.driverPoints descending
                //                    select p;


                //foreach (var car in sortedCarList)
                //{
                //    driverStandingAsAsString = driverStandingAsAsString + index + ". " + car.carName + ": " + car.driverPoints + "\n";
                //    index++;
                //}

                // Replaced above code with a sort and custom comparator class
                GlobalGameData.racePosition.Sort(new CompareDriversPoints());

                foreach (Car c in GlobalGameData.racePosition)
                {
                    driverStandingAsAsString = driverStandingAsAsString + index + ". " + c.carName + ": " + c.driverPoints + "\n";
                    index++;
                }

                sortAndExtractDriversChampionshipStandings = false;
            
            }
           
            return driverStandingAsAsString;
        } 

        // Once race is over assign points to drivers
        private void AssignPoints()
        {

            if (GlobalGameData.racePosition.Count > 0 && canAssignPoints == true)
            {
                foreach (Car c in GlobalGameData.racePosition)
                {


                    switch (GlobalGameData.racePosition.IndexOf(c))
                    {
                        case 0:
                            c.driverPoints = c.driverPoints + 10;
                            break;
                        case 1:
                            c.driverPoints = c.driverPoints + 8;
                            break;
                        case 2:
                            c.driverPoints = c.driverPoints + 7;
                            break;
                        case 3:
                            c.driverPoints = c.driverPoints + 6;
                            break;
                    }



                }

            }

            canAssignPoints = false;
            
        }

        private String RacePosition()
        {
            /*
             * Used for building string that will display the cars in order of their position
             */
                      
            if (GlobalGameData.racePosition.Count > 0)
            {
                String racePositionAsString = "";

                foreach (Car c in GlobalGameData.racePosition)
                {
                   
                    racePositionAsString = racePositionAsString + (GlobalGameData.racePosition.IndexOf(c) + 1) + ". " + c.carName + "\n";
                }


                return racePositionAsString;
            }
            else
            return "";
        } 

        
    }
     
}
