﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class Viperfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Viperfish");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 75;
            npc.width = 72;
            npc.height = 30;
            npc.defense = 15;
            npc.lifeMax = 320;
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<CrushDepth>()] = true;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.85f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ViperfishBanner>();
            npc.chaseable = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            CalamityAI.PassiveSwimmingAI(npc, mod, 0, Main.player[npc.target].Calamity().GetAbyssAggro(600f, 200f), 0.25f, 0.15f, 6f, 6f, 0.1f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return npc.chaseable;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += npc.chaseable ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/ViperfishGlow").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/ViperfishGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/ViperfishGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 120, true);
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 120, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 90);
                player.AddBuff(ModContent.BuffType<Horror>(), 90, true);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer1 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer2 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(1000000) && CalamityWorld.revenge)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HalibutCannon>());
            }
            if (CalamityWorld.downedCalamitas)
            {
                if (Main.rand.NextBool(4))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Lumenite>());
                }
                if (Main.rand.NextBool(2))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DepthCells>(), Main.rand.Next(1, 3));
                }
                if (Main.expertMode && Main.rand.NextBool(2))
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DepthCells>());
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ViperFish"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ViperFish2"), 1f);
            }
        }
    }
}
