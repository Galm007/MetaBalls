using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MetaBalls
{
	public class Game1 : Game
	{
		private const float cellSize = 10.0f;
		private const int totalCircles = 15;
		private const float blobbyness = 1.0f;
		private const float speed = 120.0f;
		private readonly Color lineColor = Color.Yellow;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private GraphicsDevice gpu => GraphicsDevice;
		private int ScreenWidth => GraphicsDevice.Viewport.Width;
		private int ScreenHeight => GraphicsDevice.Viewport.Height;
		private Texture2D lineTexture;
		private Circle[] circles = new Circle[totalCircles];
 
		private struct Circle
 		{
 			public float radius;
 			public Vector2 position;
 			public Vector2 velocity;
 
 			public Circle(float radius, Vector2 position, Vector2 velocity)
 			{
 				this.radius = radius;
 				this.position = position;
 				this.velocity = velocity;
 			}
 		}
 
 		public Game1()
 		{
 			graphics = new GraphicsDeviceManager(this);
 			Content.RootDirectory = "Content";
 			IsMouseVisible = true;
 			IsFixedTimeStep = true;
 			TargetElapsedTime = TimeSpan.FromSeconds(1.0d / 60.0d);
 		}
 
 		protected override void Initialize()
 		{
 			graphics.PreferredBackBufferWidth = 1280;
 			graphics.PreferredBackBufferHeight = 720;
 			graphics.IsFullScreen = false;
 			graphics.ApplyChanges();
 			Window.Title = "MetaBalls!";
 
 			// Create texture
 			lineTexture = new Texture2D(GraphicsDevice, 1, 1);
 			lineTexture.SetData<Color>(new Color[1] { Color.White });

 			base.Initialize();
 		}
 
 		protected override void LoadContent()
 		{
 			spriteBatch = new SpriteBatch(GraphicsDevice);
 
			// Initialize circles with random values
 			for (int i = 0; i < totalCircles; i++)
 			{
				float radius, posX, posY, velX, velY;
 				radius = RandomFloat(60.0f, 90.0f);
				posX = RandomFloat(radius, ScreenWidth - radius);
				posY = RandomFloat(radius, ScreenHeight - radius);
				velX = RandomFloat(-speed, speed);
				velY = RandomFloat(-speed, speed);
 				circles[i] = new Circle(radius, new Vector2(posX, posY), new Vector2(velX, velY));
 			}
		}
 
 		protected override void Update(GameTime gameTime)
 		{
 			float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

 			for (int i = 0; i < totalCircles; i++)
 			{
				circles[i].position += (circles[i].velocity * frameTime);

 				if (circles[i].position.X < circles[i].radius || circles[i].position.X + circles[i].radius > ScreenWidth)
 					circles[i].velocity.X *= -1f;

 				if (circles[i].position.Y < circles[i].radius || circles[i].position.Y + circles[i].radius > ScreenHeight)
 					circles[i].velocity.Y *= -1f;
 			}
 
 			base.Update(gameTime);
 		}
 
 		protected override void Draw(GameTime gameTime)
 		{
 			GraphicsDevice.Clear(Color.Black);
 			spriteBatch.Begin();
 
 			DrawBlobbyMarchingInterpolated(lineColor);
 
 			spriteBatch.End();
 			base.Draw(gameTime);
		}
 
 		private void DrawBlobbyMarchingInterpolated(Color color)
 		{
 			for (float y = 0.0f; y < ScreenHeight; y += cellSize)
			for (float x = 0.0f; x < ScreenWidth; x += cellSize)
			{
				float xPos, yPos, xPos2, yPos2;
				switch(MarchSquare(x, y))
				{
					case 1:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						DrawLine(x, yPos, xPos, y + cellSize, color);
						break;
					case 2:
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
						break;
					case 3:
						yPos = InterpolateY(x, y, x, y + cellSize);
						yPos2 = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(x, yPos, x + cellSize, yPos2, color);
						break;
					case 4:
						xPos = InterpolateX(x, y, x + cellSize, y);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y, x + cellSize, yPos, color);
						break;
					case 5:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y, x + cellSize, y);
						DrawLine(x, yPos, xPos, y, color);
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
						break;
					case 6:
						xPos = InterpolateX(x, y, x + cellSize, y);
						xPos2 = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						DrawLine(xPos, y, xPos2, y + cellSize, color);
						break;
					case 7:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y, x + cellSize, y);
						DrawLine(x, yPos, xPos, y, color);
						break;
					case 8:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y, x + cellSize, y);
						DrawLine(x, yPos, xPos, y, color);
						break;
					case 9:
						xPos = InterpolateX(x, y, x + cellSize, y);
						xPos2 = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						DrawLine(xPos, y, xPos2, y + cellSize, color);
						break;
					case 10:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						DrawLine(x, yPos, xPos, y + cellSize, color);
						xPos = InterpolateX(x, y, x + cellSize, y);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y, x + cellSize, yPos, color);
						break;
					case 11:
						xPos = InterpolateX(x, y, x + cellSize, y);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y, x + cellSize, yPos, color);
						break;
					case 12:
						yPos = InterpolateY(x, y, x, y + cellSize);
						yPos2 = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(x, yPos, x + cellSize, yPos2, color);
						break;
					case 13:
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						yPos = InterpolateY(x + cellSize, y, x + cellSize, y + cellSize);
						DrawLine(xPos, y + cellSize, x + cellSize, yPos, color);
						break;
					case 14:
						yPos = InterpolateY(x, y, x, y + cellSize);
						xPos = InterpolateX(x, y + cellSize, x + cellSize, y + cellSize);
						DrawLine(x, yPos, xPos, y + cellSize, color);
						break;
					default:
						break;
				}
			}
 		}
 
		private float RandomFloat(float min, float max)
		{
			float diff = max - min;
			Random random = new Random();
			return (float)random.NextDouble() * diff + min;
		}

 		private byte MarchSquare(float x, float y)
 		{
 			byte output = 0;
 			if (CalculateCellValue(x, y + cellSize) >= blobbyness)            output |= 1;
 			if (CalculateCellValue(x + cellSize, y + cellSize) >= blobbyness) output |= 2;
 			if (CalculateCellValue(x + cellSize, y) >= blobbyness)            output |= 4;
 			if (CalculateCellValue(x, y) >= blobbyness)                       output |= 8;
 			return output;
 		}
 
 		private float CalculateCellValue(float x, float y)
 		{
 			float sigmaSum = 0f;
			foreach (Circle circle in circles)
			{
				float rad_sq, diffX_sq, diffY_sq;
				rad_sq = circle.radius * circle.radius;
				diffX_sq = (x - circle.position.X) * (x - circle.position.X);
				diffY_sq = (y - circle.position.Y) * (y - circle.position.Y);
 				sigmaSum += rad_sq / (diffX_sq + diffY_sq);
			}
 			return sigmaSum;
 		}

 		private float InterpolateX(float minX, float minY, float maxX, float maxY)
 		{
 			float f = minX + (maxX - minX) * ((blobbyness - CalculateCellValue(minX, minY)) / (CalculateCellValue(maxX, maxY) - CalculateCellValue(minX, minY)));
 			if (f < minX) return minX;
 			if (f > maxX) return maxX;
 			return f;
 		}
 
 		private float InterpolateY(float minX, float minY, float maxX, float maxY)
 		{
 			float f = minY + (maxY - minY) * ((blobbyness - CalculateCellValue(minX, minY)) / (CalculateCellValue(maxX, maxY) - CalculateCellValue(minX, minY)));
 			if (f < minY) return minY;
 			if (f > maxY) return maxY;
 			return f;
 		}

  		private void DrawLine(float startX, float startY, float endX, float endY, Color color)
		{
 			Vector2 edge = new Vector2(endX - startX, endY - startY);
 			float angle = MathF.Atan2(edge.Y , edge.X);
			Rectangle rectangle = new Rectangle((int)startX, (int)startY, (int)edge.Length(), 1);
 			spriteBatch.Draw(lineTexture, rectangle, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
		}
		
	}
}
