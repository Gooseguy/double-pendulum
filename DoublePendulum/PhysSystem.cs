using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoublePendulum
{
	public abstract class PhysSystem
	{
		public PhysSystem ()
		{
		}

		public abstract void Update(GameTime time, float timestep);

		public abstract float GetEnergy ();	

		public abstract void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D nodeTexture, Texture2D pix);

		public abstract void SetState(Vector2 mousePosition);
	}
}

