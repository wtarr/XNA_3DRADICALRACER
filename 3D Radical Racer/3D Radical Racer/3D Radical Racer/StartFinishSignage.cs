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
    public class StartFinishSignage : GameObject
    {
        

        public StartFinishSignage(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game, graphics, spriteBatch)
        {


        }

        public override void Initialize()
        {
            position = new Vector3(0, 0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("Models\\startFinish");
            base.LoadContent();
        }

        
    }
}
