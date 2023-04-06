using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.FlashLight
{
    public class BeaconShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Beacon Shot");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
        }

        int timer = 0;
        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (timer < 30)
            {
                Projectile.velocity *= 1.12f;
            } 
            else if (timer > 30)
            {
                if (FindNearestNPC(300f, false, false, true, out int index))
                {
                    NPC npc = Main.npc[index];
                    Projectile.velocity *= .9f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * 50f, .018f);
                    //For some reason, directionTo*length seems to slow down sometimes :shrug:
                }

            }

            timer++;
        }

        //I should really just put this in a helper class... Another thing to eventually do during cleanup 
        private bool FindNearestNPC(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
        {
            npcIndex = -1;
            bool foundNPC = false;
            double dist = range * range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                //Make sure NPC is valid anyway
                if (npc.active && npc.life > 0)
                {
                    //Target and NPC friendliness are same
                    if (npc.friendly == targetIsFriendly)
                    {
                        //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                        if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                        {
                            //cache this
                            float compDist = Projectile.DistanceSQ(npc.Center);
                            //Distance is shorter than current distance, but did not overflow (underflow)
                            if (compDist < dist && compDist > 0)
                            {
                                //ignore tiles, OR scan tiles and can hit anyway
                                if (!scanTiles || (scanTiles && Collision.CanHit(Projectile, new NPCAimedTarget(npc))))
                                {
                                    npcIndex = i;
                                    dist = compDist;
                                    foundNPC = true;
                                }
                            }
                        }
                    }
                }
            }
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D WhiteShot = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/BeaconShotWhite").Value;
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Starlight");


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (true)
                    {
                        float sclae = MathHelper.Clamp(1.25f - (j * 0.2f), 0, 4);
                        Vector2 scalee = new Vector2(sclae * 1.25f, sclae) * 0.6f;
                        Main.spriteBatch.Draw(texture, Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition, null, Color.Orange * 0.75f, Projectile.rotation, texture.Size() / 2, scalee, SpriteEffects.None, 0);
                    }                 
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            /*
            Vector2 centerOffset = new Vector2(Projectile.width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length - 1; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                Main.spriteBatch.Draw(texture, drawPos + centerOffset, null, Color.Orange, Projectile.oldRot[k], WhiteShot.Size() / 2, MathHelper.Clamp((Projectile.scale * 0.65f) - k / (float)Projectile.oldPos.Length, 0, 2), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(texture, drawPos - Projectile.oldPos[k], null, Color.White * 2f, Projectile.oldRot[k], drawOrigin, (Projectile.scale  * 1f) - k / (float)Projectile.oldPos.Length, effects, 0f);
            }
            */
            /*
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (i != 0)
                {
                    float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                    Vector2 centerOffset = new Vector2(Projectile.width / 2, Projectile.height / 2);
                    Main.spriteBatch.Draw(WhiteShot, Projectile.oldPos[i] - Main.screenPosition + centerOffset, WhiteShot.Frame(1, 1, 0, 0), Color.OrangeRed,
                        Projectile.rotation, WhiteShot.Size() / 2f, new Vector2(1f, 0.8f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.1f);
                }
            }
            */

            Texture2D ShotProj = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Misc/Magic/FlashLight/BeaconShot").Value;
            SpriteEffects mySpriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.spriteBatch.Draw(ShotProj, Projectile.Center - Main.screenPosition, ShotProj.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, ShotProj.Size() / 2, Projectile.scale, mySpriteEffects, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {

            ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 6; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
                Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleDust>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.3f, 0.5f, 0f, dustShader2);
                gd.fadeIn = 2;
            }
        }
    }
}
