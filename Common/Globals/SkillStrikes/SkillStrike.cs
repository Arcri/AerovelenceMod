using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikePlayer : ModPlayer
	{
		public float skillStrikeMultiplier = 1f;

		// When a skill strike is also a crit
		public float superCritMultiplier = 1f;

        // To activate OnSkillStrike effects
        public bool justSkillStriked = false;

        // To activate OnSuperCrit
        public bool justSuperCrit = false;

    }

    public class SkillStrikeGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool SkillStrike = false;

        // IMPORTANT NOTE: Super Crit Damage stacks on top of regular SS damage
        // Also all Super Crits are also Skill Strikes
        public float skillStrikeMultiplier = 1f;

        public float superCritMultiplier = 1f;

        // The amount of times this projectile can SkillStrike, often limited for piercing projectiles 
        public int skillStrikeAmount = 1;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (skillStrikeAmount <= 0 && !SkillStrike)
                return;

            if (SkillStrike)
            {
                modifiers.FinalDamage *= skillStrikeMultiplier * 4f;
                modifiers.CritDamage *= superCritMultiplier;
                skillStrikeAmount--;

                Main.NewText("Skill Strike Damage: penis" );
                //modifiers.HideCombatText();

            }

        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!SkillStrike || skillStrikeAmount <= 0)
                return;

            if (hit.Crit)
            {
                //Do super crit stuff
            }
            else
            {
                //Do normal skill strike stuff
            }

            base.OnHitNPC(projectile, target, hit, damageDone);
        }

    }
}