using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneMonster : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterSpawn");
        public static readonly SoundStyle DroneSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterDrone");

        private float speedAdd = 0f;
        private float speedLimit = 0f;

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 320;
            Projectile.height = 320;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 36000;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;

            // Used to offset the VFX to prevent all 4 looking identical.
            if (Main.netMode != NetmodeID.Server)
                Projectile.localAI[1] = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(speedAdd);
            writer.Write(Projectile.localAI[0]);
            writer.Write(speedLimit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            speedAdd = reader.ReadSingle();
            Projectile.localAI[0] = reader.ReadSingle();
            speedLimit = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
                return;
            }

            int choice = (int)Projectile.ai[1];
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.soundDelay = 1125 - (choice * 225);
                SoundEngine.PlaySound(SpawnSound, Projectile.Center);
                Projectile.localAI[0] += 1f;
                switch (choice)
                {
                    case 0:
                        speedLimit = 10f;
                        break;
                    case 1:
                        speedLimit = 20f;
                        break;
                    case 2:
                        speedLimit = 30f;
                        break;
                    case 3:
                        speedLimit = 40f;
                        break;
                    default:
                        break;
                }
            }

            if (Main.rand.NextBool(15))
            {
                Vector2 position;
                Vector2 velocity;
                int lifetime;
                // Escaping the vortex.
                if (Main.rand.NextBool(3))
                {
                    position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
                    velocity = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 2.5f + Projectile.velocity;
                    lifetime = Main.rand.Next(60, 120);
                }
                // Being sucked in.
                else
                {
                    position = Projectile.Center + Main.rand.NextVector2CircularEdge(200f, 200f);
                    lifetime = Main.rand.Next(40, 70);
                    velocity = position.DirectionTo(Projectile.Center) * (position - Projectile.Center).Length() / lifetime + Projectile.velocity;

                }
                var soul = new BrimstoneSoul(position, velocity, Color.White, Main.rand.NextFloat(0.2f, 0.8f), lifetime);
                GeneralParticleHandler.SpawnParticle(soul);
            }

            if (speedAdd < speedLimit)
                speedAdd += 0.04f;

            if (Projectile.soundDelay <= 0 && (choice == 0 || choice == 2))
            {
                Projectile.soundDelay = 420;
                SoundEngine.PlaySound(DroneSound, Projectile.Center);
            }

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            Lighting.AddLight(Projectile.Center, 3f * Projectile.Opacity, 0f, 0f);

            float inertia = (revenge ? 4.5f : 5f) + speedAdd;
            float speed = (revenge ? 1.5f : 1.35f) + (speedAdd * 0.25f);
            float minDist = 160f;

            if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
            {
                inertia *= 1.5f;
                speed *= 0.5f;
            }

            if (Projectile.timeLeft < 90)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 90f, 0f, 1f);
            else
                Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 35910) / 90f), 0f, 1f);

            int target = (int)Projectile.ai[0];
            if (target >= 0 && Main.player[target].active && !Main.player[target].dead)
            {
                if (Projectile.Distance(Main.player[target].Center) > minDist)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[target].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * speed) / inertia;
                }
            }
            else
            {
                Projectile.ai[0] = Player.FindClosest(Projectile.Center, 1, 1);
                Projectile.netUpdate = true;
            }

            if (death)
                return;

            // Fly away from other brimstone monsters.
            float pushForce = 0.05f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible.
                if (!otherProj.active || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == Projectile.type;
                float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                float distancegate = Main.zenithWorld ? 360f : 320f;
                if (sameProjType && taxicabDist < distancegate)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 170f, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            if (CalamityGlobalNPC.SCal != -1 && Main.npc[CalamityGlobalNPC.SCal].active && Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneMonsterII").Value;
                lightColor.R = (byte)(255 * Projectile.Opacity);
                lightColor.B = (byte)(255 * Projectile.Opacity);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                return false;
            }

            Asset<Texture2D> invis = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj");

            var effect = Filters.Scene["CalamityMod:SoulVortexShader"].GetShader().Shader;//CalamityShaders.SoulVortexShader;
            effect.Parameters["time"]?.SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["timeOffset"]?.SetValue(Projectile.localAI[1]);
            effect.Parameters["opacity"]?.SetValue(Projectile.Opacity);
            effect.Parameters["screenSize"]?.SetValue(Main.ScreenSize.ToVector2());
            effect.Parameters["eyeNoiseColor"]?.SetValue(Color.Magenta.ToVector3());
            var baseColor = Color.Lerp(new Color(227, 79, 79), Color.Crimson, 0.8f);
            var highlightColor = Color.Lerp(new Color(250, 202, 140), Color.Crimson, 0.2f);
            effect.Parameters["edgeNoiseColor1"]?.SetValue(baseColor.ToVector3());
            effect.Parameters["edgeNoiseColor2"]?.SetValue(highlightColor.ToVector3());

            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneMonsterSouls").Value;//ModContent.Request<Texture2D>("CalamityMod/Graphics/Metaballs/GruesomeEminence_Ghost_Layer1").Value;
            Main.instance.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneMonsterEyes").Value;
            Main.instance.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/FrozenCrust").Value;
            Main.instance.GraphicsDevice.Textures[4] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise").Value;

            // These don't seem to be properly set else, so set them here.
            Main.instance.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
            Main.instance.GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
            Main.instance.GraphicsDevice.SamplerStates[4] = SamplerState.PointWrap;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(invis.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0f, invis.Size() * 0.5f, 450f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            // Draw a star in the middle of the vortex.
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D sparkTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle").Value;
            float properBloomSize = (float)sparkTexture.Height / bloomTexture.Height;
            Vector2 squish = Vector2.One * Projectile.Opacity;

            Main.spriteBatch.Draw(bloomTexture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.DarkMagenta, Color.Crimson, 0.3f) * 0.65f, 0, bloomTexture.Size() / 2f, squish * 4f * properBloomSize, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(sparkTexture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, Color.Crimson, 0.3f), Main.GlobalTimeWrappedHourly * 2f, sparkTexture.Size() / 2f, squish * MathHelper.Lerp(0.66f, 1f, (1f + MathF.Sin((float)Main.timeForVisualEffects * 0.4f)) * 0.5f), SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300, true);

            // Remove all positive buffs from the player if they're hit by HAGE while Cirrus is alive.
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int buffType = target.buffType[l];
                            if (target.buffTime[l] > 0 && CalamityLists.amalgamBuffList.Contains(buffType))
                            {
                                target.DelBuff(l);
                                l--;
                            }
                        }
                    }
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
            behindNPCs.Add(index);
        }
    }
}
