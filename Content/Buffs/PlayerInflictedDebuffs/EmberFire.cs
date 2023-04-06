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

namespace AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs
{
    public class EmberFire : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ember Fire"); // Buff display name
            // Description.SetDefault("Oof oww hot"); // Buff description
            Main.debuff[Type] = true;  // Is it a debuff?
            //Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world

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
            npc.GetGlobalNPC<EmberFireModNPC>().EmberFireDebuff = true;
            timer++;
        }
    }

    public class EmberFireModNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool EmberFireDebuff = false;
        public float EmberFireTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<EmberFire>()))
            {
                EmberFireDebuff = false;
                EmberFireTime = 0;


                //Add sound ?!


                
            } else
            {

            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (EmberFireDebuff)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                ArmorShaderData dustShader3 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                if (EmberFireTime % 8 == 0)
                {
                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRise>(), new Color(215, 75, 50), 0.55f, 0.6f, 0f, dustShader);
                    Main.dust[p].velocity.X *= 0.55f;
                    Main.dust[p].velocity.Y = -0.85f * Math.Abs(Main.dust[p].velocity.Y);
                    Main.dust[p].noLight = true;
                }
                if (EmberFireTime % 9 == 0)
                {
                    int d = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleRiseQuadStar>(), Color.Orange, 0.45f, 0.55f, 0f, dustShader2);
                    Main.dust[d].velocity *= 0.45f;
                    Main.dust[d].noLight = true;

                }

                npc.lifeRegen -= 10;

                EmberFireTime++;
            }
        }
    }
}