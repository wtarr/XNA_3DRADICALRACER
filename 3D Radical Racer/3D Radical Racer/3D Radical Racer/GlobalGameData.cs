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
    public static class GlobalGameData
    {
        //public static List<Track> listOfTracks;
        public static int nextTrack = 1; // 1, 2, 3, 4, 5, ...
        public static Track currentTrackModel;
        public static Texture2D currentTestTexture;
        public static int currentFastestLapinSeconds;
        public static List<CPUCar> cpuCarList;

        public static BoundingSphere checkPoint1;
        public static BoundingSphere checkPoint2;
        public static BoundingSphere checkPoint3;
        
        public static PlayerCar playerCar;
        public static PlayerCarSteerLeft playerCarSteerLeft;
        public static PlayerCarSteerRight playerCarSteerRight;

        public static CPUCar cpuCar;

        public static LuckyBox luckybox;

        public static Obstacle slick;
        
       
        public static Vector3 feelerFront_Pos, feelerLeft_Pos, feelerRight_Pos;
        public static Vector3 feelerFront_Forward, feelerLeft_Forward, feelerRight_Forward;
        public static Vector3 feelerFront_Up, feelerLeft_Up, feelerRight_Up;
        public static Vector3 feelerFront_Right, feelerLeft_Right, feelerRight_Right;

        //public static List<Car> carList; // use for sorting track position ## MAYBE REDUNDANT NOW ???

        public static List<Car> racePosition = new List<Car>(); // eliminates possiblity of duplicates
        
    }
}
