using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class TotalityBreakers : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Totality Breakers");
            Tooltip.SetDefault(@"Explodes into highly flammable black tar
Tar oils enemies and sets them alight
Stealth strikes leak tar as they fly");
        }

        public override void SafeSetDefaults()
        {
            item.width = 30;
            item.damage = 47;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.height = 40;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<TotalityFlask>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MolotovCocktail, 50);
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ConsecratedWater>());
            //recipe.AddIngredient(ModContent.ItemType<DesecratedWater>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}
