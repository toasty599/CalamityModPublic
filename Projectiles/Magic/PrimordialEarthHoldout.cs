using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PrimordialEarthHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public ref float Timer => ref Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];

        public override string Texture => "CalamityMod/Items/Weapons/Magic/PrimordialEarth";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 7200;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            // Die if no longer holding the click button or otherwise cannot use the item.
            if (!Owner.channel || Owner.dead || !Owner.active || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.MountedCenter;
            AdjustPlayerValues();

            Item heldItem = Owner.ActiveItem();
            Projectile.damage = Owner.GetWeaponDamage(heldItem);

            if (Main.myPlayer == Projectile.owner && Timer % heldItem.useTime == heldItem.useTime - 1f && Owner.CheckMana(heldItem.mana, true))
            {

                Vector2 rockSpawnPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * heldItem.width * 0.5f;
                Vector2 rockVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * heldItem.shootSpeed;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), rockSpawnPosition, rockVelocity, ModContent.ProjectileType<PrimordialEarthDust>(), Projectile.damage, heldItem.knockBack, Projectile.owner, ai1: Main.rand.Next(0, 4));
            }

            Timer++;
        }

        public void AdjustPlayerValues()
        {
            Projectile.timeLeft = 2;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.direction * Projectile.velocity).ToRotation();

            // Aim towards the mouse.
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitX * Owner.direction);
                if (Projectile.velocity != Projectile.oldVelocity)
                    Projectile.netUpdate = true;
                Projectile.spriteDirection = (Projectile.velocity.X > 0f).ToDirectionInt();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathF.PI;
            Owner.ChangeDir(Projectile.spriteDirection);

            Projectile.Center += Projectile.velocity * 20f;

            // Update the player's arm directions to make it look as though they're holding the book.
            float frontArmRotation = Projectile.rotation + Owner.direction * -0.4f;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation);
        }

        public override bool? CanDamage() => false;

        public override bool ShouldUpdatePosition() => false;
    }
}
