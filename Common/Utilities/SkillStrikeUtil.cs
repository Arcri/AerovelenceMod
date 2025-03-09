using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using System.Linq;
using Terraria.Audio;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.DataStructures;

namespace AerovelenceMod.Common.Utilities
{
    public static class SkillStrikeUtil
    {
        public static void setSkillStrike(Projectile projectile, float multiplier, int timesToStrike = 1, float impactVolume = 0f, float impactScale = 0f)
        {
            Player player = Main.player[projectile.owner];

            projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().skillStrikeMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().superCritMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().superCritMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeAmount = timesToStrike;

            projectile.GetGlobalProjectile<SkillStrikeGProj>().impactVolume = impactVolume;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().impactScale = impactScale;

        }

        public static void setSkillStrikeWithImpactType(Projectile projectile, float multiplier, int timesToStrike = 1,
            SkillStrikeImpactType impactType = SkillStrikeImpactType.Basic, float impactVolume = 0f, float impactScale = 0f)
        {
            Player player = Main.player[projectile.owner];

            projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().skillStrikeMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().superCritMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().superCritMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeAmount = timesToStrike;

            projectile.GetGlobalProjectile<SkillStrikeGProj>().impactType = impactType;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().impactVolume = impactVolume;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().impactScale = impactScale;

        }

        public static void fakeSkillStrike(Player player, NPC target, Vector2 hitPosition, float multiplier = 1f, bool crit = false,
            float damage = 0f, SkillStrikeImpactType impactType = SkillStrikeImpactType.Basic, float impactVolume = 1f, float impactScale = 1f)
        {
            SkillStrikePlayer skillPlayer = player.GetModPlayer<SkillStrikePlayer>();
            float skillStrikeMultiplier = multiplier * skillPlayer.skillStrikeMultiplier;
            float superCritMultiplier = multiplier * skillPlayer.superCritMultiplier;
            float finalDamage = damage;
            if (finalDamage > 0)
            {
                if (crit)
                {
                    finalDamage *= superCritMultiplier;
                }
                else
                {
                    finalDamage *= skillStrikeMultiplier;
                }
            }

            int hitDirection = (target.Center.X > player.Center.X) ? 1 : -1;

            SkillStrikeOldNPC skillNPC = target.GetGlobalNPC<SkillStrikeOldNPC>();
            skillNPC.PendingSkillStrike = true;
            skillNPC.IsCrit = crit;
            skillNPC.DamageAmount = (int)finalDamage;
            skillNPC.HitPosition = hitPosition;
            skillNPC.ImpactScale = impactScale;
            skillNPC.ImpactType = impactType;

            skillPlayer.justSkillStriked = true;
            if (crit)
            {
                skillPlayer.justSuperCrit = true;
            }

            #region effects
            if (impactType == SkillStrikeImpactType.Basic)
            {
                for (int j = 0; j < (5 + Main.rand.Next(0, 2)) * impactScale; j++)
                {
                    Dust star = Dust.NewDustPerfect(hitPosition, ModContent.DustType<GlowPixelCross>(),
                    Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.35f, 0.5f) * 1f);

                    star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                    rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);
                }
                for (int ii = 0; ii < (6 + Main.rand.Next(0, 2)) * impactScale; ii++)
                {
                    Dust d = Dust.NewDustPerfect(hitPosition, ModContent.DustType<MuraLineBasic>(),
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.35f);
                }
            }
            else if (impactType == SkillStrikeImpactType.Pixel)
            {
                int a = Projectile.NewProjectile(null, hitPosition, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0);
                Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                Main.projectile[a].scale = impactScale;

                for (int ii = 0; ii < (6 + Main.rand.Next(0, 2)) * impactScale; ii++)
                {
                    Dust d = Dust.NewDustPerfect(hitPosition, ModContent.DustType<MuraLineBasic>(),
                            Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), Alpha: Main.rand.Next(10, 15), new Color(255, 180, 60), 0.35f);
                }
            }

            if (impactVolume > 0f)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .46f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.5f * impactVolume };
                SoundEngine.PlaySound(style, target.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_2") with { Pitch = -.26f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.25f * impactVolume };
                SoundEngine.PlaySound(style2, target.Center);
            }
            #endregion

            if (finalDamage > 0)
            {
                NPC.HitInfo hit = new NPC.HitInfo
                {
                    Damage = (int)finalDamage,
                    Knockback = 4f * target.knockBackResist,
                    HitDirection = hitDirection,
                    Crit = crit,
                    HideCombatText = true
                };

                target.StrikeNPC(hit, false, false);
            }

        }

        public enum StrikeEffectMode
        {
            A = 1,
            B = 2,
            C = 3,
        }

        public static void GenericStrikeEffect(StrikeEffectMode mode, Vector2 position, float scale = 1f, float volume = 1f)
        {
            switch (mode)
            {
                case StrikeEffectMode.A: //basic Impact
                    for (int j = 0; j < (5 + Main.rand.Next(0, 2)) * scale; j++)
                    {
                        Dust star = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.5f, 3.25f), newColor: new Color(255, 180, 60), Scale: Main.rand.NextFloat(0.35f, 0.5f) * 1f);

                        star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                        rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);
                    }
                    if (volume > 0f)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .46f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.5f * volume };
                        SoundEngine.PlaySound(style, position);
                    }
                    break;

                case StrikeEffectMode.B: //electric Impact
                    for (int j = 0; j < (6 + Main.rand.Next(0, 2)) * scale; j++)
                    {
                        Dust star = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(2f, 4f), newColor: new Color(100, 100, 255), Scale: Main.rand.NextFloat(0.4f, 0.6f) * 1f);

                        star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                        rotPower: 0.2f, preSlowPower: 0.93f, timeBeforeSlow: 12, postSlowPower: 0.91f, velToBeginShrink: 2.2f, fadePower: 0.94f, shouldFadeColor: false);
                    }
                    if (volume > 0f)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_aura_zap") with { Pitch = .2f, PitchVariance = .15f, MaxInstances = -1, Volume = 0.4f * volume };
                        SoundEngine.PlaySound(style, position);
                    }
                    break;

                case StrikeEffectMode.C: //heavy Impact
                    for (int j = 0; j < (8 + Main.rand.Next(0, 3)) * scale; j++)
                    {
                        Dust star = Dust.NewDustPerfect(position, ModContent.DustType<GlowPixelCross>(),
                        Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(1.8f, 3.5f), newColor: new Color(255, 215, 0), Scale: Main.rand.NextFloat(0.5f, 0.7f) * 1f);

                        star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                                        rotPower: 0.1f, preSlowPower: 0.9f, timeBeforeSlow: 18, postSlowPower: 0.88f, velToBeginShrink: 1.8f, fadePower: 0.92f, shouldFadeColor: false);
                    }
                    //shock wave effect
                    for (int ii = 0; ii < (8 + Main.rand.Next(0, 2)) * scale; ii++)
                    {
                        Dust d = Dust.NewDustPerfect(position, ModContent.DustType<MuraLineBasic>(),
                                Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(2.5f, 4.5f), Alpha: Main.rand.Next(10, 15), new Color(255, 215, 0), 0.45f);
                    }
                    if (volume > 0f)
                    {
                        SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_2") with { Pitch = -.3f, PitchVariance = .1f, MaxInstances = -1, Volume = 0.6f * volume };
                        SoundEngine.PlaySound(style, position);

                        SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_flamethrower_loop") with { Pitch = -.4f, PitchVariance = .05f, MaxInstances = -1, Volume = 0.25f * volume };
                        SoundEngine.PlaySound(style2, position);
                    }
                    break;
            }
        }
    }
}
