using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Plaguenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguenade");
            Tooltip.SetDefault("Releases a swarm of angry plague bees\n" +
			"Stealth strikes spawn more bees and generate a larger explosion");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 60;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1.5f;
            item.maxStack = 999;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 28;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<PlaguenadeProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Beenade, 15);
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 3);
            recipe.AddIngredient(ItemID.Obsidian, 2);
            recipe.AddIngredient(ItemID.Stinger);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
