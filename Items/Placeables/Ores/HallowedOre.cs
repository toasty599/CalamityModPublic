﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Ores
{
    public class HallowedOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Hallowed Ore");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Ores.HallowedOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            Recipe r = Mod.CreateRecipe(ItemID.HallowedBar);
            r.AddIngredient<HallowedOre>(4).AddTile(TileID.AdamantiteForge).Register();
        }
    }
}
