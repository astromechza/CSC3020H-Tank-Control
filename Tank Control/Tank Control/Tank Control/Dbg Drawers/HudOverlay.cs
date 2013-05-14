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
using Tank_Control.Game_Objects;


namespace Tank_Control
{
    public class HudOverlay : GameObject
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private float elapsedTime, totalFrames, fps;
        private bool showFps;
        private string fontName = "fpsfont";
        private float lineheight;
        public bool ShowFPS
        {
            get { return showFps; }
            set { showFps = value; }
        }

        public HudOverlay(Game game) : base(game)
        {
            showFps = true;
        }

        public override void Update(double ellapsedMillis)
        {
            elapsedTime += (float)ellapsedMillis/1000;
            totalFrames++;

            if (elapsedTime >= 1.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }
        }

        public override void LoadContent(ContentManager contentMan)
        {
            spriteBatch = new SpriteBatch(this.game.GraphicsDevice);
            spriteFont = this.game.Content.Load<SpriteFont>(fontName);
            lineheight = spriteFont.MeasureString("test").Y;
        }

        
        public override void Draw()
        {

            int line = 0;
            if (showFps)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, "Tank Control by Ben Meier (MRXBEN001) ", new Vector2(10, 10 + (line++) * lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

                spriteBatch.DrawString(spriteFont, "FPS: " + fps.ToString(), new Vector2(10, 10 + (line++)*lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

                spriteBatch.DrawString(spriteFont, "CAMERA: " + game.camera.mode, new Vector2(10, 10 + (line++) * lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

                if (game.camera.mode != Cameras.CameraMode.FirstPerson)
                {
                    spriteBatch.DrawString(spriteFont, " Distance: " + game.camera.distance, new Vector2(10, 10 + (line++) * lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(spriteFont, " Height: " + game.camera.height, new Vector2(10, 10 + (line++) * lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(spriteFont, " Looseness: " + game.camera.looseness, new Vector2(10, 10 + (line++) * lineheight), Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                    
                }
                

                spriteBatch.End();

                this.game.GraphicsDevice.BlendState = BlendState.Opaque;
                this.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                this.game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            }
            
        }
    }
}
