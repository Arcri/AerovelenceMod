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
    public class AuroraFire : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aurora Fire"); // Buff display name
            Description.SetDefault("Toasty!"); // Buff description
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
            npc.GetGlobalNPC<AuroraFireModNPC>().AuroraFireDebuff = true;
            timer++;
        }
    }

    public class AuroraFireModNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool AuroraFireDebuff = false;
        public float AuroraFireTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<AuroraFire>()))
            {
                AuroraFireDebuff = false;
                AuroraFireTime = 0;


                //Add sound !!

                /* moved to Projectile
                //Explode dust on first debuff
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                for (int i = 0; i < 20; i++)
                {
                    float randomR = Main.rand.NextFloat(0, 256);
                    float randomG = Main.rand.NextFloat(0, 256);
                    float randomB = Main.rand.NextFloat(0, 256);
                    Color randomColor = new Color(randomR, randomG, randomB);

                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleQuadStar>(), randomColor, 0.3f, 0.65f, 0f, dustShader);
                }
                */
            } else
            {
                //Think this works
                //AuroraFireTime = 0f;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (AuroraFireDebuff)
            {
                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                float randomR = 20f;// Main.rand.NextFloat(0, 256);
                float randomG = 55f; //Main.rand.NextFloat(0, 256);
                float randomB = 200f;// Main.rand.NextFloat(0, 256);
                Color randomColor = new Color(randomR, randomG, randomB);

                Color c = new Color(

                (byte)Main.rand.Next(0, 255),

                (byte)Main.rand.Next(0, 255),

                (byte)Main.rand.Next(0, 255));

                if (AuroraFireTime % 4 == 0)
                {
                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowCircleDust>(), c, 0.5f, 0.8f, 0f, dustShader);
                    Main.dust[p].velocity *= 0.5f;
                    Main.dust[p].noLight = true;

                }
                else if (AuroraFireTime % 7 == 0) //else if is intentional
                {

                    int p = GlowDustHelper.DrawGlowDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowSpark>(), c, 0.1f, 1f, 0f, dustShader2);
                    Main.dust[p].noLight = true;
                    //Main.dust[p].velocity.X *= 0.2f;
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
                npc.lifeRegen -= 20;

                AuroraFireTime++;
            }
        }
    }
}