﻿using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Permafrost
{
    public class PopoNoseless : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Noseless Popo");
            Description.SetDefault("Your nose has been stolen!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.snowmanPrevious)
            {
                modPlayer.snowmanPower = true;
                modPlayer.snowmanNoseless = true;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
	}
}
