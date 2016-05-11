using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	public class HarmonicOscillatorEnsemble : PhysSystem
	{
		List<HarmonicOscillator> systems;

		const int NumSystems=5;

		PhasePlot plot;

		public HarmonicOscillatorEnsemble (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture)
		{
			systems = new List<HarmonicOscillator> ();
			Random rand = new Random ();
			plot = new PhasePlot (200, new Vector2 (100, 200), graphicsDevice);
			plot.Title = "Phase Portrait 1";
			plot.VerticalAxisLabel = "d\u03B8/dt";
			plot.HorizontalAxisLabel = "\u03B8";
			plot.MinT = -(float)Math.PI;
			plot.MaxT = (float)Math.PI;
			plot.MinP = -5;
			plot.MaxP = 5;
			for (int i = 0; i < NumSystems; i++) {
				systems.Add (new HarmonicOscillator (offset, graphicsDevice, circleTexture, plot));
				systems [i].SetState (new Vector2 (offset.X + 100f+50f*(float)rand.NextDouble (), 0));
			}
		}

		public override void DrawPlot (SpriteBatch spriteBatch, SpriteFont font)
		{
			systems [0].DrawPlot (spriteBatch, font);
		}

		public override void Update(GameTime gameTime, float timestep)
		{
			foreach (HarmonicOscillator system in systems) {
				system.Update (gameTime, timestep);
			}
		}

		public override void Reset ()
		{
			
		}

		public override float GetEnergy ()
		{
			throw new NotImplementedException ();
		}

		public override void SetState (Vector2 mousePosition)
		{
			throw new NotImplementedException ();
		}


		public override void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix)
		{
			foreach (HarmonicOscillator system in systems) 
			{
				system.Draw (spriteBatch, font, nodeTexture, pix);
			}
		}

	}
}

