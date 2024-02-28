using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class DustProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public ref float Timer => ref Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 30 * Projectile.MaxUpdates; // 30 effectively, 120 total
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13 * Projectile.MaxUpdates; // 13 effective, 52 total
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.4f, 0.01f);

            if (!Main.dedServ)
                DoDustVFX();

            Projectile.velocity *= 0.995f;
            Projectile.rotation += 0.3f * Projectile.direction;

            // Fade out.
            if (Projectile.timeLeft < 5)
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, (float)Projectile.timeLeft / 5f);

            Timer++;
        }

        public void DoDustVFX()
        {
            // Closer to death = less intense.
            float lifetimeScalar = MathHelper.Clamp(Projectile.timeLeft / 80f, 0f, 1f);
            for (int i = 0; i < 2; i++)
            {
                Color sandColor = Color.Lerp(Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat(0.2f, 0.8f)), Color.DimGray, Main.rand.NextFloat(0f, 0.3f));
                var sandCloud = new SandCloud(Projectile.Center + Main.rand.NextVector2Circular(30f, 30f), Projectile.velocity * Main.rand.NextFloat(0.7f, 0.9f),
                    sandColor, Main.rand.NextFloat(1.3f, 2.3f) * lifetimeScalar, Main.rand.Next(7, 15), Main.rand.NextFloat(-0.05f, 0.05f), 0.13f,true);

                GeneralParticleHandler.SpawnParticle(sandCloud);
            }
            if (Timer % 15f == 0f)
            {
                for (int i = 0; i < 2; i++)
                {
                    var sandDust = new SandyDustParticle(Projectile.Center + Main.rand.NextVector2Circular(23f, 23f), Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) *
                        Main.rand.NextFloat(1.3f, 2f), Color.White, Main.rand.NextFloat(0.4f, 1.4f) * MathHelper.Clamp(lifetimeScalar * 2f, 0f, 1f), Main.rand.Next(20, 50));
                    GeneralParticleHandler.SpawnParticle(sandDust);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            OnHitFX(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            OnHitFX(target);
        }

        public void OnHitFX(Entity target)
        {
            Projectile.velocity *= 0.45f;

            // Custom sound would be nicer but this suffices.
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Pitch = -0.6f, PitchVariance = 1f }, target.Center);
            Owner.Calamity().GeneralScreenShakePower = 1f;

            // Closer to death = less intense.
            float lifetimeScalar = MathHelper.Clamp(Projectile.timeLeft / 80f, 0f, 1f);

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity;
                if (i <= 2)
                    velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(-7.3f, -2.5f);
                else
                    velocity = Vector2.UnitY.RotatedBy(Main.rand.NextFloat(MathF.Tau)) * Main.rand.NextFloat(-4.3f, -2.5f);

                Particle rock = new StoneDebrisParticle(Main.rand.NextVector2FromRectangle(target.Hitbox), velocity * MathHelper.Lerp(0.5f, 1f, lifetimeScalar),
                    Color.SandyBrown, Main.rand.NextFloat(0.6f, 1.2f) * MathHelper.Lerp(0.2f, 1f, lifetimeScalar), 45, Main.rand.NextFloat(-0.05f, 0.05f));
                GeneralParticleHandler.SpawnParticle(rock);
            }

            for (int i = 0; i < 13; i++)
            {
                Vector2 sandPosition = Main.rand.NextVector2FromRectangle(target.Hitbox);
                Color sandColor = Color.Lerp(Color.SandyBrown, Color.SaddleBrown, Main.rand.NextFloat(0.2f, 0.8f));
                var sandCloud = new SandCloud(sandPosition, Vector2.UnitY.RotatedBy(Main.rand.NextFloat(MathF.Tau)) * Main.rand.NextFloat(1.3f, 3.9f),
                    sandColor, Main.rand.NextFloat(1.3f, 2.3f) * MathHelper.Lerp(0.25f, 0.8f, lifetimeScalar), Main.rand.Next(20, 30), Main.rand.NextFloat(-0.05f, 0.05f), 0.6f * lifetimeScalar, true);

                GeneralParticleHandler.SpawnParticle(sandCloud);
            }
        }

        public override bool? CanDamage() => Projectile.timeLeft > 5;

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
