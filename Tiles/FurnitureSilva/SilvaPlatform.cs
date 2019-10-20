using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Dusts.Furniture;
using Terraria.ID;

namespace CalamityMod.Tiles.FurnitureSilva
{
    public class SilvaPlatform : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpPlatform(Type);
            AddMapEntry(new Color(191, 142, 111));
            drop = ModContent.ItemType<Items.Placeables.FurnitureSilva.SilvaPlatform>();
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Platforms };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<SilvaTileGold>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 157, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
