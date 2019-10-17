using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class SeaPrismCrystals : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Sea Prism Crystal");
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(0, 150, 200), name);
            soundType = 2;
            soundStyle = 27;
            dustType = 67;
            drop = ModContent.ItemType<Items.PrismShard>();
            Main.tileSpelunker[Type] = true;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return CalamityWorld.downedDesertScourge;
        }

        public override bool CanExplode(int i, int j)
        {
            return CalamityWorld.downedDesertScourge;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.5f;
            b = 0.5f;
        }

        public override bool CanPlace(int i, int j)
        {
            if ((Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick() && Main.tile[i, j + 1].active()) ||
                (Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick() && Main.tile[i, j - 1].active()) ||
                (Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick() && Main.tile[i + 1, j].active()) ||
                (Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick() && Main.tile[i - 1, j].active()))
                return true;

            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
            {
                Main.tile[i, j].frameY = (short)(0 * 18);
            }
            else if (Main.tile[i, j - 1].active() && Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick())
            {
                Main.tile[i, j].frameY = (short)(1 * 18);
            }
            else if (Main.tile[i + 1, j].active() && Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick())
            {
                Main.tile[i, j].frameY = (short)(2 * 18);
            }
            else if (Main.tile[i - 1, j].active() && Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick())
            {
                Main.tile[i, j].frameY = (short)(3 * 18);
            }
            Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(18) * 18);
        }
    }
}
