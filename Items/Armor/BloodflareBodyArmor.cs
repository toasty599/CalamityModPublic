﻿using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BloodflareBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Body Armor");
            Tooltip.SetDefault("12% increased damage and 8% increased critical strike chance\n" +
                       "You regenerate life quickly and gain +30 defense while in lava\n" +
                       "+40 max life");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.defense = 35;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.Calamity().AllCritBoost(8);
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodstoneCore>(16)
                .AddIngredient<RuinousSoul>(4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
