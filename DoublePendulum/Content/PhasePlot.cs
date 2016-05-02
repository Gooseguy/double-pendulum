using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	/// <summary>
	/// Visualization of phase portraits
	/// </summary>
	public class PhasePlot
	{
		Texture2D texture;
		readonly int Resolution = 200;
		public float MinT { get; set; }
		public float MaxT { get; set; }
		public float MinP { get; set; }
		public float MaxP { get; set; }

		public string Title { get; set; }
		public string HorizontalAxisLabel { get; set; }
		public string VerticalAxisLabel { get; set; }

		public int FadePeriod { get; set; }

		public float FadeMultiplier { get; set; }

		public Vector2 Position { get; set; }
		private int count=0;

		public PhasePlot (int resolution, Vector2 position, GraphicsDevice graphicsDevice)
		{
			Resolution = resolution;
			Position = position;
			Title = "";
			FadeMultiplier = 0.95f;
			FadePeriod = 100;
			texture = new Texture2D (graphicsDevice, resolution, resolution);
			Color[] data = new Color[resolution * resolution];
			for (int i = 0; i < resolution * resolution; i++)
				data [i] = new Color (Color.White, 0);
			texture.SetData<Color> (data);
		}

		/// <summary>
		/// Update the plot.
		/// </summary>
		/// <param name="t">Current value of canonical position</param>
		/// <param name="p">Current value of canonical momentum</param>
		public void Update(GameTime gameTime, float t, float p)
		{
			//
			setPhasePortrait(t,p);
			if (count % FadePeriod == 0)
				fadePhasePortrait ();
			count++;
		}

		void setPhasePortrait(float t, float p)
		{

			int rT = (int)(Resolution * (t-MinT) / (MaxT - MinT));
			int rP = (int)(Resolution * (p-MinP) / (MaxP - MinP));

			texture.SetData (0, new Rectangle (rT, rP, 1, 1), new Color[] { Color.Black }, 0, 1);
		}

		void fadePhasePortrait()
		{
			Color[] data = new Color[Resolution * Resolution];
			texture.GetData<Color> (data);

			for (int i = 0; i < Resolution * Resolution; i++)
				data [i] = data [i] * FadeMultiplier;

			texture.SetData<Color>(data);
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			spriteBatch.Draw (texture, Position, null, null, null, 0, null, Color.White, SpriteEffects.None, 0);

			Vector2 size = font.MeasureString (Title);

			spriteBatch.DrawString (font, Title, Position + new Vector2 ((Resolution - size.X) / 2, -size.Y), Color.Black);

			Vector2 size2 = font.MeasureString (HorizontalAxisLabel);
			spriteBatch.DrawString (font, HorizontalAxisLabel, Position + new Vector2 ((Resolution - size2.X), -size2.Y), Color.Black);

			Vector2 size3 = font.MeasureString (VerticalAxisLabel);
			spriteBatch.DrawString (font, VerticalAxisLabel, Position + new Vector2 (-size3.Y, (Resolution + size3.X)/2), Color.Black, 
				-(float)Math.PI/2, Vector2.Zero,1.0f,SpriteEffects.None,0);
			
		}
	}
}

