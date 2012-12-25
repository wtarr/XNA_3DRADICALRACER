
/*
 * This will have code that is common to all classes 
 * I will filter up common code as I go and deem necessary do to 
 * repetition.
 * 
 * */

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
    public class GameObject:DrawableGameComponent
    {
        
        internal Model model;
        internal Vector3 position = Vector3.Zero;
        internal Vector3 up = Vector3.Up;
        internal Vector3 forward = Vector3.Forward;
        internal Vector3 right = Vector3.Right;
        internal BoundingSphere boundingRadius;
        internal Vector3 velocity = Vector3.Zero;
        internal Vector3 accelleration = Vector3.Zero;

        // Color key collison detection specific attributes
        internal Game game;
        internal GraphicsDeviceManager graphics;
        internal SpriteBatch spriteBatch;
        float mappedXpos;
        float mappedZpos;
        int colorAreaToCheck_Width = 5;
        int colorAreaToCheck_Height = 5;
        internal Color offTrack = new Color(255, 0, 0, 255); // the color to check (red)
        RenderTarget2D trackRender;
        Rectangle rect1;
        

        public GameObject(Game game, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            game.Components.Add(this);
            rect1 = new Rectangle(0, 0, colorAreaToCheck_Width, colorAreaToCheck_Height);
            
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override void Initialize()
        {
            
            base.Initialize();
        }

        protected override void LoadContent()
        {

            
            //boundingRadius.Radius = model.Meshes[0].BoundingSphere.Radius;
            //base.LoadContent();
            trackRender = new RenderTarget2D(graphics.GraphicsDevice, colorAreaToCheck_Width, colorAreaToCheck_Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
           
            
            //Implement physics that implement frame rate independence.
            
            // v "= u + "      a         *      t    
            velocity += accelleration * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // s     =     u      *      t
            position += velocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            boundingRadius.Center = position;
            
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //From semester 1 project
            
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {

                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    
                    effect.LightingEnabled = true; // turn on the lighting subsystem. 
                    
                    ////http://rbwhitaker.wikidot.com/basic-effect-lighting
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0, 0); // a red light 
                    effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis 
                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green
                    effect.AmbientLightColor = new Vector3(0.9f, 0.9f, 0.9f); // Add some overall ambient light.
                    
                    
                   effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateWorld(position, forward, up);

                    effect.View =
                        Matrix.CreateLookAt(Camera.position,
                                            Camera.target,
                                            Camera.up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        1.0f, 1.0f, 5000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        //-------------------Modified Method from The Road not taken-------------------
        //http://www.xnadevelopment.com/tutorials/theroadnottaken/theroadnottaken.shtml
        internal bool OffTrackCollisonOccurred(float x, float z, Color colorToTestAgainst)
        {
            
            mappedXpos = (x * -1f) + 402.312988281250000f;

            mappedZpos = 402.312988281250000f + z;
            
            mappedZpos = (2 * 402.312988281250000f) - mappedZpos;
              
            //Create a texture of the area i am testing by passing my new co-ords and applying scale
            //so that it corresponds correctly to the texture I am testing.
            Texture2D aCollisionCheck = CreateCollisonTexture(mappedXpos * 1.28f, mappedZpos * 1.28f);
                        
            //Use GetData to fill in an array with all of the colors of the Pixels in the area of the collison Texture
            int numberOfPixelsToStore = colorAreaToCheck_Width * colorAreaToCheck_Height;
                        
            Color[] myColors = new Color[numberOfPixelsToStore];
            aCollisionCheck.GetData<Color>(0, rect1, myColors, 0, numberOfPixelsToStore);

           // Interate thru array and test for off track colors, in my case alpha.
            bool aCollisonOccurred = false;
            foreach (Color pixel in myColors)
            {
                
                if (pixel == colorToTestAgainst)
                {
                    
                    aCollisonOccurred = true;
                    break;
                }
            }

            return aCollisonOccurred;
        }

        //-------------------Modified Method from The Road not taken-------------------
        //http://www.xnadevelopment.com/tutorials/theroadnottaken/theroadnottaken.shtml
        //Create the Collison Texture that contains the rotated Track image for determining
        //the pixels underneath the Car sprite
        private Texture2D CreateCollisonTexture(float x, float z)
        {
            
            //Grab a square of the Track image that is around the car
            graphics.GraphicsDevice.SetRenderTarget(trackRender);
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Red, 0, 0);

            spriteBatch.Begin();
            spriteBatch.Draw(
                GlobalGameData.currentTestTexture, //A texture.
                rect1, //A rectangle that specifies (in screen coordinates)
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
    }
}
