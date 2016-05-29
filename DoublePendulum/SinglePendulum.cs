using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	public class SinglePendulum : PhysSystem
	{
		public Vector2 Offset { get; set; }
		float t1;
		float p1;
		float m1;
		float g;
		float l1;

		PhasePlot plot1;

		BallSprite ball1;

		public SinglePendulum (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture)
		{
			Reset ();
			Offset = offset;
			plot1 = new PhasePlot (200, new Vector2(287,450), graphicsDevice);
			plot1.Title = "Phase Portrait 1";
			plot1.VerticalAxisLabel = "d\u03B8/dt";
			plot1.HorizontalAxisLabel = "\u03B8";
			plot1.MinT = -(float)Math.PI;
			plot1.MaxT = (float)Math.PI;
			plot1.MinP = -5;
			plot1.MaxP = 5;


			ball1 = new BallSprite (circleTexture, "1");
		}

		public override float GetEnergy ()
		{
			float e = 0;

			//kinetic
			e += m1 * l1 * l1 * p1*p1*0.5f;

			//potential

			e -= (float)Math.Cos (t1) * m1 * l1*g;

			return e;
		}

		public override void Update (GameTime gameTime, float timestep)
		{


			float dt = p1 / (m1 * l1 * l1);
			if (Active) {
				p1 += timestep * -m1 * g * l1 * (float)Math.Sin (t1);
				t1 += timestep * dt;
			}

			plot1.Update (gameTime, ref t1, ref p1);
		}

		public override void Draw (SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix)
		{
			const int thickness = 4;

			Vector2 pos1 = Offset + Scale*l1 * new Vector2 ((float)Math.Sin (t1), (float)Math.Cos (t1));
//			Vector2 pos2 = pos1 + scale*l2 * new Vector2 ((float)Math.Sin (t2), (float)Math.Cos (t2));

			spriteBatch.Draw (pix, null, new Rectangle ((int)Offset.X, (int)Offset.Y-thickness/2, (int)((pos1-Offset).Length ()), thickness), null, null, 
				(float)Math.Atan2 (pos1.Y-Offset.Y, pos1.X-Offset.X), null, Color.Black, SpriteEffects.None, 0);

			ball1.Position = pos1;
			ball1.Draw (spriteBatch, font, Color.Black);

		}
		public override void DrawPlot (SpriteBatch spriteBatch, SpriteFont font)
		{
			plot1.Draw (spriteBatch, font);
		}

		public override void SetState (Vector2 mousePosition)
		{
			if (!plot1.ContainsMouse(mousePosition))
			{
				Vector2 pos = mousePosition - Offset;
				t1 = (float)Math.Atan2 (pos.X, pos.Y);
				p1 = 0;
			}
		}
		public override void Reset ()
		{
			t1 = (float)Math.PI / 16;
			p1 = -0.5f;
			m1 = 1;
			g = 1.0f;
			l1 = 1;
		}
	}
}

