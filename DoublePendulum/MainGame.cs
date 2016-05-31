#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;
using System.IO;

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
		RenderTarget2D screenshot;

		KeyboardState prevKeyState;
		const int VIEWPORT_WIDTH = 1024;
		const int VIEWPORT_HEIGHT = 768;

		const int CAPTURE_PERIOD = 50;
		const bool CAPTURE = false;
		const string CAPTURE_SAVE_LOCATION = "/Users/christian/Desktop/pendulum_screenshots";
		Rectangle CAPTURE_RECTANGLE = new Rectangle(100,200, 200,200);
//		Rectangle CAPTURE_RECTANGLE = new Rectangle(100,0, VIEWPORT_WIDTH-2*100, 400);
		const bool CROP_CAPTURE = true;

		public MainGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = VIEWPORT_WIDTH;
			graphics.PreferredBackBufferHeight =VIEWPORT_HEIGHT;
			this.IsMouseVisible = true;
			graphics.IsFullScreen = false;

			//1024,768
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			if (CAPTURE) ClearCaptureFolder ();
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
			Texture2D bpix = new Texture2D (graphics.GraphicsDevice, 1, 1);
			bpix.SetData<Color> (new Color[]{ Color.Black });
			systems = new List<PhysSystem> ();
			systems.Add(new HarmonicOscillator (new Vector2 (512, 40), graphics.GraphicsDevice, circle));
			systems.Add(new SinglePendulum (new Vector2 (512, 40), graphics.GraphicsDevice, circle));
			systems.Add(new DoublePendulum (new Vector2 (512, 40), graphics.GraphicsDevice, circle, Color.Black));
			systems.Add(new DoublePendulumEnsemble (new Vector2 (512, 40), graphics.GraphicsDevice, circle));
			//systems.Add(new HarmonicOscillatorEnsemble (new Vector2 (512, 40), graphics.GraphicsDevice, circle, Content.Load<Texture2D>("smiley")));
			//systems.Add (new SinglePendulumEnsemble (new Vector2 (512, 40), graphics.GraphicsDevice, circle, Content.Load<Texture2D>("smiley")));
//			systems.Add(new SpringPendulum (new Vector2 (512, 40), graphics.GraphicsDevice, circle));

			if (CAPTURE) {
				screenshot = new RenderTarget2D (GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);


				GraphicsDevice.SetRenderTarget (screenshot);
			}
//			font = Content.Load<SpriteFont> ("Comic.spritefont");
			//TODO: use this.Content to load your game content here 
		}

		public void ClearCaptureFolder()
		{
			if (!Directory.Exists (CAPTURE_SAVE_LOCATION))
				Directory.CreateDirectory (CAPTURE_SAVE_LOCATION);
			string[] files = Directory.GetFiles (CAPTURE_SAVE_LOCATION);
			foreach (string f in files) {
				File.Delete (f);
			}
		}

		public Texture2D TakeScreenshot(GameTime gameTime)
		{
			int w, h;
			w = GraphicsDevice.PresentationParameters.BackBufferWidth;
			h = GraphicsDevice.PresentationParameters.BackBufferHeight;
			// _lastUpdatedGameTime is a variable typed GameTime, used to record the time last updated and create a common time standard for some game components
			Draw(gameTime);
			GraphicsDevice.Present();
//			GraphicsDevice.SetRenderTarget(null);
			return screenshot;
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
			Console.WriteLine ("Avg energy: {0}", energyAverage / count);
			base.Update (gameTime);

			KeyboardState keyState = Keyboard.GetState ();

			if (keyState.IsKeyDown (Keys.Tab) && prevKeyState.IsKeyUp (Keys.Tab)) {
				currSystem++;
				if (currSystem >= systems.Count)
					currSystem = 0;
				systems [currSystem].Reset ();
			}
			if (keyState.IsKeyDown (Keys.Space) && prevKeyState.IsKeyUp (Keys.Space)) {
				systems [currSystem].Active = !systems [currSystem].Active;
			}
			if (keyState.IsKeyDown (Keys.R))
				systems [currSystem].Reset ();

			var mstate = Mouse.GetState ();
			if (mstate.LeftButton == ButtonState.Pressed)
				systems [currSystem].SetState (mstate.Position.ToVector2 ());
			prevKeyState = keyState;

			if (CAPTURE && count % CAPTURE_PERIOD == 0) {
				Texture2D t = TakeScreenshot (gameTime);
				if (CROP_CAPTURE) {
					Color[] data = new Color[CAPTURE_RECTANGLE.Width * CAPTURE_RECTANGLE.Height];
					t.GetData<Color> (0, CAPTURE_RECTANGLE, data, 0, CAPTURE_RECTANGLE.Width * CAPTURE_RECTANGLE.Height);
					FileStream s = new FileStream (String.Format ("{0}/{1}.png", CAPTURE_SAVE_LOCATION, count / CAPTURE_PERIOD), FileMode.OpenOrCreate);
					Texture2D newTexture = new Texture2D (GraphicsDevice, CAPTURE_RECTANGLE.Width, CAPTURE_RECTANGLE.Height);
					newTexture.SetData (data);
					newTexture.SaveAsPng (s, newTexture.Width, newTexture.Height);
				} else {
					FileStream s = new FileStream (String.Format ("{0}/{1}.png", CAPTURE_SAVE_LOCATION, count / CAPTURE_PERIOD), FileMode.OpenOrCreate);

					t.SaveAsPng (s, t.Width, t.Height);
				}
			}
			count++;
		}



		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.White);

			spriteBatch.Begin ();


			const string copyright = "ChaosRender v\u03B5 - Christian Cosgrove";
			Vector2 dim = font.MeasureString (copyright);
			const int padding = 16;
			spriteBatch.DrawString (font, copyright, new Vector2 (VIEWPORT_WIDTH - dim.X - padding, VIEWPORT_HEIGHT - dim.Y - padding), Color.Black);

			systems[currSystem].Draw (spriteBatch, font, circle, pix);
			systems [currSystem].DrawPlot (spriteBatch, font);

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