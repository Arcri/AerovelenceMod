using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using AerovelenceMod.Content.Dusts;
using static Terraria.NPC;

namespace AerovelenceMod.Content.Buffs.FlareDebuffs
{
    public class FlareFire : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FlareFireModNPC>().FlareFireDebuff = true;
            timer++;
        }
    }

    public class FlareFireModNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool FlareFireDebuff = false;
        public float FlareFireTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<FlareFire>()))
            {
                FlareFireDebuff = false;
                FlareFireTime = 0;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (FlareFireDebuff)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                if (FlareFireTime % 3 == 0)
                {
                    if (Main.rand.NextBool())
                    {
                        int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), new Color(255, 75, 50), 0.4f, 0.65f, 0f, dustShader);
                        Main.dust[p].velocity *= 0.5f;
                    }
                    else
                    {
                        int d = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.OrangeRed, 0.4f, 0.65f, 0f, dustShader);
                        Main.dust[d].velocity *= 0.5f;
                    }
                }
                else if (FlareFireTime % 7 == 0) //else if is intentional
                {
                    Color ColorToUse = (Main.rand.NextBool() ? Color.SlateGray * 0.75f : Color.SlateGray * 0.75f);

                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader);
                    Main.dust[p].velocity.X *= 0.2f;
                    Main.dust[p].velocity.Y = Math.Abs(Main.dust[p].velocity.Y) * -1f;
                }


                //looks dumb now but will prolly need for skill crit system
                if (FlareFireTime % 30 == 0)
                {
                    Terraria.Audio.SoundStyle style = 
                        new Terraria.Audio.SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f,  PitchVariance = 0.3f, Volume = 0.5f, MaxInstances = -1} ;

                    Terraria.Audio.SoundStyle? storedHitsound = npc.HitSound;

                    npc.HitSound = style;

                    HitInfo myHit = new HitInfo();
                    myHit.Damage = 10;
                    myHit.HideCombatText = true;

                    npc.HitSound = storedHitsound;
                }
                FlareFireTime++;
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (FlareFireDebuff && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
            {
                projectile.damage += 5;
            }
        }
    }
}