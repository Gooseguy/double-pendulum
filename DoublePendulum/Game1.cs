#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Threading;

#endregion

namespace DoublePendulum
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;

		SpriteBatch spriteBatch;

		float t1 = (float)Math.PI/8, t2 = (float)Math.PI/8;
		float p1 = 0, p2 = 2f;
		float m1 = 1, m2 = 1;
		float g = 1.0f;
		float l1 = 1, l2=1f;
		float timestep = 0.005f;
		float scale = 100;
		Vector2 offset = new Vector2(150,100);

		int count=0;
		float fadeMultiplier=0.96f;

		const float minT1 = -(float)Math.PI;
		const float maxT1 = (float)Math.PI;
		const float minP1 = -5;
		const float maxP1 = 5;
		const float minT2 = -(float)Math.PI*15f;
		const float maxT2 = (float)Math.PI*15f;
		const float minP2 = -8;
		const float maxP2 = 8;

		Texture2D phasePortrait1, phasePortrait2;
		const int phaseresolution = 200;

		float energyAverage = 0;

		Vector2 phasePortrait1Position = new Vector2 (300, 200);
		Vector2 phasePortrait2Position = new Vector2 (500, 200);

		SpriteFont font;

		Texture2D circle;

		Texture2D pix;

		Texture2D thetaTex1, dThetaTex1,thetaTex2, dThetaTex2;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
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

			font = Content.Load<SpriteFont> ("MainFontLarge");

			pix = new Texture2D (graphics.GraphicsDevice, 1, 1);
			pix.SetData<Color> (new Color[]{ Color.White });

			phasePortrait1 = new Texture2D (graphics.GraphicsDevice, phaseresolution, phaseresolution);
			phasePortrait2 = new Texture2D (graphics.GraphicsDevice, phaseresolution, phaseresolution);
			Color[] data = new Color[phaseresolution * phaseresolution];
			for (int i = 0; i < phaseresolution * phaseresolution; i++)
				data [i] = new Color (Color.White, 0);
			phasePortrait1.SetData<Color> (data);
			phasePortrait2.SetData<Color> (data);
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
				float cosdiff = (float)Math.Cos (t1 - t2);
				float sindiff = (float)Math.Sin (t1 - t2);
				float sin2diff = (float)Math.Sin (2 * (t1 - t2));
				float sin1 = (float)Math.Sin (t1);
				float sin2 = (float)Math.Sin (t2);

				float c1 = p1 * p2 * sindiff / (
				              l1 * l2 * (m1 + m2 * sindiff * sindiff));
				float c2 = sin2diff * (l2 * l2 * m2 * p1 * p1 +
				          l2 * l2 * (m1 + m2) * p2 * p2 -
				          l1 * l2 * m2 * p1 * p2 * cosdiff) /
				          ((2 * l1 * l1 * l2 * l2 * (m1 + m2 * sindiff * sindiff)) *
				          (2 * l1 * l1 * l2 * l2 * (m1 + m2 * sindiff * sindiff)));

				float dt1 = (l2 * p1 - l1 * p2 * cosdiff) / (
				               l1 * l1 * l2 * (m1 + m2 * sindiff * sindiff));
				float dt2 = (l1 * (m1 + m2) * p2 - l2 * m2 * p1 * cosdiff) /
				           (l1 * l2 * l2 * m2 * (m1 + m2 * sindiff * sindiff));
				float dp1 = -(m1 + m2) * g * l1 * sin1 - c1 + c2;
				float dp2 = -m2 * g * l2 * sin2 + c1 - c2;

				t1 += timestep * dt1;
				t2 += timestep * dt2;
				p1 += timestep * dp1;
				p2 += timestep * dp2;

//				p1 *= 0.9f;
//				p2 *= 0.9f;
				setPhasePortrait1 ();
				setPhasePortrait2 ();

				if (count % 100 == 0) {
					fadePhasePortrait1 ();
					fadePhasePortrait2 ();
				}
			}
			count++;
			energyAverage += calculateEnergy ();
			Console.WriteLine ("Avg energy: {0}", energyAverage / count);
			base.Update (gameTime);
		}
		
		void setPhasePortrait1()
		{
			int rT = (int)(phaseresolution * (t1-minT1) / (maxT1 - minT1));
			int rP = (int)(phaseresolution * (p1-minP1) / (maxP1 - minP1));

			phasePortrait1.SetData (0, new Rectangle (rT, rP, 1, 1), new Color[] { Color.Black }, 0, 1);
		}
		void setPhasePortrait2()
		{
			int rT = (int)(phaseresolution * (t2-minT2) / (maxT2 - minT2));
			int rP = (int)(phaseresolution * (p2-minP2) / (maxP2 - minP2));

			phasePortrait2.SetData (0, new Rectangle (rT, rP, 1, 1), new Color[] { Color.Black }, 0, 1);
//			phasePortrait2.SetData (0, new Rectangle (count%phaseresolution, (int)(calculateEnergy()*phaseresolution/10), 1, 1), new Color[] { Color.Red }, 0, 1);
		}

		void fadePhasePortrait1()
		{
			Color[] data = new Color[phaseresolution * phaseresolution];
			phasePortrait1.GetData<Color> (data);

			for (int i = 0; i < phaseresolution * phaseresolution; i++)
				data [i] = data [i] * fadeMultiplier;

			phasePortrait1.SetData<Color>(data);
		}
		void fadePhasePortrait2()
		{
			Color[] data = new Color[phaseresolution * phaseresolution];
			phasePortrait2.GetData<Color> (data);

			for (int i = 0; i < phaseresolution * phaseresolution; i++)
				data [i] = data [i] * fadeMultiplier;

			phasePortrait2.SetData<Color>(data);
		}

		float calculateEnergy()
		{
			float e = 0;

			//kinetic
			e += m1 * l1 * l1 * p1*p1*0.5f;
			e += m2 * l2 * l2 * p2*p2*0.5f;

			//potential

			e -= (float)Math.Cos (t1) * m1 * l1*g;
			e -= (float)(Math.Cos (t2)*l2 + Math.Cos (t1)*l1) * m2 * g;

			return e;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.White);

			const int thickness = 4;
			spriteBatch.Begin ();

			Vector2 pos1 = offset + scale*l1 * new Vector2 ((float)Math.Sin (t1), (float)Math.Cos (t1));
			Vector2 pos2 = pos1 + scale*l2 * new Vector2 ((float)Math.Sin (t2), (float)Math.Cos (t2));
			spriteBatch.Draw (circle, pos1, null, null, new Vector2(circle.Width/2, circle.Height/2),0,null, Color.White, SpriteEffects.None,0);
			spriteBatch.Draw (circle, pos2,null, null, new Vector2(circle.Width/2, circle.Height/2),0,null, Color.White, SpriteEffects.None,0);
			spriteBatch.Draw (pix, null, new Rectangle ((int)pos1.X, (int)pos1.Y-thickness/2, (int)((pos2 - pos1).Length ()), thickness), null, null, 
				(float)Math.Atan2 (pos2.Y - pos1.Y, pos2.X - pos1.X), null, Color.Black, SpriteEffects.None, 0);
			spriteBatch.Draw (pix, null, new Rectangle ((int)offset.X, (int)offset.Y-thickness/2, (int)((pos1-offset).Length ()), thickness), null, null, 
				(float)Math.Atan2 (pos1.Y-offset.Y, pos1.X-offset.X), null, Color.Black, SpriteEffects.None, 0);
			
			spriteBatch.Draw (phasePortrait1, new Rectangle((int)phasePortrait1Position.X,(int)phasePortrait1Position.Y,phaseresolution,phaseresolution), Color.White);
			spriteBatch.Draw (phasePortrait2, new Rectangle((int)phasePortrait2Position.X,(int)phasePortrait2Position.Y,phaseresolution,phaseresolution), Color.White);
			spriteBatch.Draw (thetaTex1, phasePortrait1Position + new Vector2(-20,phaseresolution/2), Color.White);
			spriteBatch.Draw (dThetaTex1, phasePortrait1Position + new Vector2(phaseresolution/2,-20), Color.White);
			spriteBatch.Draw (thetaTex2, phasePortrait2Position + new Vector2(-20,phaseresolution/2), Color.White);
			spriteBatch.Draw (dThetaTex2, phasePortrait2Position + new Vector2(phaseresolution/2,-20), Color.White);



			spriteBatch.DrawString (font, "Phase Portrait 1", phasePortrait1Position + new Vector2(phaseresolution/2 - font.MeasureString("Phase Portrait 1").X/2,-80), Color.Black);
			spriteBatch.DrawString (font, "Phase Portrait 2", phasePortrait2Position + new Vector2(phaseresolution/2 - font.MeasureString("Phase Portrait 2").X/2,-80), Color.Black);

			spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}