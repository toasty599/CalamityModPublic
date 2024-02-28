using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SandCloud : Particle
    {
        #region Fields/Properties
        // THis should be true for things using this particle for their main visuals where them not being present would mess it up. -Toasty.
        public readonly bool IsImportant;

        public float RotationSpeed;

        public readonly float MaxScale;

        public readonly float Opacity;

        public override bool Important => IsImportant;

        public override bool SetLifetime => true;

        public override bool UseHalfTransparency => false;

        public override bool UseCustomDraw => true;

        public override string Texture => "CalamityMod/Particles/MediumMist";

        public override int FrameVariants => 3;
        #endregion

        #region Constructor
        public SandCloud(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime, float rotationSpeed, float opacity, bool isImportant)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            MaxScale = Scale = scale;
            RotationSpeed = rotationSpeed;
            Opacity = opacity;
            Lifetime = lifetime;
            IsImportant = isImportant;
            Variant = Main.rand.Next(3);
        }
        #endregion

        #region Methods
        public override void Update()
        {
            Velocity *= 0.95f;
            Rotation += RotationSpeed;
            RotationSpeed *= 0.99f;
            //Scale = MathHelper.Lerp(MaxScale, MaxScale * 0.5f, 1f - (float)Time / Lifetime);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Texture2D texture2 = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumSmoke").Value;

            Rectangle frame = new(0, 34 * Variant, 32, 34);
            float opacity = 1f - (float)Time / Lifetime;
            spriteBatch.Draw(texture2, Position - Main.screenPosition, frame, Color * opacity * Opacity * 1.075f, Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color with { A = 0 } * opacity * Opacity, Rotation, frame.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}
