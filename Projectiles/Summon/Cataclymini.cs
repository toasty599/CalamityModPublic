using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Cataclymini : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cataclymini");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 20;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (int)CalamityDusts.Brimstone, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            float num633 = Calamitamini.Range;
            float num634 = Calamitamini.SeparationAnxietyMin;
            float num635 = Calamitamini.SeparationAnxietyMax;
            float num636 = Calamitamini.SafeDist;
            bool flag64 = projectile.type == ModContent.ProjectileType<Cataclymini>();
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.cEyes = false;
                }
                if (modPlayer.cEyes)
                {
                    projectile.timeLeft = 2;
                }
            }
			projectile.MinionAntiClump();
            bool flag24 = false;
            if (flag24)
            {
                return;
            }
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (projectile.ai[0] != 1f)
            {
                projectile.tileCollide = false;
            }
            if (projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16)))
            {
                projectile.tileCollide = false;
            }
            Vector2 value = new Vector2(0.5f);
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    Vector2 vector2 = npc.position + npc.Size * value;
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if ((!flag25 && num646 < num633) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC npc = Main.npc[num645];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
						Vector2 vector2 = npc.position + npc.Size * value;
                        float num646 = Vector2.Distance(npc.Center, projectile.Center);
                        if ((!flag25 && num646 < num633) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            num633 = num646;
                            vector46 = vector2;
                            flag25 = true;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 8f; //6
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[0] == 1f;
                }
                float num650 = 6f;
                if (flag26)
                {
                    num650 = 18f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 10f)
                {
                    num650 = 10f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num651 > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num651 > 70f)
                {
                    vector48.Normalize();
                    vector48 *= num650;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.25f;
                    projectile.velocity.Y = -0.15f;
                }
            }
            if (flag25)
            {
                projectile.rotation = (vector46 - projectile.Center).ToRotation() + 3.14159274f;
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + 3.14159274f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 30);
            }
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float scaleFactor3 = 8f;
                int num658 = ModContent.ProjectileType<BrimstoneFireSummon>();
                if (flag25 && projectile.ai[1] == 0f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, vector46, 0, 0))
                    {
                        Vector2 value19 = vector46 - projectile.Center;
                        value19.Normalize();
                        value19 *= scaleFactor3;
                        int num659 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value19.X, value19.Y, num658, projectile.damage, 0f, projectile.owner, 0f, 0f);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
