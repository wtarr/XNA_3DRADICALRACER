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
    public class Obstacle : GameObject
    {

        internal int xPos = 0, zPos = 0;
        Random random = new Random();

        public Obstacle(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)           
        {

        }
        
        public override void Initialize()
        {

            position = new Vector3(0, 0, 0);
            boundingRadius.Center = position;
            boundingRadius.Radius = 1;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("Models\\slick");

            
            base.LoadContent();
        }

        public void generatePosition()
        {

            while (OffTrackCollisonOccurred((float)xPos, (float)zPos, new Color(255, 0, 0, 255)) == true || xPos == 0 || zPos == 0)
            {
                xPos = random.Next(-400, 400);
                zPos = random.Next(-400, 400);
            }

            position = new Vector3((float)xPos, 0, (float)zPos);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            boundingRadius.Center = position;
            
            checkForCollisonWithPlayerCar();
            checkForCollisonWithCPUCar();
            
            base.Update(gameTime);
        }

        private void checkForCollisonWithCPUCar()
        {
            foreach (CPUCar c in GlobalGameData.cpuCarList)
            {
                if (this.boundingRadius.Intersects(c.boundingRadius))
                    c.spin = true;
            }
        }

        private void checkForCollisonWithPlayerCar()
        {
           if (this.boundingRadius.Intersects(GlobalGameData.playerCar.boundingRadius))
               GlobalGameData.playerCar.spin = true;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

                
    }
}