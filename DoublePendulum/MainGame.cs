#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;

#endregion

namespace DoublePendulum
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : Game
	{
		GraphicsDeviceManager graphics;

		SpriteBatch spriteBatch;

		int count=0;
		float fadeMultiplier=0.96f;

		float timestep = 0.005f;

		float energyAverage = 0;

		SpriteFont font;

		Texture2D circle;

		Texture2D pix;

		Texture2D thetaTex1, dThetaTex1,thetaTex2, dThetaTex2;

		List<PhysSystem> systems;
		int currSystem = 0;

		KeyboardState prevKeyState;

		public MainGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
			this.IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			circle = Content.Load<Texture2D> ("circle.png");

			thetaTex1 = Content.Load <Texture2D>("theta1.png");
			dThetaTex1 = Content.Load <Texture2D>("dtheta1.png");

			thetaTex2 = Content.Load <Texture2D>("theta2.png");
			dThetaTex2 = Content.Load <Texture2D>("dtheta2.png");

			font = Content.Load<SpriteFont> ("Arial");

			pix = new Texture2D (graphics.GraphicsDevice, 1, 1);
			pix.SetData<Color> (new Color[]{ Color.White });
			systems = new List<PhysSystem> ();
			systems.Add(new DoublePendulum (new Vector2 (100, 100), graphics.GraphicsDevice, circle));
			systems.Add(new SinglePendulum (new Vector2 (100, 100), graphics.GraphicsDevice, circle));


//			font = Content.Load<SpriteFont> ("Comic.spritefont");
			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
			    Keyboard.GetState ().IsKeyDown (Keys.Escape)) {
				Exit ();
			}
			#endif
			for (int i = 0; i < 10; i++) {
				systems[currSystem].Update (gameTime, timestep);
			}
			count++;
			Console.WriteLine ("Avg energy: {0}", energyAverage / count);
			base.Update (gameTime);

			KeyboardState keyState = Keyboard.GetState ();

			if (keyState.IsKeyDown (Keys.Tab) && prevKeyState.IsKeyUp (Keys.Tab)) {
				currSystem++;
				if (currSystem >= systems.Count)
					currSystem = 0;
			}

			var mstate = Mouse.GetState ();
			if (mstate.LeftButton == ButtonState.Pressed)
				systems [currSystem].SetState (mstate.Position.ToVector2 ());
			prevKeyState = keyState;

		}



		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.White);

			spriteBatch.Begin ();


			systems[currSystem].Draw (spriteBatch, font, circle, pix);


//			spriteBatch.Draw (phasePortrait1, new Rectangle((int)phasePortrait1Position.X,(int)phasePortrait1Position.Y,phaseresolution,phaseresolution), Color.White);
//			spriteBatch.Draw (phasePortrait2, new Rectangle((int)phasePortrait2Position.X,(int)phasePortrait2Position.Y,phaseresolution,phaseresolution), Color.White);
//			spriteBatch.Draw (thetaTex1, phasePortrait1Position + new Vector2(-20,phaseresolution/2), Color.White);
//			spriteBatch.Draw (dThetaTex1, phasePortrait1Position + new Vector2(phaseresolution/2,-20), Color.White);
//			spriteBatch.Draw (thetaTex2, phasePortrait2Position + new Vector2(-20,phaseresolution/2), Color.White);
//			spriteBatch.Draw (dThetaTex2, phasePortrait2Position + new Vector2(phaseresolution/2,-20), Color.White);
//
//
//
//			spriteBatch.DrawString (font, "Phase Portrait 1", phasePortrait1Position + new Vector2(phaseresolution/2 - font.MeasureString("Phase Portrait 1").X/2,-80), Color.Black);
//			spriteBatch.DrawString (font, "Phase Portrait 2", phasePortrait2Position + new Vector2(phaseresolution/2 - font.MeasureString("Phase Portrait 2").X/2,-80), Color.Black);
//
			spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}