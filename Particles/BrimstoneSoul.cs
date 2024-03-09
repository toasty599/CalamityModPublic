using System;
using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class BrimstoneSoul : Particle
    {
        public override bool UseCustomDraw => true;

        public override bool SetLifetime => true;

        public override int FrameVariants => 4;

        public override string Texture => "CalamityMod/Particles/BrimstoneSoul";

        public float Opacity;

        public BrimstoneSoul(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Lifetime = lifeTime;
            Variant = Main.rand.Next(0, FrameVariants);
        }

        public override void Update()
        {
            Opacity = CalamityUtils.Convert01To010(LifetimeCompletion);

            Lighting.AddLight(Position, Color.Magenta.ToVector3());
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            var texture = ModContent.Request<Texture2D>(Texture).Value;
            var frame = new Rectangle(24 * Variant, 0, 24, 30);
            for (int i = 0; i < 12; i++)
            {
                var offset = (MathHelper.TwoPi * i / 12f + Main.GlobalTimeWrappedHourly * 1f).ToRotationVector2() * 3f;
                spriteBatch.Draw(texture, Position + offset - Main.screenPosition, frame, Color with { A = 0 } * Opacity * 0.5f, Rotation, frame.Size() / 2f, Scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color * Opacity, Rotation, frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
