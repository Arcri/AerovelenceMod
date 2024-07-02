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
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;

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


        //TODO PLAN:
        //Every frame check if justSkillStriked is true and if it is do accessory effects and such
        //^ Maybe make this an array or something to account for multiple strikes in a frame and store info about them like weapon class

        //Maybe add a vector 2 to store point of skill strike impact 
    }

    public enum SkillStrikeImpactType
    {
        Basic = 0, 
        Pixel = 1,
        PlaceHolder = 2,
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

        //VFX amount
        public float impactScale = 0f;

        //Impact Sound
        public float impactVolume = 0f;

        //Type of Impact VFX
        public SkillStrikeImpactType impactType = SkillStrikeImpactType.Basic;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!SkillStrike)
                return;

            if (skillStrikeAmount <= 0)
                SkillStrike = false;

            if (SkillStrike)
            {
                modifiers.FinalDamage *= skillStrikeMultiplier * 1f; //1f
                //modifiers.CritDamage *= superCritMultiplier;
                skillStrikeAmount--;

                if (skillStrikeAmount >= 0)
                    modifiers.HideCombatText();
            }

        }

        //Currently only supports hit effects origination from projectile position, TODO add option for taget position
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // < 0 and not <= 0 because skillStrikeAmount is decremented in ModifyHitNPC which runs before OnHitNPC
            if (!SkillStrike || skillStrikeAmount < 0)
            {
                SkillStrike = false;
                return;
            }

            if (hit.Crit)
            {
                //Do super crit stuff
            }
            else
            {
                
                //Do normal skill strike stuff
            }

            if (impactType == SkillStrikeImpactType.Basic)
            {
                for (int j = 0; j < (5 + Main.rand.Next(0, 2)) * impactScale; j++)
                {
                    Dust star = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<GlowPixelCross>(),
                    Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.35f, 0.5f) * 1f);

                    star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                    rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);
                }
                for (int ii = 0; ii < (6 + Main.rand.Next(0, 2)) * impactScale; ii++)
                {
                    Dust d = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<MuraLineBasic>(),
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.35f);
                }
            }
            else if (impactType == SkillStrikeImpactType.Pixel)
            {
                int a = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0);
                Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                Main.projectile[a].scale = impactScale;

                for (int ii = 0; ii < (6 + Main.rand.Next(0, 2)) * impactScale; ii++)
                {
                    Dust d = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<MuraLineBasic>(),
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.35f);
                }
            }

            //Hit Sound
            if (impactVolume > 0f)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .46f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.5f * impactVolume };
                SoundEngine.PlaySound(style, target.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_2") with { Pitch = -.26f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.25f * impactVolume };
                SoundEngine.PlaySound(style2, target.Center);
            }

            //Damage number
            Vector2 randomSpawnPos = Main.rand.NextVector2FromRectangle(new Rectangle((int)target.position.X, (int)target.position.Y, target.width, (int)(target.height * 0.75f)));
            Dust text = Dust.NewDustPerfect(randomSpawnPos, ModContent.DustType<SkillStrikeText>(), new Vector2(0f, -12f), Scale: 1f);

            SkillStrikeTextBehavior sstb = new SkillStrikeTextBehavior();
            sstb.isCrit = false;
            sstb.damageNumber = "" + hit.Damage;

            text.customData = sstb;

            base.OnHitNPC(projectile, target, hit, damageDone);
        }

    }
}