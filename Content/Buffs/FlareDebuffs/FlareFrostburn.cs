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
using AerovelenceMod.Content.Items.Weapons.Flares;
using Terraria.Audio;

namespace AerovelenceMod.Content.Buffs.FlareDebuffs
{
    public class FlareFrostburn : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Fire"); // Buff display name
            Description.SetDefault("So cold it burns!"); // Buff description
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FlareFrostburnModNPC>().DebuffActive = true;
            timer++;
        }
    }

    public class FlareFrostburnModNPC : BaseFlareDebuffNPC
    {
        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            if (!DebuffActive)
            {
                DebuffIndex = ModContent.BuffType<FlareFrostburn>();
                DebuffTime = 0;
                baseResetEffects(npc);
            }
            if (!npc.HasBuff(ModContent.BuffType<FlareFire>()))
            {
                //DebuffIndex = ModContent.BuffType<FlareFrostburn>();
                //baseResetEffects(npc);
            }
            baseResetEffects(npc);

        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (DebuffActive)
            {
                timeBetweenHits = 30;
                tickDamage = 3;
                sound = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, PitchVariance = 0.3f, Volume = 0.5f, MaxInstances = -1 };
                colorA = Color.DodgerBlue;
                colorB = Color.Teal;
                DebuffIndex = ModContent.BuffType<FlareFrostburn>();
                baseUpdateLifeRegen(npc, ref damage);
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (DebuffActive)
            {
                DebuffIndex = ModContent.BuffType<FlareFrostburn>();
                timeBetweenHits = 30;
                tickDamage = 3;
                sound = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_1") with { Pitch = -.53f, PitchVariance = 0.3f, Volume = 0.5f, MaxInstances = -1 };
                colorA = Color.DeepSkyBlue;
                colorB = Color.SkyBlue;
                tagDamage = 3;
                tagCrit = 4;
                baseModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            }

        }

    }
}