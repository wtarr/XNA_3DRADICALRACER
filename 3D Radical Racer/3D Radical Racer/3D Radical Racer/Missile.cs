using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3D_Radical_Racer
{
    public class Missile : GameObject
    {

        private float thrustValue = 3000;
        private String caller;


        public Missile(String caller, Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {
            this.caller = caller;

        }

        public override void Initialize()
        {

            boundingRadius.Radius = 1;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            model = Game.Content.Load<Model>("Models\\missile");
            base.LoadContent();

        }

        /// <param name="gameTime">Time elapsed since the last call to Update</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //move missile
            accelleration = thrustValue * forward;

            checkForCollisonWithCPUCar();
            checkForCollisonWithPlayerCar();

            base.Update(gameTime);
        }

        private void checkForCollisonWithCPUCar()
        {
            if (caller.Equals("player"))
            {
                foreach (CPUCar c in GlobalGameData.cpuCarList)
                {
                    if (this.boundingRadius.Intersects(c.boundingRadius))
                    {
                        c.spin = true;
                        Game.Components.Remove(this);
                    }
                }
            }

        }

        private void checkForCollisonWithPlayerCar()
        {
            if (caller.Equals("cpu") && this.boundingRadius.Intersects(GlobalGameData.playerCar.boundingRadius))
            {
                GlobalGameData.playerCar.spin = true;
                Game.Components.Remove(this);
            }
        }

    }
}
