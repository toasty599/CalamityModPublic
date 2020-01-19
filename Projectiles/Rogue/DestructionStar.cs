using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class DestructionStar : ModProjectile
    {
		public int hitCount = 0;
        private static float Radius = 43f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star of Destruction");
        }

        public override void SetDefaults()
        {
            projectile.width = 86;
            projectile.height = 86;
            projectile.friendly = true;
            projectile.penetrate = 16;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.Next(8) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 191, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.rotation += Math.Sign(projectile.velocity.X) * MathHelper.ToRadians(8f);
			if (projectile.Calamity().stealthStrike == true || hitCount > 16)
				hitCount = 16;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            hitCount++;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            hitCount++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= Radius;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
			Vector2 vector2 = new Vector2(20f, 20f);
			for (int index1 = 0; index1 < 10; ++index1)
			{
				int index2 = Dust.NewDust(projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
				Dust dust = Main.dust[index2];
				dust.velocity = dust.velocity * 1.4f;
			}
            if (projectile.owner == Main.myPlayer)
            {
				int i;
				for (i = 0; i < hitCount; i++)
				{
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<DestructionBolt>(), (int)((double)projectile.damage * 0.5), 0f, Main.myPlayer, 0f, 0f);
                }
			}
		}
    }
}
