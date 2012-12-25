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
    public static class Camera
    {
        public static Vector3 position;
        public static Vector3 target;
        public static Vector3 up;
        public static Vector2 relativeToCar;// = new Vector2(-70, 40);
        public static float horizonDistance = 100f;
    }
}
