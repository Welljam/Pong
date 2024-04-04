using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;


namespace pong
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		SpriteFont arial;
		Vector2 poängPosition = new Vector2(350, 50);
		int vänsterPoäng = 0;
		int högerPoäng = 0;

		//boll1
		Vector2 bollPosition;
		Vector2 bollHastighet;
		Rectangle bollHitbox;
		Texture2D bollBild;

		//boll2
		Vector2 bollPosition2 = new Vector2(0, -100);
		Vector2 bollHastighet2;
		Rectangle bollHitbox2;
		Texture2D bollBild2;

        //paddles
        Rectangle vänsterPaddle;
		Rectangle högerPaddle;
		Rectangle datorpaddle;
		Texture2D paddleBild;

		//tangent
		KeyboardState tangentBord = Keyboard.GetState();
		KeyboardState gammaltTangentbord = Keyboard.GetState();

		//random
		Random slump = new Random();

		//tid variabel
		int tid = 0;
		int tid2 = 0;

		//mus
		MouseState mus = Mouse.GetState();
		MouseState gammalMus = Mouse.GetState();

		//datorspelare
		Texture2D player1Bild;
		Rectangle player1Rect;

		//player vs player
		Texture2D player2Bild;
		Rectangle player2Rect;

		//scen varibel
		int scen = 0;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			IsMouseVisible = true;
			do
			{
				bollHastighet.X = slump.Next(-5, 5);
				bollHastighet.Y = slump.Next(-5, 5);
			} while (bollHastighet.X == 0 || bollHastighet.X == 1 || bollHastighet.X == -1 
			|| bollHastighet.Y == 0 || bollHastighet.Y == 1 || bollHastighet.Y == -1);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// TODO: use this.Content to load your game content here
			spriteBatch = new SpriteBatch(GraphicsDevice);

			bollBild = Content.Load<Texture2D>("boll");
			bollHitbox = new Rectangle(0, 0, bollBild.Width, bollBild.Height);
			NyBoll();

            bollBild2 = Content.Load<Texture2D>("boll");
            bollHitbox2 = new Rectangle(0, 0, bollBild2.Width, bollBild2.Height);

			paddleBild = Content.Load<Texture2D>("paddle");
			vänsterPaddle = new Rectangle(50, 250, paddleBild.Width, paddleBild.Height);
			högerPaddle = new Rectangle(750 - paddleBild.Width, 250, paddleBild.Width, paddleBild.Height);
			datorpaddle = new Rectangle(750 - paddleBild.Width, 250, paddleBild.Width, paddleBild.Height);

			player1Bild = Content.Load<Texture2D>("player1");
			player1Rect = new Rectangle(500, 125, player1Bild.Width-1000, player1Bild.Height - 1000);

			player2Bild = Content.Load<Texture2D>("player2");
			player2Rect = new Rectangle(100, 100, player1Bild.Width - 950, player1Bild.Height - 950);

			arial = Content.Load<SpriteFont>("arial");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			gammaltTangentbord = tangentBord;
			tangentBord = Keyboard.GetState();

			//mus logik
			gammalMus = mus;
			mus = Mouse.GetState();

			//extraboll gör så båda vänder, vänsterpaddle.
			switch (scen)
			{
				case 0:
					UppdateraMeny();
					break;

				case 1:
                    bollPosition += bollHastighet;
                    bollPosition2 += bollHastighet2;

                    KollaKollisioner();
					FlyttaPaddles();
					KollaPoäng();
					Paddlegräns();
					Datorspelare();
					Bollriktning();

					if (tangentBord.IsKeyDown(Keys.B) && gammaltTangentbord.IsKeyUp(Keys.B))
					{
						Extraboll();
					}
					ExtrabollKollisioner();
					UppdateraSpel();
					break;

				case 2:
                    bollPosition += bollHastighet;
                    bollPosition2 += bollHastighet2;

                    KollaKollisioner();
					FlyttaPaddles();
					KollaPoäng();
					Paddlegräns();
					Bollriktning();

                    if (tangentBord.IsKeyDown(Keys.B) && gammaltTangentbord.IsKeyUp(Keys.B))
                    {
                        Extraboll();

                    }
                    ExtrabollKollisioner();
					UppdateraSpel();
                    break;
			}
		  

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			// TODO: Add your drawing code here
			spriteBatch.Begin();

			switch (scen)
			{
				case 0:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Draw(player1Bild, player1Rect, Color.White);
                    spriteBatch.Draw(player2Bild, player2Rect, Color.White);
                    spriteBatch.DrawString(arial, $"{vänsterPoäng} - {högerPoäng}", poängPosition, Color.White);
                    break;
				case 1:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.DrawString(arial, $"{vänsterPoäng} - {högerPoäng}", poängPosition, Color.White);
                    spriteBatch.Draw(bollBild, bollPosition, Color.White);
                    spriteBatch.Draw(bollBild, bollPosition2, Color.White);

                    spriteBatch.Draw(paddleBild, vänsterPaddle, Color.White);
                    spriteBatch.Draw(paddleBild, datorpaddle, Color.White);
                    break;
				case 2:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.DrawString(arial, $"{vänsterPoäng} - {högerPoäng}", poängPosition, Color.White);
                    spriteBatch.Draw(bollBild, bollPosition, Color.White);
                    spriteBatch.Draw(bollBild, bollPosition2, Color.White);

                    spriteBatch.Draw(paddleBild, vänsterPaddle, Color.White);
                    spriteBatch.Draw(paddleBild, högerPaddle, Color.White);
                    break;
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}


		/// <summary>
		/// Placerar bollen i mitten av spelplanen med en slumpad hastighet
		/// </summary>
		void NyBoll()
		{
			bollPosition.X = 400 - (bollBild.Width / 2);
			bollPosition.Y = 240 - (bollBild.Height / 2);

			do
			{
				bollHastighet.X = slump.Next(-5, 5);
				bollHastighet.Y = slump.Next(-5, 5);
			} while (bollHastighet.X == 0 || bollHastighet.X == 1 || bollHastighet.X == -1
			|| bollHastighet.Y == 0 || bollHastighet.Y == 1 || bollHastighet.Y == -1);
		}

        /// <summary>
        /// ger en slumpmässig hastighet till boll2
        /// </summary>
        void Extraboll()
        {
            bollPosition2.X = 400 - (bollBild.Width / 2);
            bollPosition2.Y = 240 - (bollBild.Height / 2);

            do
            {
                bollHastighet2.X = slump.Next(-5, 5);
                bollHastighet2.Y = slump.Next(-5, 5);
            } while (bollHastighet2.X == 0 || bollHastighet2.X == 1 || bollHastighet2.X == -1
            || bollHastighet2.Y == 0 || bollHastighet2.Y == 1 || bollHastighet2.Y == -1);
        }


        /// <summary>
        /// Ser till så att bollen studsar mot paddles och väggarna
        /// </summary>
        void KollaKollisioner()
		{
			bollHitbox.X = (int)bollPosition.X;
			bollHitbox.Y = (int)bollPosition.Y;

			tid--;
			if (tid < 0)
			{
				if (bollHitbox.Intersects(vänsterPaddle) == true || bollHitbox.Intersects(högerPaddle) == true || bollHitbox.Intersects(datorpaddle) == true)
				{
					bollHastighet.X *= -1;
					tid = 30;
				}

			}

		if (bollPosition.Y < 0 || bollPosition.Y + bollBild.Height > 480)
			{
				bollHastighet.Y *= -1;
			}

		}

		/// <summary>
		/// kollar efter kollisioner med bollen för boll2
		/// </summary>
		void ExtrabollKollisioner()
		{
			//boll2
			bollHitbox.X = (int)bollPosition2.X;
			bollHitbox.Y = (int)bollPosition2.Y;

			tid2--;
			if (tid2 < 0)
			{
				if (bollHitbox.Intersects(vänsterPaddle) == true || bollHitbox.Intersects(högerPaddle) == true || bollHitbox.Intersects(datorpaddle) == true)
				{
					bollHastighet2.X *= -1;
                    tid2 = 30;
				}
			}

			if (bollPosition2.Y < 0 || bollPosition2.Y + bollBild.Height > 480)
			{
				bollHastighet2.Y *= -1;
			}
		}

		/// <summary>
		/// Hanterar förflyttningen av spelarnas paddles
		/// </summary>
		void FlyttaPaddles()
		{
			if (tangentBord.IsKeyDown(Keys.W) == true)
			{
				vänsterPaddle.Y -= 5;
			}
			if (tangentBord.IsKeyDown(Keys.S) == true)
			{
				vänsterPaddle.Y += 5;
			}
			if (tangentBord.IsKeyDown(Keys.Up) == true)
			{
				högerPaddle.Y -= 5;
			}
			if (tangentBord.IsKeyDown(Keys.Down) == true)
			{
				högerPaddle.Y += 5;
			}
		}

		/// <summary>
		/// Ger poäng om bollen kommit i mål och startar då om bollen i mitten av planen
		/// </summary>
		void KollaPoäng()
		{
			if (bollPosition.X < 0)
			{
				högerPoäng++;
				NyBoll();
			}
			else if (bollPosition.X > 800)
			{
				vänsterPoäng++;
				NyBoll();
			}

			//boll2
			if (bollPosition2.X < 0)
			{
				högerPoäng++;
				Extraboll();
			}
			else if (bollPosition2.X > 800)
			{
				vänsterPoäng++;
				Extraboll();
			}
		}

		/// <summary>
		/// skickar paddles till motsatt sida ifall dom åker ner för långt
		/// </summary>
		void Paddlegräns()
		{
			if (vänsterPaddle.Y > 480)
			{
				vänsterPaddle.Y = -50;
			}

			if (högerPaddle.Y > 480)
			{
				högerPaddle.Y = -50;
			}

			if (vänsterPaddle.Y < -50)
			{
				vänsterPaddle.Y = 480;
			}

			if (högerPaddle.Y < -50)
			{
				högerPaddle.Y = 480;
			}

			if (datorpaddle.Y > 480)
			{
				datorpaddle.Y = -50;
			}

			if (datorpaddle.Y < -50)
			{
				datorpaddle.Y = 480;
			}

		}

		/// <summary>
		/// Skapar en datorspelare
		/// </summary>
		void Datorspelare()
		{
			if (bollHastighet.X > 0 && bollHastighet2.X > 0)
			{
				do
				{
					datorpaddle.Y += 2;
				} while (bollHastighet.X < 0 && bollHastighet2.X < 0);
			}

			if (bollPosition.Y > datorpaddle.Y && bollHastighet.X > 0)
			{
				datorpaddle.Y += 2;
			}
			else if (bollPosition.Y < datorpaddle.Y && bollHastighet.X > 0)
			{
				datorpaddle.Y -= 2;
			}

			if (bollPosition2.Y > datorpaddle.Y && bollHastighet2.X > 0)
			{
				datorpaddle.Y += 2;
			}
			else if (bollPosition2.Y < datorpaddle.Y && bollHastighet2.X > 0)
			{
				datorpaddle.Y -= 2;
			}
		}

		/// <summary>
		/// förändrar bollens riktningen beroende på vart den träffar paddlen
		/// </summary>
		void Bollriktning()
		{
			if (bollHitbox.Intersects(vänsterPaddle) == true)
			{
				float along = ((float)bollHitbox.Y - (float)vänsterPaddle.Y) / (float)vänsterPaddle.Height - 0.5f;
				float angle = along * MathF.PI * 0.5f;
				bollHastighet = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * bollHastighet.Length();

			}

			if (bollHitbox.Intersects(datorpaddle) == true)
			{
				float along = ((float)bollHitbox.Y - (float)datorpaddle.Y) / (float)datorpaddle.Height - 0.5f;
				float angle = -along * MathF.PI * 0.5f + MathF.PI;
				bollHastighet = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * bollHastighet.Length();

			}

			//boll2
			if (bollHitbox2.Intersects(vänsterPaddle) == true)
			{
				float along = ((float)bollHitbox2.Y - (float)vänsterPaddle.Y) / (float)vänsterPaddle.Height - 0.5f;
				float angle = along * MathF.PI * 0.5f;
				bollHastighet = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * bollHastighet.Length();

			}

			if (bollHitbox2.Intersects(datorpaddle) == true)
			{
				float along = ((float)bollHitbox2.Y - (float)datorpaddle.Y) / (float)datorpaddle.Height - 0.5f;
				float angle = -along * MathF.PI * 0.5f + MathF.PI;
				bollHastighet = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * bollHastighet.Length();

			}

		}

		/// <summary>
		/// ser till att vänstermusknapp trycks
		/// </summary>
		/// <returns></returns>
		bool VänsterMusTryckt()
		{
			if (mus.LeftButton == ButtonState.Pressed && gammalMus.LeftButton == ButtonState.Released)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// byter scen
		/// </summary>
		/// <param name="nyscen"></param>
		void BytScen(int nyscen)
		{
			scen = nyscen;
		}

		/// <summary>
		/// spelaren får välja ifall dem vill köra mot datorn eller en annan spelare
		/// </summary>
		void UppdateraMeny()
		{
			if (VänsterMusTryckt() && player1Rect.Contains(mus.Position))
			{
				BytScen(1);
                högerPoäng = 0;
                vänsterPoäng = 0;
            }

            if (VänsterMusTryckt() && player2Rect.Contains(mus.Position))
            {
                BytScen(2);
                högerPoäng = 0;
                vänsterPoäng = 0;
            }
        }

		/// <summary>
		/// Ifall nån av spelarna får 5 poäng skickas dom tillbaka till startmeny
		/// </summary>
		void UppdateraSpel()
		{
			if (högerPoäng == 5 || vänsterPoäng == 5)
			{
				BytScen(0);
				
				bollPosition2.X = 0;
				bollPosition2.Y = -100;
				bollHastighet2.X = 0;
                bollHastighet2.Y = 0;
                bollPosition.X = 400 - (bollBild.Width / 2);
                bollPosition.Y = 240 - (bollBild.Height / 2);
            }
		}

	}
}