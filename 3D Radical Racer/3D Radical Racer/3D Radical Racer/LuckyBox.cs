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
    public class LuckyBox : GameObject
    {
        internal int xPos = 0, zPos = 0;
        Random random = new Random();
        String[] bonusItemList = new String[3];
        private int luckyBoxTimer;

        public LuckyBox(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {
            
           
        }
        
        public override void Initialize()
        {
            position = new Vector3(0, 0, 0);
            boundingRadius.Center = position;
            boundingRadius.Radius = 2;
            bonusItemList[0] = "Go Faster";
            bonusItemList[1] = "Poison";
            bonusItemList[2] = "Missile";
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("Models\\luckBox");

            
            base.LoadContent();
        }

        public void generatePosition()
        {
           
            while (OffTrackCollisonOccurred((float)xPos, (float)zPos, offTrack) == true || xPos == 0 || zPos == 0)
            {
                xPos = random.Next(-400, 400);
                zPos = random.Next(-400, 400);
            }

            position = new Vector3((float)xPos, 0, (float)zPos);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            boundingRadius.Center = position;

            checkForCollisionWithCPUCar();
            checkForCollisonWithPlayerCar();

            if (luckyBoxTimer > 1000)
            {
                luckyBoxTimer = 0;
                GlobalGameData.luckybox.xPos = 0;
                GlobalGameData.luckybox.zPos = 0;
                GlobalGameData.luckybox.generatePosition();
            }

            luckyBoxTimer++;

            base.Update(gameTime);
        }

        private void checkForCollisonWithPlayerCar()
        {
            if (this.boundingRadius.Intersects(GlobalGameData.playerCar.boundingRadius) && GlobalGameData.playerCar.currentBonusItem.Equals("None"))
            {
                //Console.WriteLine("Collison");
                // Select a random item
                luckyBoxTimer = 0;
                GlobalGameData.luckybox.xPos = 0;
                GlobalGameData.luckybox.zPos = 0;
                GlobalGameData.luckybox.generatePosition();
                int ranNum = random.Next(0, 3);
                GlobalGameData.playerCar.currentBonusItem = bonusItemList[ranNum];
                //Console.WriteLine(GameData.playerCar.currentBonusItem);
            }
            
        }

        private void checkForCollisionWithCPUCar()
        {
            foreach (Car c in GlobalGameData.cpuCarList)
            {
                if (this.boundingRadius.Intersects(c.boundingRadius) && c.currentBonusItem.Equals("None"))
                {
                    luckyBoxTimer = 0;
                    GlobalGameData.luckybox.xPos = 0;
                    GlobalGameData.luckybox.zPos = 0;
                    GlobalGameData.luckybox.generatePosition();
                    int ranNum = random.Next(0, 3);
                    c.currentBonusItem = bonusItemList[ranNum];
                    //Console.WriteLine(GameData.playerCar.currentBonusItem);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

                
    }
}
