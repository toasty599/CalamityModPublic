using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    public class EutrophicChair : ModTile
    {
        public override void SetDefaults()
        {
            CalamityUtils.SetUpChair(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Chair");
            AddMapEntry(new Color(191, 142, 111), name);
            disableSmartCursor = true;
            adjTiles = new int[] { TileID.Chairs };
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 51, 0f, 0f, 1, new Color(54, 69, 72), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.EutrophicChair>());
        }
    }
}
