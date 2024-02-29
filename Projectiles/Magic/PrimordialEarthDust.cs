using System;
using System.Collections.Generic;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PrimordialEarthDust : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public ref float Timer => ref Projectile.ai[0];

        public int Variant => (int)Projectile.ai[1];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; 

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
        }

        public override void AI()
        {
            if (!Main.dedServ)
                DoDustVFX();

            // Fade out.
            if (Projectile.timeLeft < 5)
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 5f);

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
                    sandColor, Main.rand.NextFloat(1.3f, 2.3f) * lifetimeScalar, Main.rand.Next(7, 15), Main.rand.NextFloat(-0.05f, 0.05f), 0.13f, true);

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
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(target.Hitbox), Vector2.Zero,
                    ModContent.ProjectileType<PrimordialEarthCloud>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, 0, Variant, 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
