using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	public class DoublePendulumEnsemble : PhysSystem
	{
		List<DoublePendulum> systems;

		const int NumSystems=100;

		PhasePlot plot1;
		PhasePlot plot2;

		public DoublePendulumEnsemble (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture)
		{
			systems = new List<DoublePendulum> ();
			Random rand = new Random ();
			plot1 = new PhasePlot (200, new Vector2(287,450), graphicsDevice);
			plot1.Title = "Phase Portrait 1";
			plot1.VerticalAxisLabel = "d\u03B8/dt";
			plot1.HorizontalAxisLabel = "\u03B8";
			plot1.MinT = -(float)Math.PI;
			plot1.MaxT = (float)Math.PI;
			plot1.MinP = -5;
			plot1.MaxP = 5;
			plot2 = new PhasePlot (200, new Vector2(537,450), graphicsDevice);
			plot2.VerticalAxisLabel = "d\u03B8/dt";
			plot2.HorizontalAxisLabel = "\u03B8";
			plot2.Title = "Phase Portrait 2";
			plot2.MinT = -(float)Math.PI*1f;
			plot2.MaxT = (float)Math.PI*1f;
			plot2.MinP = -8;
			plot2.MaxP = 8;
			for (int i = 0; i < NumSystems; i++) {
				systems.Add (new DoublePendulum (offset, graphicsDevice, circleTexture, plot1,plot2, i==0 ? Color.Red : Color.Blue));
				systems [i].SetState (new Vector2 (offset.X + 100f+5f*(float)rand.NextDouble (), 5f*(float)rand.NextDouble ()));
			}
		}

		public override void DrawPlot (SpriteBatch spriteBatch, SpriteFont font)
		{
			systems [0].DrawPlot (spriteBatch, font);
		}

		public override void Update(GameTime gameTime, float timestep)
		{
			foreach (DoublePendulum system in systems) {
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
			foreach (DoublePendulum system in systems) 
			{
				system.Draw (spriteBatch, font, nodeTexture, pix);
			}
		}

	}
}

