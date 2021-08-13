using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MetaBalls
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Texture2D pixelTexture;
		private const int cellSize = 13;

		private const int totalCircles = 15;
		private const float blobbyness = 1f;
		private Circle[] circles = new Circle[totalCircles];

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
			IsFixedTimeStep = true;
			TargetElapsedTime = TimeSpan.FromSeconds(0.015d);
		}

		protected override void Initialize()
		{
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			Window.Title = "MetaBalls!";

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Create texture
			pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
			pixelTexture.SetData<Color>(new Color[1] { Color.Green });

			for (int i = 0; i < totalCircles; i++)
			{
				float radius = (float)new Random().Next(400, 700) * 0.1f;
				Vector2 position = new Vector2 (
					new Random().Next((int)radius, GraphicsDevice.Viewport.Width - (int)radius),
					new Random().Next((int)radius, GraphicsDevice.Viewport.Height - (int)radius)
				);
				circles[i] = new Circle(radius, position, GraphicsDevice);
			}
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			for (int i = 0; i < totalCircles; i++)
			{
				circles[i].position += (circles[i].velocity * frameTime);

				if (circles[i].position.X < circles[i].radius || circles[i].position.X + circles[i].radius > GraphicsDevice.Viewport.Width)
					circles[i].velocity.X *= -1f;
					
				if (circles[i].position.Y < circles[i].radius || circles[i].position.Y + circles[i].radius > GraphicsDevice.Viewport.Height)
					circles[i].velocity.Y *= -1f;
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();

			DrawBlobbyMarchingInterpolated(Color.Green);

			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void DrawBlobbyMarchingInterpolated(Color color)
		{
			for (int y = 0; y < GraphicsDevice.Viewport.Height; y += cellSize)
			{
				for (int x = 0; x < GraphicsDevice.Viewport.Width; x += cellSize)
				{
					int xPos, yPos, xPos2, yPos2;
					switch(MarchingSquare(x, y))
					{
						case 1:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							DrawLine(x, yPos, xPos, y + cellSize, color);
							break;
						case 2:
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
							break;
						case 3:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							yPos2 = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(x, yPos, x + cellSize, yPos2, color);
							break;
						case 4:
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y, x + cellSize, yPos, color);
							break;
						case 5:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							DrawLine(x, yPos, xPos, y, color);
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
							break;
						case 6:
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							xPos2 = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y, xPos2, y + cellSize, color);
							break;
						case 7:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							DrawLine(x, yPos, xPos, y, color);
							break;
						case 8:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							DrawLine(x, yPos, xPos, y, color);
							break;
						case 9:
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							xPos2 = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y, xPos2, y + cellSize, color);
							break;
						case 10:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							DrawLine(x, yPos, xPos, y + cellSize, color);
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y, x + cellSize, yPos, color);
							break;
						case 11:
							xPos = (int)InterpolateX(new Point(x, y), new Point(x + cellSize, y));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y, x + cellSize, yPos, color);
							break;
						case 12:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							yPos2 = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(x, yPos, x + cellSize, yPos2, color);
							break;
						case 13:
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							yPos = (int)InterpolateY(new Point(x + cellSize, y), new Point(x + cellSize, y + cellSize));
							DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
							break;
						case 14:
							yPos = (int)InterpolateY(new Point(x, y), new Point(x, y + cellSize));
							xPos = (int)InterpolateX(new Point(x, y + cellSize), new Point(x + cellSize, y + cellSize));
							DrawLine(x, yPos, xPos, y + cellSize, color);
							break;
						default:
							break;
					}
				}
			}
		}

		private byte MarchingSquare(int x, int y)
		{
			byte output = 0;
			if (GetPixelValue(x, y + cellSize) >= blobbyness)
				output |= 1;
			if (GetPixelValue(x + cellSize, y + cellSize) >= blobbyness)
				output |= 2;
			if (GetPixelValue(x + cellSize, y) >= blobbyness)
				output |= 4;
			if (GetPixelValue(x, y) >= blobbyness)
				output |= 8;
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

		private float GetPixelValue(int x, int y)
		{
			float sigmaSum = 0f;
			for (int i = 0; i < totalCircles; i++)
				sigmaSum += (circles[i].radius * circles[i].radius) / ((x - circles[i].position.X) * (x - circles[i].position.X) + (y - circles[i].position.Y) * (y - circles[i].position.Y));
			return sigmaSum;
		}

		private void DrawLine(int startX, int startY, int endX, int endY, Color color)
		{
			Vector2 edge = new Vector2(endX - startX, endY - startY);
			float angle = (float)Math.Atan2(edge.Y , edge.X);

			spriteBatch.Draw(pixelTexture, new Rectangle(startX, startY, (int)edge.Length(), 1), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
		}
		
	}
}
