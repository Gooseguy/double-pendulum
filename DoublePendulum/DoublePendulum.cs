using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoublePendulum
{
	public class DoublePendulum : PhysSystem
	{
		public Vector2 Offset { get; set; }
		public float t1, t2;
		float p1, p2;
		float m1, m2;
		float g;
		float l1, l2;

		Color color;

		PhasePlot plot1, plot2;

		BallSprite ball1, ball2;
		public DoublePendulum (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture, PhasePlot _plot1,PhasePlot _plot2, Color _color)
		{
			Reset ();
			Offset = offset;
			plot1 = _plot1;
			plot2 = _plot2;
			ball1 = new BallSprite (circleTexture, "1");
			ball2 = new BallSprite (circleTexture, "2");
			color = _color;
		}


		public DoublePendulum (Vector2 offset, GraphicsDevice graphicsDevice, Texture2D circleTexture, Color _color)
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
			plot2 = new PhasePlot (200, new Vector2(537,450), graphicsDevice);
			plot2.VerticalAxisLabel = "d\u03B8/dt";
			plot2.HorizontalAxisLabel = "\u03B8";
			plot2.Title = "Phase Portrait 2";
			plot2.MinT = -(float)Math.PI*1f;
			plot2.MaxT = (float)Math.PI*1f;
			plot2.MinP = -8;
			plot2.MaxP = 8;

			ball1 = new BallSprite (circleTexture, "1");
			ball2 = new BallSprite (circleTexture, "2");
			color = _color;
		}

		public override float GetEnergy ()
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

		public override void Update (GameTime gameTime, float timestep)
		{
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

			plot1.Update (gameTime, ref t1, ref p1);
			plot2.Update (gameTime, ref t2, ref p2);
			if (Active) {
				t1 += timestep * dt1;
				t2 += timestep * dt2;
				p1 += timestep * dp1;
				p2 += timestep * dp2;
			}

		}

		public override void Draw (SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix)
		{
			const int thickness = 4;

			Vector2 pos1 = Offset + Scale*l1 * new Vector2 ((float)Math.Sin (t1), (float)Math.Cos (t1));
			Vector2 pos2 = pos1 + Scale*l2 * new Vector2 ((float)Math.Sin (t2), (float)Math.Cos (t2));

			spriteBatch.Draw (pix, null, new Rectangle ((int)pos1.X, (int)pos1.Y-thickness/2, (int)((pos2 - pos1).Length ()), thickness), null, null, 
				(float)Math.Atan2 (pos2.Y - pos1.Y, pos2.X - pos1.X), null, color, SpriteEffects.None, 0);
			spriteBatch.Draw (pix, null, new Rectangle ((int)Offset.X, (int)Offset.Y-thickness/2, (int)((pos1-Offset).Length ()), thickness), null, null, 
				(float)Math.Atan2 (pos1.Y-Offset.Y, pos1.X-Offset.X), null, color, SpriteEffects.None, 0);

			ball1.Position = pos1;
			ball2.Position = pos2;
			ball1.Draw (spriteBatch, font, color);
			ball2.Draw (spriteBatch, font, color);
		}

		public override void DrawPlot (SpriteBatch spriteBatch, SpriteFont font)
		{
			plot1.Draw (spriteBatch, font);
			plot2.Draw (spriteBatch, font);
		}

		public override void SetState (Vector2 mousePosition)
		{
			if (!plot1.ContainsMouse (mousePosition) && !plot2.ContainsMouse (mousePosition)) {
				Vector2 pos = mousePosition - Offset;
				t1 = (float)Math.Atan2 (pos.X, pos.Y);
				p1 = 0;
			}
		}


		public override void SetState (float t, float p)
		{
			t1 = t; p1 = p;
		}

		public override void Reset ()
		{
			t1 = (float)Math.PI / 3 + 0.01f;
			t2 = 0;
			p1 = 0;
			p2 = 0;
			m1 = 1;
			m2 = 1;
			g = 1.0f;
			l1 = 1;
			l2=1;
		}
	}
}

