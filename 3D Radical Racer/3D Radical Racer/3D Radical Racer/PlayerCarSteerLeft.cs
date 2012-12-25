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
    public class PlayerCarSteerLeft : Car
    {
        public PlayerCarSteerLeft(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game,graphics,spriteBatch)
        {

           
        }
        
        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("Models\\F1MODELLeft");
            
            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            this.position = GlobalGameData.playerCar.position;
            this.forward = GlobalGameData.playerCar.forward;
            this.right = GlobalGameData.playerCar.right;
            this.up = GlobalGameData.playerCar.up;
            //Console.WriteLine("steerleft " + position);
            
            base.Update(gameTime);
        }




    }

    
   
}
