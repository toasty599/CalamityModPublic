using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class RainBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 150;
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;

            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item9, projectile.position);
                projectile.localAI[0] += 1f;
            }

            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;

			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 400f, 14f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item60, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                int rain = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB));
                Main.dust[rain].noGravity = true;
                Main.dust[rain].velocity *= 4f;
            }
        }
    }
}
