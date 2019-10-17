﻿using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    
    public class Kylie : CalamityDamageItem
    {
        public static float Speed = 11f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
            Tooltip.SetDefault("Throws three short ranged boomerangs if stealth is full\n" + "Also known as Dowak");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 70;
            item.knockBack = 12;
            item.thrown = true;
            item.crit = 16;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.useTime = 25;
            item.useAnimation = 25;
            item.width = 32;
            item.height = 46;
            item.useStyle = 1;
            item.UseSound = SoundID.Item1;
            item.shootSpeed = Speed;
            item.shoot = mod.ProjectileType("KylieBoomerang");
            item.noMelee = true;
            item.noUseGraphic = true;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityPlayer p = Main.player[Main.myPlayer].Calamity();
            //If stealth is full, shoot a spread of 3 boomerangs with reduced range
            if (p.StealthStrikeAvailable())
            {
                int spread = 10;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 perturbedspeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(spread));
                    int proj = Projectile.NewProjectile(position.X, position.Y, perturbedspeed.X, perturbedspeed.Y, mod.ProjectileType("KylieBoomerang"), item.damage, item.knockBack, player.whoAmI, 0f, 1f);
                    Main.projectile[proj].Calamity().stealthStrike = true;
                    spread -= 10;
                }
                return false;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            //If you have more than one boomerang, you cant shoot another
            int MAX = 1;
            int launched = 0;
            foreach (Projectile projectile in Main.projectile)
                if (projectile.type == item.shoot && projectile.owner == item.owner && projectile.active)
                {
                    launched++;
                }
            return (launched >= MAX) ? false : true;
        }
    }
}
