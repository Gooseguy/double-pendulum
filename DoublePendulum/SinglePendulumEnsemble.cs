using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	public class SinglePendulumEnsemble : PhysSystem
	{
		List<SinglePendulum> systems;

		const int NumSystems=500;

		PhasePlot plot;

		public SinglePendulumEnsemble (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture, Texture2D sampleTex)
		{
			systems = new List<SinglePendulum> ();
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
				systems.Add (new SinglePendulum (offset, graphicsDevice, circleTexture, plot));

				float x = -1, y = -1;
				Color[] pix = new Color[1];
				while (x == -1 || y == -1 || pix [0].B > 128) {
					x = (float)rand.NextDouble ();
					y = (float)rand.NextDouble ();
					sampleTex.GetData<Color> (0, new Rectangle ((int)(x * sampleTex.Width), (int)(y * sampleTex.Height), 1, 1), pix, 0, 1);
				}

				systems [i].SetState (1f+0.6f*x, 1.0f*y);
			}
		}

		public override void DrawPlot (SpriteBatch spriteBatch, SpriteFont font)
		{
			systems [0].DrawPlot (spriteBatch, font);
		}

		public override void Update(GameTime gameTime, float timestep)
		{
			foreach (SinglePendulum system in systems) {
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

		public override void SetState (float t, float p)
		{
			throw new NotImplementedException ();
		}


		public override void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix)
		{
			foreach (SinglePendulum system in systems) 
			{
				system.Draw (spriteBatch, font, nodeTexture, pix);
			}
		}

	}
}

