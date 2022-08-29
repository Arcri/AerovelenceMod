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

namespace AerovelenceMod.Content.Buffs.FlareDebuffs
{
    public class FlareFire : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire debuff"); // Buff display name
            Description.SetDefault("Losing life"); // Buff description
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;

        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            if (timer % 3 == 0)
            {
                //int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), new Color(255, 75, 50), 0.4f, 0.6f, 0f, dustShader);
                //Main.dust[p].velocity *= 0.5f;
            
            }
            else if (timer % 7 == 0) //else if is intentional
            {
                //int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.Gray, 0.7f, 1f, 0f, dustShader);;
                //Main.dust[p].velocity.X *= 0.2f;
                //Main.dust[p].velocity.Y = Math.Abs(Main.dust[p].velocity.Y) * -1f;
            }
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

                    /*
                    if (Main.rand.NextBool())
                    {
                        int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), new Color(255, 75, 50), 0.4f, 0.6f, 0f, dustShader);
                        Main.dust[p].velocity *= 0.5f;
                    }
                    else
                    {
                        int d = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.OrangeRed, 0.4f, 0.6f, 0f, dustShader);
                        Main.dust[d].velocity *= 0.5f;
                    }
                    */
                }
                else if (FlareFireTime % 7 == 0) //else if is intentional
                {
                    //int a = Dust.NewDust(npc.Center, 4, 4, ModContent.DustType<ColorSmoke>());

                    //Main.dust[a].scale *= 0.5f;

                    //Main.dust[a].rotation = Main.rand.NextFloat(6.28f);
                    //Main.dust[a].velocity * = 0.4f


                    Color ColorToUse = (Main.rand.NextBool() ? Color.SlateGray * 0.75f : Color.SlateGray * 0.75f);

                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.Gray * 0.65f, Main.rand.NextFloat(0.5f, 0.9f), 1f, 0f, dustShader);
                    Main.dust[p].velocity.X *= 0.2f;
                    Main.dust[p].velocity.Y = Math.Abs(Main.dust[p].velocity.Y) * -1f;
                }

                
                /*
                if (Main.rand.NextFloat() < 0.29069766f && Main.rand.NextBool() && Main.rand.NextBool())
                {
                    //int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), Color.DarkSlateGray, 0.7f, 1f, 0f, dustShader); ;

                    Dust dust;
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 position = Main.LocalPlayer.Center;
                    dust = Main.dust[Terraria.Dust.NewDust(npc.Center + new Vector2(-20,0), 30, 30, 314, 0f, -6.976744f, 0, new Color(255, 255, 255), 0.30697676f)];
                }
                */


                //looks dumb now but will prolly need for skill crit system
                if (FlareFireTime % 30 == 0)
                {
                    Terraria.Audio.SoundStyle style = 
                        new Terraria.Audio.SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f,  PitchVariance = 0.3f, Volume = 0.5f, MaxInstances = -1} ;
                    //.Audio.SoundStyle style2 = new Terraria.Audio.SoundStyle("Terraria/Sounds/Item_20") with { Pitch = -.67f, PitchVariance = .58f, };

                    Terraria.Audio.SoundStyle? storedHitsound = npc.HitSound;

                    npc.HitSound = style;

                    npc.StrikeNPC(10, 0, 0, noEffect: true);

                    npc.HitSound = storedHitsound;


                }
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
                FlareFireTime++;
            }
        }
    }
}