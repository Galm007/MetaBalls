using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MetaBalls
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private KeyboardState previousKeyboard;

        private bool showCircles = false;
        private bool showCorners = false;
        private int show = 3;

        private Texture2D pixelTexture;
        private const int pixelSize = 13;

        private List<Circle> circles = new List<Circle>();
        private const int totalCircles = 15;
        private const float blobbyness = 1f;

        private class Circle
        {
            public float radius;
            public Vector2 position;
            public Vector2 velocity = Vector2.Zero;

            public Circle(float radius, Vector2 position, GraphicsDevice graphics)
            {
                this.radius = radius;
                this.position = position;

                float velX = new Random().Next(-70, 70);
                float velY = new Random().Next(-70, 70);
                this.velocity = new Vector2(velX, velY);
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Resize Screen
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData<Color>(new Color[1] { Color.Green });

            for (int i = 0; i < totalCircles; i++)
            {
                float r = RandomFloat(40f, 70f);
                Vector2 p = new Vector2(new Random().Next((int)r, GraphicsDevice.Viewport.Width - (int)r), new Random().Next((int)r, GraphicsDevice.Viewport.Height - (int)r));
                circles.Add(new Circle(r, p, GraphicsDevice));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.A) && previousKeyboard.IsKeyUp(Keys.A))
                showCircles = !showCircles;
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && previousKeyboard.IsKeyUp(Keys.S))
                showCorners = !showCorners;

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                show = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
                show = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.E))
                show = 2;
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
                show = 3;

            for (int i = 0; i < totalCircles; i++)
            {
                circles[i].position += (circles[i].velocity * deltatime);

                if (circles[i].position.X < circles[i].radius || circles[i].position.X + circles[i].radius > GraphicsDevice.Viewport.Width)
                    circles[i].velocity.X *= -1f;
                    
                if (circles[i].position.Y < circles[i].radius || circles[i].position.Y + circles[i].radius > GraphicsDevice.Viewport.Height)
                    circles[i].velocity.Y *= -1f;
            }

            previousKeyboard = Keyboard.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            switch (show)
            {
                case 0:
                    DrawWithoutBlobby();
                    break;
                case 1:
                    DrawWithBlobby();
                    break;
                case 2:
                    DrawBlobbyMarching(Color.Green);
                    break;
                case 3:
                    DrawBlobbyMarchingInterpolated(Color.Green);
                    break;
                default:
                    throw new Exception("show can only be 0 to 3.");
            }

            if (showCircles)
            {
                DrawDebugCircles(MathF.PI / 10f, Color.Red);
            }
            if (showCorners)
            {
                DrawDebugOccupiedCorners();
            }
                
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebugCircles(float step, Color color)
        {
            for (int i = 0; i < totalCircles; i++)
            {
                Point previousPoint = new Point
                (
                    (int)(circles[i].position.X + circles[i].radius * MathF.Cos(0f)),
                    (int)(circles[i].position.Y + circles[i].radius * MathF.Sin(0f))
                );

                for (float angle = step; angle < MathF.PI * 2f + step; angle += step)
                {
                    float x = circles[i].position.X + circles[i].radius * MathF.Cos(angle);
                    float y = circles[i].position.Y + circles[i].radius * MathF.Sin(angle);
                    Point point = new Vector2(x, y).ToPoint();
                    
                    DrawLine(previousPoint, point, color);
                    previousPoint = point;
                }
                //spriteBatch.Draw(debugCircleTexture, new Rectangle((circles[i].position - new Vector2(circles[i].radius)).ToPoint(), new Point((int)circles[i].radius * 2)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        private void DrawDebugOccupiedCorners()
        {
            Texture2D t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[1] { Color.White });
            
            for (int y = 0; y < GraphicsDevice.Viewport.Height; y += pixelSize)
            {
                for (int x = 0; x < GraphicsDevice.Viewport.Width; x += pixelSize)
                {
                    if (GetPixelValue(x, y) >= blobbyness)
                    {
                        spriteBatch.Draw(t, new Rectangle(x, y, 2, 2), null, Color.Green);
                    }
                    else
                    {
                        spriteBatch.Draw(t, new Rectangle(x, y, 1, 1), null, Color.Gray);
                    }
                }
            }
        }

        private void DrawWithoutBlobby()
        {
            for (int y = 0; y < GraphicsDevice.Viewport.Height; y += pixelSize)
            {
                for (int x = 0; x < GraphicsDevice.Viewport.Width; x += pixelSize)
                {
                    for (int i = 0; i < totalCircles; i++)
                    {
                        if ((circles[i].radius * circles[i].radius) / ((x - circles[i].position.X) * (x - circles[i].position.X) + (y - circles[i].position.Y) * (y - circles[i].position.Y)) >= 1)
                        {
                            spriteBatch.Draw(pixelTexture, new Rectangle(x, y, pixelSize, pixelSize), null, Color.White);
                        }
                    }
                }
            }
        }

        private void DrawWithBlobby()
        {
            for (int y = 0; y < GraphicsDevice.Viewport.Height; y += pixelSize)
            {
                for (int x = 0; x < GraphicsDevice.Viewport.Width; x += pixelSize)
                {
                    if (GetPixelValue(x, y) >= blobbyness)
                    {
                        spriteBatch.Draw(pixelTexture, new Rectangle(x, y, pixelSize, pixelSize), null, Color.White);
                    }
                }
            }
        }

        private void DrawBlobbyMarching(Color color)
        {
            for (int y = 0; y < GraphicsDevice.Viewport.Height; y += pixelSize)
            {
                for (int x = 0; x < GraphicsDevice.Viewport.Width; x += pixelSize)
                {
                    // Lines will be drawn from left to right
                    // And if a line is vertical, it will be drawn from top to bottom
                    switch(new string(MarchingSquare(x, y)))
                    {
                        case "0001":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y + pixelSize), color);
                            break;
                        case "0010":
                            DrawLine(new Point(x + pixelSize / 2, y + pixelSize), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "0011":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "0100":
                            DrawLine(new Point(x + pixelSize / 2, y), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "0101":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y), color);
                            DrawLine(new Point(x + pixelSize / 2, y + pixelSize), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "0110":
                            DrawLine(new Point(x + pixelSize / 2, y), new Point(x + pixelSize / 2, y + pixelSize), color);
                            break;
                        case "0111":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y), color);
                            break;
                        case "1000":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y), color);
                            break;
                        case "1001":
                            DrawLine(new Point(x + pixelSize / 2, y), new Point(x + pixelSize / 2, y + pixelSize), color);
                            break;
                        case "1010":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y + pixelSize), color);
                            DrawLine(new Point(x + pixelSize / 2, y), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "1011":
                            DrawLine(new Point(x + pixelSize / 2, y), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "1100":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "1101":
                            DrawLine(new Point(x + pixelSize / 2, y + pixelSize), new Point(x + pixelSize, y + pixelSize / 2), color);
                            break;
                        case "1110":
                            DrawLine(new Point(x, y + pixelSize / 2), new Point(x + pixelSize / 2, y + pixelSize), color);
                            break;
                        case "1111":
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void DrawBlobbyMarchingInterpolated(Color color)
        {
            for (int y = 0; y < GraphicsDevice.Viewport.Height; y += pixelSize)
            {
                for (int x = 0; x < GraphicsDevice.Viewport.Width; x += pixelSize)
                {
                    // Lines will be drawn from left to right
                    // And if a line is vertical, it will be drawn from top to bottom
                    int xPos, yPos, xPos2, yPos2;
                    switch(new string(MarchingSquare(x, y)))
                    {
                        case "0001": //
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(x, yPos), new Point(xPos, y + pixelSize), color);
                            break;
                        case "0010":
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y + pixelSize), new Point(x + pixelSize, yPos), color);
                            break;
                        case "0011":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            yPos2 = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(x, yPos), new Point(x + pixelSize, yPos2), color);
                            break;
                        case "0100":
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y), new Point(x + pixelSize, yPos), color);
                            break;
                        case "0101":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            DrawLine(new Point(x, yPos), new Point(xPos, y), color);
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y + pixelSize), new Point(x + pixelSize, yPos), color);
                            break;
                        case "0110":
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            xPos2 = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y), new Point(xPos2, y + pixelSize), color);
                            break;
                        case "0111":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            DrawLine(new Point(x, yPos), new Point(xPos, y), color);
                            break;
                        case "1000":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            DrawLine(new Point(x, yPos), new Point(xPos, y), color);
                            break;
                        case "1001":
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            xPos2 = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y), new Point(xPos2, y + pixelSize), color);
                            break;
                        case "1010":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(x, yPos), new Point(xPos, y + pixelSize), color);
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y), new Point(x + pixelSize, yPos), color);
                            break;
                        case "1011":
                            xPos = (int)InterpolateX(new Point(x, y), new Point(x + pixelSize, y));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y), new Point(x + pixelSize, yPos), color);
                            break;
                        case "1100":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            yPos2 = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(x, yPos), new Point(x + pixelSize, yPos2), color);
                            break;
                        case "1101":
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            yPos = (int)InterpolateY(new Point(x + pixelSize, y), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(xPos, y + pixelSize), new Point(x + pixelSize, yPos), color);
                            break;
                        case "1110":
                            yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + pixelSize));
                            xPos = (int)InterpolateX(new Point(x, y + pixelSize), new Point(x + pixelSize, y + pixelSize));
                            DrawLine(new Point(x, yPos), new Point(xPos, y + pixelSize), color);
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        private float GetPixelValue(int x, int y)
        {
            float sigmaSum = 0f;
            for (int i = 0; i < totalCircles; i++)
            {
                sigmaSum += (circles[i].radius * circles[i].radius) / ((x - circles[i].position.X) * (x - circles[i].position.X) + (y - circles[i].position.Y) * (y - circles[i].position.Y));
            }
            return sigmaSum;
        }

        private char[] MarchingSquare(int x, int y)
        {
            char[] output = new char[4];
            output[0] = GetPixelValue(x, y) >= blobbyness ? '1' : '0';
            output[1] = GetPixelValue(x + pixelSize, y) >= blobbyness ? '1' : '0';
            output[2] = GetPixelValue(x + pixelSize, y + pixelSize) >= blobbyness ? '1' : '0';
            output[3] = GetPixelValue(x, y + pixelSize) >= blobbyness ? '1' : '0';
            return output;
        }

        private float InterpolateX(Point min, Point max)
        {
            float f = (float)min.X + ((float)max.X - (float)min.X) * ((blobbyness - GetPixelValue(min.X, min.Y)) / (GetPixelValue(max.X, max.Y) - GetPixelValue(min.X, min.Y)));
            if (f < min.X)
                return min.X;
            if (f > max.X)
                return max.X;
            return f;
        }

        private float InterpolateY(Point min, Point max)
        {
            float f = (float)min.Y + ((float)max.Y - (float)min.Y) * ((blobbyness - GetPixelValue(min.X, min.Y)) / (GetPixelValue(max.X, max.Y) - GetPixelValue(min.X, min.Y)));
            if (f < min.Y)
                return min.Y;
            if (f > max.Y)
                return max.Y;
            return f;
        }


        private Texture2D CircleText(int radius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    colorData[x * radius + y] = new Vector2(x - diam, y - diam).LengthSquared() <= diamsq ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }
        private float RandomFloat(float min, float max)
        {
            int decimalsMin = min.ToString().ToCharArray().Length;
            int decimalsMax = max.ToString().ToCharArray().Length;

            for (int i = 0; i < min.ToString().ToCharArray().Length; i++)
                if (min.ToString().ToCharArray()[i] == '.')
                {
                    decimalsMin = min.ToString().ToCharArray().Length - (i + 1);
                    break;
                }

            for (int i = 0; i < max.ToString().ToCharArray().Length; i++)
                if (max.ToString().ToCharArray()[i] == '.')
                {
                    decimalsMax = max.ToString().ToCharArray().Length - (i + 1);
                    break;
                }
            
            int accuracy;
            if (decimalsMin > decimalsMax)
                accuracy = (int)MathF.Pow(10, decimalsMin);
            else
                accuracy = (int)MathF.Pow(10, decimalsMax);

            return (float)(new Random().Next((int)min * accuracy, (int)max * accuracy)) / (float)accuracy;
        }

        private void DrawLine(Point start, Point end, Color color)
        {
            Vector2 edge = (end - start).ToVector2();
            float angle = (float)Math.Atan2(edge.Y , edge.X);

            Texture2D t = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            t.SetData<Color>(new Color[1] { color });

            spriteBatch.Draw(t, new Rectangle(start.X, start.Y, (int)edge.Length(), 1), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}