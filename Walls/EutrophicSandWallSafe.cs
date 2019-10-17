using Microsoft.Xna.Framework; using CalamityMod.Dusts;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Walls
{
    public class EutrophicSandWallSafe : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            dustType = 108;
            drop = ModContent.ItemType<Items.EutrophicSandWallSafe>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Sand Wall Safe");
            AddMapEntry(new Color(100, 100, 150), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
