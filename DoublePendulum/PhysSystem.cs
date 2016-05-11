using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoublePendulum
{
	public abstract class PhysSystem
	{

		public bool Active=true;
		public float Scale = 150f;
		public PhysSystem ()
		{
		}

		public abstract void Update(GameTime time, float timestep);

		public abstract float GetEnergy ();	

		public abstract void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix);

		public abstract void SetState(Vector2 mousePosition);

		public abstract void Reset ();

		public abstract void DrawPlot(SpriteBatch spriteBatch, SpriteFont font);
	}
}

