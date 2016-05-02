using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoublePendulum
{
	public class BallSprite
	{
		public Vector2 Position;
		Texture2D texture;

		public string Label;

		public BallSprite (Texture2D texture, string label = "")
		{
			this.texture = texture;
			Label = label;
		}

		public void Draw(SpriteBatch spriteBatch, SpriteFont font)
		{
			spriteBatch.Draw (texture, Position, null, null, new Vector2(texture.Width/2, texture.Height/2),0,null, Color.Black, SpriteEffects.None,0);
			spriteBatch.Draw (texture, Position, null, null, new Vector2(texture.Width/2, texture.Height/2),0,0.9f * Vector2.One, Color.White, SpriteEffects.None,0);

			Vector2 size = font.MeasureString (Label);
			spriteBatch.DrawString (font, Label, Position - size*0.5f, Color.Black);
		}
	}
}

