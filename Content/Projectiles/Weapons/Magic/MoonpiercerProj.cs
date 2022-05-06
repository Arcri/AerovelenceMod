using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class MoonpiercerProj : ModProjectile
    {
        public bool Strike;
        public int Damage;
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 900)
            {       
                Damage = Projectile.damage;
            }
            if (Projectile.owner == Main.myPlayer && !Strike)
            {
                if (!Main.mouseLeft && Projectile.damage == 0)
                {
                    Strike = true;
                    Projectile.rotation = (Projectile.Center - Main.MouseWorld).ToRotation();
                    Projectile.damage = Damage;
                    float MaxSpeed = 32f;
                    Vector2 VectorCursor = Main.MouseWorld - Projectile.Center;
                    float Distance = VectorCursor.Length();
                    if (Distance > MaxSpeed)
                    {
                        Distance = MaxSpeed / Distance;
                        VectorCursor *= Distance;
                    }
                    Projectile.velocity = VectorCursor;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.damage = 0;
                    Projectile.rotation = (Projectile.Center - Main.MouseWorld).ToRotation();
                    List<int> Rotato = new List<int>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].type == Projectile.type && !(Main.projectile[i].modProjectile as MoonpiercerProj).Strike)
                        {
                            Rotato.Add(i);
                        }
                    }
                    for (int i2 = 0; i2 < Rotato.Count; i2++)
                    {
                        Main.projectile[Rotato[i2]].Center = Main.player[Projectile.owner].Center + new Vector2(0f, -40f - (6f * Rotato.Count)).RotatedBy(MathHelper.ToRadians(360f / Rotato.Count * i2));
                        Main.projectile[Rotato[i2]].netUpdate = true;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
    }
}