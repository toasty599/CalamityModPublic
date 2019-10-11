﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AegisBlast2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aegis Blast");
		}

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			bool flag15 = false;
			bool flag16 = false;
			if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
			{
				flag15 = true;
			}
			if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
			{
				flag15 = true;
			}
			if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
			{
				flag16 = true;
			}
			if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
			{
				flag16 = true;
			}
			if (flag15 && flag16)
			{
				projectile.Kill();
			}
			float num461 = 25f;
			if (projectile.ai[0] > 180f)
			{
				num461 -= (projectile.ai[0] - 180f) / 2f;
			}
			if (num461 <= 0f)
			{
				num461 = 0f;
				projectile.Kill();
			}
			num461 *= 0.7f;
			projectile.ai[0] += 4f;
			int num462 = 0;
			while ((float)num462 < num461)
			{
				float num463 = (float)Main.rand.Next(-5, 6);
				float num464 = (float)Main.rand.Next(-5, 6);
				float num465 = (float)Main.rand.Next(2, 5);
				float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
				num466 = num465 / num466;
				num463 *= num466;
				num464 *= num466;
				int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1.5f);
				Main.dust[num467].noGravity = true;
				Main.dust[num467].position.X = projectile.Center.X;
				Main.dust[num467].position.Y = projectile.Center.Y;
				Dust expr_149DF_cp_0 = Main.dust[num467];
				expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
				Dust expr_14A09_cp_0 = Main.dust[num467];
				expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
				Main.dust[num467].velocity.X = num463;
				Main.dust[num467].velocity.Y = num464;
				num462++;
			}
        }
    }
}
