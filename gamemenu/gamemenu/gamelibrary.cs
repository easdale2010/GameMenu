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


namespace WindowsGameLibrary1
{
    public class graphics2d
    {
        public Texture2D image;
        public Rectangle rect;

        public graphics2d() { }
        public graphics2d(ContentManager content, string spritename, int dwidth, int dheight)
        {
            image = content.Load<Texture2D>(spritename);
            float ratio = ((float)dwidth / image.Width);
            rect.Width = dwidth;
            rect.Height = (int)(image.Height * ratio);
            rect.X = 0;
            rect.Y = (dheight - rect.Height) / 2;
        }
        public void drawme(ref SpriteBatch spriteBatch2)
        {
            spriteBatch2.Draw(image, rect, Color.White);
        }
    }
    public class sprites2d
    {
        public Texture2D image;
        public Vector3 position;
        public Vector3 oldposition;
        public Rectangle rect;
        public Vector2 origin;
        public float rotation = 0;
        public Vector3 velocity;
        public BoundingSphere bsphere;
        public BoundingBox bbox;
        public Boolean visible = true;
        public Color colour = Color.White;
        public float size;
        private float spinspeed=0.02f;

        public sprites2d() { }
        public sprites2d(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis, Random randomiser)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.X = x;
            rect.Y = y;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            colour = mcolour;
            visible = mvis;
            size = msize;
            oldposition = position;
            
            spinspeed = (float)(randomiser.Next(100)-50)/100f;

        }
        public void moveme(GamePadState gpad, KeyboardState keys, int dwidth, int dheight, float gtime)
        {
            velocity.X = gpad.ThumbSticks.Left.X;
            velocity.Y = -gpad.ThumbSticks.Left.Y;

            float speed = 0.5f;
            position += velocity * gtime * speed;

            if (position.X < rect.Width / 2) position.X = rect.Width / 2;
            if (position.X > dwidth - rect.Width / 2) position.X = dwidth - rect.Width / 2;
            if (position.Y < rect.Height / 2) position.Y = rect.Height / 2;
            if (position.Y > dheight - rect.Height / 2) position.Y = dheight - rect.Height / 2;

            updateobject();
        }

        public void automove(int dwidth, int dheight, float gtime)
        {
            rotation += spinspeed;
            position += velocity * gtime;

            if ((position.X + rect.Width / 2) > dwidth)
            {
                velocity.X = -velocity.X;
                position.X = dwidth - rect.Width / 2;
            }
            if ((position.X - rect.Width / 2) <= 0)
            {
                velocity.X = -velocity.X;
                position.X = rect.Width / 2;
            }
            if ((position.Y + rect.Height / 2) >= dheight)
            {
                velocity.Y = -velocity.Y;
                position.Y = dheight - rect.Height / 2;
            }
            if ((position.Y - rect.Height / 2) <= 0)
            {
                velocity.Y = -velocity.Y;
                position.Y = rect.Height / 2;
            }
            updateobject();
        }

        public void updateobject()
        {
            rect.Y = (int)position.Y;
            rect.X = (int)position.X;
            bsphere = new BoundingSphere(position, rect.Width / 2);

            bbox = new BoundingBox(new Vector3(position.X - rect.Width / 2, position.Y - rect.Height / 2, 0),
            new Vector3(position.X + rect.Width / 2, position.Y + rect.Height / 2, 0));

        }
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
                sbatch.Draw(image, rect, null, colour, rotation, origin, SpriteEffects.None, 0);
        }
        public void drawme(ref  SpriteBatch sbatch, Vector3 newpos)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.Y = (int)newpos.Y;
                newrect.X = (int)newpos.X;

                sbatch.Draw(image, newrect, null, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }

    public class ships : sprites2d
    {
        public Vector3 direction;
        float thrust;
        float rotationspeeed = 0.005f;
        float shipspeed = 0.01f;
        float friction = 0.99f;
        public int lives = 5;
        public int score = 0;
        public float spawntime = 0;

        public ships(){}
        public ships(ContentManager content, string spritename,int x,int y, float msize, Color mcolour,Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.Y = y;
            rect.X = x;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            colour = mcolour;
            visible = mvis;
            size = msize;
            oldposition = position;
        }
        public void moveme(GamePadState gpad, int dwidth, int dheight, float gtime)
        {
            spawntime += gtime;

            rotation += gpad.ThumbSticks.Left.X * rotationspeeed * gtime;
            thrust = (shipspeed * gtime * (gpad.Triggers.Right - gpad.Triggers.Left));

            direction.X = (float)(Math.Cos(rotation));
            direction.Y = (float)(Math.Sin(rotation));

            velocity += direction * thrust;
            velocity *= friction;
            position += velocity;

            if (position.X < rect.Width / 2) position.X = rect.Width / 2;
            if (position.Y < rect.Height / 2) position.Y = rect.Height / 2;
            if (position.X > dwidth - rect.Width / 2) position.X = dwidth - rect.Width / 2;
            if (position.Y > dheight - rect.Height / 2) position.Y = dheight - rect.Height / 2;

            updateobject();
        }
    }

    public class ammo : sprites2d
    {
        public float bulletlength = 1000;
        public float bulletspawned = 1001;

        public ammo() { }
        public ammo(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis)
        {
            image = content.Load<Texture2D>(spritename);
            position = new Vector3((float)x, (float)y, 0);
            rect.Y = y;
            rect.X = x;
            origin.X = image.Width / 2;
            origin.Y = image.Height / 2;
            rect.Width = (int)(image.Width * msize);
            rect.Height = (int)(image.Height * msize);
            size = msize;
            colour = mcolour;
            oldposition = position;
            visible = mvis;
        }
        public void firebullet(Vector3 pos, Vector3 dir)
        {
            if (!visible && bulletspawned > bulletlength)
            {
                float bulletspeed = 1.5f;
                visible = true;
                position = pos;
                velocity = dir * bulletspeed;
                updateobject();
              
                bulletspawned = 0;
              
            }
        }
        public void movebullet(float gtime)
        {
            bulletspawned += gtime;
            if (visible)
                position += velocity * gtime;

            if (bulletspawned > bulletlength) visible = false;

            updateobject();
        }
        public void firebullet2(Vector3 pos, Vector3 dir)
        {
            if (!visible)
            {
                float bulletspeed = 1.5f;
                visible = true;
                position = pos;
                velocity = dir * bulletspeed;
                updateobject();

                bulletspawned = 0;

            }
        }
        public void movebullet2(float gtime,int dwidth,int dheight)
        {
    
            if (visible)
                position += velocity * gtime;

            if (position.X > dwidth || position.X < rect.Width || position.Y < rect.Height || position.Y > dheight) visible = false;

            updateobject();
        }
    }

    public static class sfunctions
    {
        // Function to handle collision response
        public static void cresponse(Vector3 position1, Vector3 position2, ref Vector3 velocity1, ref Vector3 velocity2, float weight1, float weight2)
        {
            // Calculate Collision Response Directions
            Vector3 x = position1 - position2;
            x.Normalize();
            Vector3 v1x = x * Vector3.Dot(x, velocity1);
            Vector3 v1y = velocity1 - v1x;
            x = -x;
            Vector3 v2x = x * Vector3.Dot(x, velocity2);
            Vector3 v2y = velocity2 - v2x;

            velocity1 = v1x * (weight1 - weight2) / (weight1 + weight2) + v2x * (2 * weight2) / (weight1 + weight2) + v1y;
            velocity2 = v1x * (2 * weight1) / (weight1 + weight2) + v2x * (weight2 - weight1) / (weight1 + weight2) + v2y;
        }


        public static Vector3 midpoint(Vector3 position1, Vector3 position2)
        {
            Vector3 middle;
            middle.X = (position1.X + position2.X) / 2;
            middle.Y = (position1.Y + position2.Y) / 2;
            middle.Z = (position1.Z + position2.Z) / 2;

            return middle;
        }
    }
    public class animation
    {
        Texture2D image;            // Texture which holds animation sheet
        public Vector3 position;    // Position of animation
        public Rectangle rect;      // Rectangle to hold size and position
        Rectangle frame_rect;       // Rectangle to hold position of frame to draw
        Vector2 origin;             // Centre point
        public float rotation = 0;  // Rotation amount
        public Color colour = Color.White; // Colour
        public float size;          // Size Ratio
        public Boolean visible;     // Should object be drawn true or false
        public int framespersecond; // Frame Rate
        int frames;                 // Number of frames of animation
        int rows;                   // Number of rows in the sprite sheet
        int columns;                // Number of columns in the sprite sheet
        int frameposition;          // Current position in the animation
        int framewidth;             // Width in pixels of each frame of animation
        int frameheight;            // Height in pixels of each frame of animation
        float timegone;             // Time since animation began
        public Boolean loop = false;// Should animation loop
        int noofloops = 0;          // Number of loops to do
        int loopsdone = 0;          // Number of loops completed
        public Boolean paused = false;  // Freeze frame animation

        public animation() { }

        // Constructor which initialises the animation
        public animation(ContentManager content, string spritename, int x, int y, float msize, Color mcolour, Boolean mvis, int fps, int nrows, int ncol, Boolean loopit)
        {
            image = content.Load<Texture2D>(spritename);    // Load image into texture
            position = new Vector3((float)x, (float)y, 0);  // Set position
            rect.X = x;                                     // Set position of draw rectangle x
            rect.Y = y;                                     // Set position of draw rectangle y
            size = msize;                                   // Store size ratio
            colour = mcolour;                               // Set colour
            visible = mvis;                                 // Image visible TRUE of FALSE? 
            framespersecond = fps;                          // Store frames per second
            rows = nrows;                                   // Number of rows in the sprite sheet
            columns = ncol;                                 // Number of columns in the sprite sheet
            frames = rows * columns;                          // Store no of frames
            framewidth = (int)(image.Width / columns);      // Calculate the width of each frame of animation
            frameheight = (int)(image.Height / rows);       // Calculate the heigh of each frame of animation
            rect.Width = (int)(framewidth * size);          // Set the new width based on the size ratio    
            rect.Height = (int)(frameheight * size);	    // Set the new height based on the size ratio
            frame_rect.Width = framewidth;                  // Set the width of each frame
            frame_rect.Height = frameheight;                // Set the height of each frame
            origin.X = framewidth / 2;                      // Set X origin to half of frame width
            origin.Y = frameheight / 2;              	    // Set Y origin to half of frame heigh
            loop = loopit;                                  // Should it be looped or not
        }

        public void start(Vector3 pos, float rot, int repeatnumber)
        {
            // Set position of object into the rectangle from the position Vector
            position = pos;
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;

            // Start new animation
            noofloops = repeatnumber;
            rotation = rot;
            visible = true;
            frameposition = 0;
            timegone = 0;
            loopsdone = 0;
            paused = false;
        }

        public void update(float gtime)
        {
            if (framespersecond < 1) framespersecond = 1; // Error checking to avoid divide by zero

            if (visible && !paused)
            {
                frameposition = (int)(timegone / (1000 / framespersecond));   // Work out what frame the animation is on
                timegone += gtime;                                          // Time gone during the animation
                // Check if the animation is at the end
                if (frameposition >= frames)
                {
                    // Repeat animation if necessary
                    if (loop || loopsdone < noofloops)
                    {
                        loopsdone++;
                        frameposition = 0;
                        timegone = 0;
                    }
                    else
                    {
                        visible = false;   // End animation
                    }
                }
            }
        }

        // Use this method to draw the image
        public void drawme(ref SpriteBatch sbatch)
        {
            if (visible)
            {   // Work out the co-ordinates of the current frame and then draw that frame
                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                sbatch.Draw(image, rect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }

        // Use this method to draw the image at a specified position
        public void drawme(ref SpriteBatch sbatch, Vector3 newpos)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.X = (int)newpos.X;
                newrect.Y = (int)newpos.Y;

                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }

        // Use this method to draw the image at a specified position and allow image to be flipped horizontally or vertically
        public void drawme(ref SpriteBatch sbatch, Vector3 newpos, Boolean flipx, Boolean flipy)
        {
            if (visible)
            {
                Rectangle newrect = rect;
                newrect.X = (int)newpos.X;
                newrect.Y = (int)newpos.Y;

                frame_rect.Y = ((int)(frameposition / columns)) * frameheight;
                frame_rect.X = (frameposition - ((int)(frameposition / columns)) * columns) * framewidth;
                if (flipx)
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.FlipHorizontally, 0);
                else if (flipy)
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.FlipVertically, 0);
                else
                    sbatch.Draw(image, newrect, frame_rect, colour, rotation, origin, SpriteEffects.None, 0);
            }
        }
    }

}
