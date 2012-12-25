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
    public class Track:GameObject
    {

        //private String trackName;
        //private int lapRecordinSeconds;
        internal int numberOfLaps = 3;
        
        //private Song trackMusic;
        
        internal BoundingBox startFinish;
       
        //private int timer;

        



        public Track(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {

           
        }

        public override void Initialize()
        {
            position = Vector3.Zero;
            startFinish = new BoundingBox(new Vector3(-260, -100, 72.1f), new Vector3(-190, 100, 72.2f));
            base.Initialize();
        }

        protected override void LoadContent()
        {

            String pathBuilder = "Models\\track" + GlobalGameData.nextTrack;
            
            model = Game.Content.Load<Model>(pathBuilder);

            

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            
            //performLapCountForCars();

            //Console.WriteLine("Check point counter = " + GameData.playerCar.checkPointCounter);
                    
            base.Update(gameTime);
        }
                

        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }

        public void RemoveTrack()
        {
            Game.Components.Remove(this);
        }
    }
}
