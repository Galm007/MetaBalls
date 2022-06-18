using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MetaBalls
{
	public class Game1 : Game
	{
		const int   ScreenWidth  = 800;
		const int   ScreenHeight = 600;
		const float CellSize     = 10.0f;
		const int   TotalCircles = 10;
		const float Blobbyness   = 1.6f;
		const float Speed        = 100.0f;

		readonly Color lineColor = Color.Lime;

		// single-pixel texture for drawing lines
		Texture2D lineTexture;
		Circle[] circles = new Circle[TotalCircles];

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		class Circle
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

			public void Update(
				float frameTime,
				int screenWidth,
				int screenHeight)
			{
				// move the circle
				position += velocity * frameTime;

				// bounce when hitting the edge of the screen
 				if (position.X < radius || position.X + radius > screenWidth)
					velocity.X = -velocity.X;
 				if (position.Y < radius || position.Y + radius > screenHeight)
 					velocity.Y = -velocity.Y;
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
 			graphics.PreferredBackBufferWidth = 800;
 			graphics.PreferredBackBufferHeight = 600;
 			graphics.IsFullScreen = false;
 			graphics.ApplyChanges();

 			Window.Title = "MetaBalls!";

 			// Create texture for drawing lines
 			lineTexture = new Texture2D(GraphicsDevice, 1, 1);
 			lineTexture.SetData<Color>(new Color[1] { Color.White });

 			base.Initialize();
 		}

 		protected override void LoadContent()
 		{
 			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Initialize circles with random values
			float radius, posX, posY, velX, velY;
			Random rand = new Random();
 			for (int i = 0; i < TotalCircles; i++)
 			{
 				radius = RandomFloat(rand, 60.0f, 90.0f);
				posX   = RandomFloat(rand, radius, ScreenWidth - radius);
				posY   = RandomFloat(rand, radius, ScreenHeight - radius);
				velX   = RandomFloat(rand, -Speed, Speed);
				velY   = RandomFloat(rand, -Speed, Speed);

 				circles[i] = new Circle(
						radius,
						new Vector2(posX, posY),
						new Vector2(velX, velY));
 			}
		}

 		protected override void Update(GameTime gameTime)
 		{
 			float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			
			// move circles
 			for (int i = 0; i < TotalCircles; i++)
				circles[i].Update(frameTime, ScreenWidth, ScreenHeight);

 			base.Update(gameTime);
 		}

 		protected override void Draw(GameTime gameTime)
 		{
 			GraphicsDevice.Clear(Color.Black);
 			spriteBatch.Begin();
 			DrawMetaBalls();
 			spriteBatch.End();
 			base.Draw(gameTime);
		}

 		private void DrawMetaBalls()
 		{
			float xPos, yPos, xPos2, yPos2;

			/*
			 * idk how to explain this
			 *
			 * just read this article by jamie wong to
			 * understand whats happening:
			 * http://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
			 */

 			for (float y = 0.0f; y < ScreenHeight; y += CellSize)
			for (float x = 0.0f; x < ScreenWidth;  x += CellSize)
			{
				switch(MarchSquare(x, y))
				{
				case 1:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					DrawLine(x, yPos, xPos, y + CellSize);
					break;
				case 2:
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y + CellSize, x + CellSize, yPos);
					break;
				case 3:
					yPos = InterpolateY(x, y, x, y + CellSize);
					yPos2 = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(x, yPos, x + CellSize, yPos2);
					break;
				case 4:
					xPos = InterpolateX(x, y, x + CellSize, y);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y, x + CellSize, yPos);
					break;
				case 5:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y, x + CellSize, y);
					DrawLine(x, yPos, xPos, y);
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y + CellSize, x + CellSize, yPos);
					break;
				case 6:
					xPos = InterpolateX(x, y, x + CellSize, y);
					xPos2 = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					DrawLine(xPos, y, xPos2, y + CellSize);
					break;
				case 7:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y, x + CellSize, y);
					DrawLine(x, yPos, xPos, y);
					break;
				case 8:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y, x + CellSize, y);
					DrawLine(x, yPos, xPos, y);
					break;
				case 9:
					xPos = InterpolateX(x, y, x + CellSize, y);
					xPos2 = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					DrawLine(xPos, y, xPos2, y + CellSize);
					break;
				case 10:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					DrawLine(x, yPos, xPos, y + CellSize);
					xPos = InterpolateX(x, y, x + CellSize, y);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y, x + CellSize, yPos);
					break;
				case 11:
					xPos = InterpolateX(x, y, x + CellSize, y);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y, x + CellSize, yPos);
					break;
				case 12:
					yPos = InterpolateY(x, y, x, y + CellSize);
					yPos2 = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(x, yPos, x + CellSize, yPos2);
					break;
				case 13:
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					yPos = InterpolateY(x + CellSize, y, x + CellSize, y + CellSize);
					DrawLine(xPos, y + CellSize, x + CellSize, yPos);
					break;
				case 14:
					yPos = InterpolateY(x, y, x, y + CellSize);
					xPos = InterpolateX(x, y + CellSize, x + CellSize, y + CellSize);
					DrawLine(x, yPos, xPos, y + CellSize);
					break;
				default:
					break;
				}
			}
 		}

		float RandomFloat(Random random, float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}

 		byte MarchSquare(float x, float y)
 		{
 			byte output = 0;

			// flip a corresponding bit for each
			// cell corner value that is above Blobbyness

 			if (CellValue(x, y + CellSize) >= Blobbyness)
				output |= 1;
 			if (CellValue(x + CellSize, y + CellSize) >= Blobbyness)
				output |= 2;
 			if (CellValue(x + CellSize, y) >= Blobbyness)
				output |= 4;
 			if (CellValue(x, y) >= Blobbyness)
				output |= 8;

 			return output;
 		}

 		float CellValue(float x, float y)
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

 		float InterpolateX(
			float minX,
			float minY,
			float maxX,
			float maxY)
 		{
			float blob, celldiff, f;

			blob = Blobbyness - CellValue(minX, minY);
			celldiff = CellValue(maxX, maxY) - CellValue(minX, minY);
 			f = minX + (maxX - minX) * (blob / celldiff);

			return f < minX ? minX : f > maxX ? maxX : f;
 		}

 		float InterpolateY(
			float minX,
			float minY,
			float maxX,
			float maxY)
 		{
			float blob, celldiff, f;

			blob = Blobbyness - CellValue(minX, minY);
			celldiff = CellValue(maxX, maxY) - CellValue(minX, minY);
 			f = minY + (maxY - minY) * (blob / celldiff);

			return f < minY ? minY : f > maxY ? maxY : f;
 		}

  		void DrawLine(
			float startX,
			float startY,
			float endX,
			float endY)
		{
 			Vector2 edge = new Vector2(endX - startX, endY - startY);
 			float angle = MathF.Atan2(edge.Y , edge.X);

			Rectangle rectangle = new Rectangle(
				(int)startX,
				(int)startY,
				(int)edge.Length(),
				1);

 			spriteBatch.Draw(
				lineTexture,
				rectangle,
				null,
				lineColor,
				angle,
				Vector2.Zero,
				SpriteEffects.None,
				0);
		}
		
	}
}
