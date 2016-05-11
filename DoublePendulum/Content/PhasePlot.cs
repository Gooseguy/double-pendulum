using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DoublePendulum
{
	/// <summary>
	/// Visualization of phase portraits
	/// </summary>
	public class PhasePlot
	{
		Texture2D texture;
		Texture2D grid;
		readonly int Resolution = 200;
		public float MinT { get; set; }
		public float MaxT { get; set; }
		public float MinP { get; set; }
		public float MaxP { get; set; }
		public float CurrT { get; set; }
		public float CurrP { get; set; }
		float zoom = 1f;
		const float MinZoom = 0.1f;
		const float MaxZoom = 10f;
		public string Title { get; set; }
		public string HorizontalAxisLabel { get; set; }
		public string VerticalAxisLabel { get; set; }

		public int FadePeriod { get; set; }

		const float gridIntervalT = (float)Math.PI/16f;
		const float gridIntervalP = 0.05f;

		Color gridColor = Color.LightGray;

		public float FadeMultiplier { get; set; }

		public Vector2 Position { get; set; }
		private int count=0;

		public Rectangle BoundingRectangle { get { return new Rectangle ((int)Position.X, (int)Position.Y, Resolution, Resolution); } }

		public bool ContainsMouse(Vector2 position) {
			return BoundingRectangle.Contains (position);
		}

		bool axisTickLabels = true;


		MouseState prevMouseState;

		public PhasePlot (int resolution, Vector2 position, GraphicsDevice graphicsDevice)
		{
			Resolution = resolution;
			Position = position;
			Title = "";
			FadeMultiplier = 0.95f;
			FadePeriod = 100;
			texture = new Texture2D (graphicsDevice, resolution, resolution);
			grid = new Texture2D (graphicsDevice, resolution, resolution);
			Color[] data = new Color[resolution * resolution];
			for (int i = 0; i < resolution * resolution; i++)
				data [i] = new Color (Color.White*0f, 0);
			texture.SetData<Color> (data);

			generateGrid ();
		}

		/// <summary>
		/// Update the plot.
		/// </summary>
		/// <param name="t">Current value of canonical position</param>
		/// <param name="p">Current value of canonical momentum</param>
		public void Update(GameTime gameTime, ref float t, ref float p)
		{
			//
			setPhasePortrait(t,p);
			if (count % FadePeriod == 0)
				fadePhasePortrait ();
			count++;

			CurrT = t;
			CurrP = p;
			handleInput ();
			t = CurrT;
			p = CurrP;
		}

		void handleInput()
		{
			MouseState mstate = Mouse.GetState ();

			if (BoundingRectangle.Contains (Mouse.GetState ().Position)) {

				if (mstate.LeftButton == ButtonState.Pressed) {
					Vector2 position = Mouse.GetState ().Position.ToVector2 ();
					position -= Position;
					position *= new Vector2 (MaxT - MinT, MaxP - MinP);
					position /= Resolution;
					position += new Vector2 (MinT, MinP);
					position /= zoom;
					if (Keyboard.GetState ().IsKeyDown (Keys.LeftShift))
						position.Y = 0;
					CurrT = position.X;
					CurrP = position.Y;


				}

				int scrollChange = mstate.ScrollWheelValue - prevMouseState.ScrollWheelValue;
				zoom += 0.0001f * scrollChange;
				zoom = MathHelper.Clamp (zoom, MinZoom, MaxZoom);
				if (scrollChange != 0) { 
					clearPhasePortrait ();
					generateGrid ();
				}
			}
			
			prevMouseState = mstate;
		}

		void setPhasePortrait(float t, float p)
		{

			int rT = (int)(Resolution * (t*zoom-MinT) / (MaxT - MinT));
			int rP = (int)(Resolution * (p*zoom-MinP) / (MaxP - MinP));

			texture.SetData (0, new Rectangle (rT, rP, 1, 1), new Color[] { Color.Black }, 0, 1);
		}

		

		void generateGrid()
		{
			Color[] data = new Color[Resolution * Resolution];
			int dT = (int)(gridIntervalT*zoom * Resolution);
			int dP = (int)(gridIntervalP*zoom * Resolution);
			for (int i = 0; i < Resolution; i++)
				for (int j = 0; j < Resolution; j++) {
					if ((i-Resolution/2) % dT == 0 || (j-Resolution/2)%dP==0)
						data [i + j * Resolution] = gridColor;
					else data [i + j * Resolution] = Color.White;
				}

			grid.SetData<Color> (data);
		}

		void fadePhasePortrait()
		{
			Color[] data = new Color[Resolution * Resolution];
			texture.GetData<Color> (data);

			for (int i = 0; i < Resolution * Resolution; i++)
				data [i] = data [i] * FadeMultiplier;

			texture.SetData<Color>(data);
		}

		void clearPhasePortrait()
		{
			Color[] data = new Color[Resolution * Resolution];
			texture.GetData<Color> (data);

			for (int i = 0; i < Resolution * Resolution; i++)
				data [i] = Color.White*0f;

			texture.SetData<Color>(data);
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			spriteBatch.Draw (grid, Position, null, null, null, 0, null, Color.White, SpriteEffects.None, 0);
			spriteBatch.Draw (texture, Position, null, null, null, 0, null, Color.White, SpriteEffects.None, 0);

			Vector2 size = font.MeasureString (Title);

			spriteBatch.DrawString (font, Title, Position + new Vector2 ((Resolution - size.X) / 2, -2*size.Y), Color.Black);

			Vector2 size2 = font.MeasureString (HorizontalAxisLabel);
			spriteBatch.DrawString (font, HorizontalAxisLabel, Position + new Vector2 ((Resolution - size2.X), -size2.Y), Color.Black);

			Vector2 size3 = font.MeasureString (VerticalAxisLabel);
			spriteBatch.DrawString (font, VerticalAxisLabel, Position + new Vector2 (-size3.Y, (Resolution + size3.X)/2), Color.Black, 
				-(float)Math.PI/2, Vector2.Zero,1.0f,SpriteEffects.None,0);
			string zoomLabel = String.Format ("{0}%", (int)(zoom * 100));
			Vector2 size4 = font.MeasureString (zoomLabel);

			spriteBatch.DrawString (font, zoomLabel, Position + new Vector2 ((Resolution - size4.X), Resolution-size4.Y), Color.Black);
		}
	}
}

