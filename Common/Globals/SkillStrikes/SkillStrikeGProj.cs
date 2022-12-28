using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikeGProj : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		public bool SkillStrike = false;

		public bool firstFrame = true;

		public int storedDamage = 0;

        public bool shouldHideCT = false;

        public int travelDust = 0;
        public enum TravelDustType
        {
            None = 0,
            glowProjCenter = 1,
            glowPlayerCenter = 2,
            pixelProjCenter = 3,
            pixelPlayerCenter = 4
        }

        //7 is not implemented yet
        public int critImpact = 0;
        public enum CritImpactType
        {
            None = 0,
            glowProjCenter = 1,
            glowPlayerCenter = 2,
            glowTargetCenter = 3,
            pixelProjCenter = 4,
            pixelPlayerCenter = 5,
            pixelTargetCenter = 6,
            pixelTargetCenterSticky = 7,
            
        }

        public float hitSoundVolume = 1f;
        public float impactScale = 0.75f;
        public float impactRot = -1f;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            /*
            if (SkillStrike)
            {
                On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool -= baseCombatText_NewText_Rectangle_Color_int_bool_bool;
                On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool -= baseCombatText_NewText_Rectangle_Color_string_bool_bool;

                On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool += CombatText_NewText_Rectangle_Color_int_bool_bool;
                On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
            } else
            {
                On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool -= CombatText_NewText_Rectangle_Color_int_bool_bool;
                On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool -= CombatText_NewText_Rectangle_Color_string_bool_bool;

                On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool += baseCombatText_NewText_Rectangle_Color_int_bool_bool;
                On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool += baseCombatText_NewText_Rectangle_Color_string_bool_bool;
            }
            */
        }
        public override void Load()
        {
            //base.Load();
        }

        int dustTimer = 0;

        
        public override void AI(Projectile projectile)
		{
			if (SkillStrike)
			{
                /*
                shouldHideCT = true;
                if (shouldHideCT)
                {
                    //On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool += CombatText_NewText_Rectangle_Color_int_bool_bool;
                    //On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
                }
                */

                if (firstFrame)
                {
                    storedDamage = (int)(projectile.damage * 1.3f);
                    firstFrame = false;
                    //projectile.CritChance = 0;
                }

                projectile.damage = storedDamage;

                switch (travelDust)
                {
                    case (int)TravelDustType.None:
                        break;

                    case (int)TravelDustType.glowProjCenter:
                        if (dustTimer % 20 == 0)
                        {
                            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                            int p = GlowDustHelper.DrawGlowDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<GlowCircleQuadStar>(), 0f, 0f,
                                Color.Gold, Main.rand.NextFloat(0.3f, 0.5f), 0.3f, 0f, dustShader);
                        }
                        break;

                    case (int)TravelDustType.glowPlayerCenter:
                        if (dustTimer % 20 == 0)
                        {
                            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                            int p = GlowDustHelper.DrawGlowDust(Main.player[projectile.owner].position, projectile.width, projectile.height, ModContent.DustType<GlowCircleQuadStar>(), 0f, 0f,
                                Color.Gold, Main.rand.NextFloat(0.3f, 0.5f), 0.3f, 0f, dustShader);
                        }
                        break;
                    case (int)TravelDustType.pixelProjCenter:

                        break;
                    case (int)TravelDustType.pixelPlayerCenter:
                        //TDB
                        break;
                }



            }

            dustTimer++;

		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
            if (SkillStrike)
			{
                //AerovelenceMod.shouldHide = true;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .46f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.5f * hitSoundVolume };
                SoundEngine.PlaySound(style, target.Center);

                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_2") with { Pitch = -.26f, PitchVariance = .12f, MaxInstances = -1, Volume = 0.25f * hitSoundVolume };
                SoundEngine.PlaySound(style2, target.Center);

                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                float pixelHitRotation = (impactRot == -1 ? Main.rand.NextFloat(6.28f) : impactRot);

                Color colToUse = crit ? Color.DeepPink * 0.75f : Color.Orange;

                switch (critImpact)
                {
                    case (int)CritImpactType.glowProjCenter:
                        for (int j = 0; j < 8 * impactScale; j++)
                        {
                            Dust dust1 = GlowDustHelper.DrawGlowDustPerfect(projectile.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                                colToUse, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader2);
                            dust1.velocity *= 1f;

                        }
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(projectile.Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 42 + Main.rand.NextFloat(-3f, 4f);
                        }
                        break;

                    case (int)CritImpactType.glowPlayerCenter:
                        for (int j = 0; j < 8 * impactScale; j++)
                        {
                            Dust dust2 = GlowDustHelper.DrawGlowDustPerfect(Main.player[projectile.owner].Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                                colToUse, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader2);
                            dust2.velocity *= 1f;

                        }
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Main.player[projectile.owner].Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 42 + Main.rand.NextFloat(-3f, 4f);
                        }
                        break;

                    case (int)CritImpactType.glowTargetCenter:
                        for (int j = 0; j < 8 * impactScale; j++)
                        {
                            Dust dust3 = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                                colToUse, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader2);
                            dust3.velocity *= 1f;

                        }
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(target.Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f,1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 42 + Main.rand.NextFloat(-3f,4f);
                        }
                        break;

                    case (int)CritImpactType.pixelProjCenter:
                        int a = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0, Main.myPlayer);
                        Main.projectile[a].rotation = pixelHitRotation;
                        Main.projectile[a].scale = impactScale;
                        for (int i = 0; i < 5 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(projectile.Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.25f, 0.2f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);

                        }
                        break;

                    case (int)CritImpactType.pixelPlayerCenter:
                        int b = Projectile.NewProjectile(null, Main.player[projectile.owner].Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0, Main.myPlayer);
                        Main.projectile[b].rotation = pixelHitRotation;
                        Main.projectile[b].scale = impactScale;
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(Main.player[projectile.owner].Center + randomStart, ModContent.DustType<GlowLine1>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);
                        }
                        break;

                    case (int)CritImpactType.pixelTargetCenter:
                        int c = Projectile.NewProjectile(null, target.Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0, Main.myPlayer);
                        Main.projectile[c].rotation = pixelHitRotation;
                        Main.projectile[c].scale = impactScale;
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(target.Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);
                        }
                        break;

                    case (int)CritImpactType.pixelTargetCenterSticky:
                        int d = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0, Main.myPlayer);
                        Main.projectile[d].rotation = pixelHitRotation;
                        Main.projectile[d].scale = impactScale;
                        if (Main.projectile[d].ModProjectile is SkillCritImpact impact)
                        {
                            impact.sticky = true;
                            impact.stuckNPCIndex = target.whoAmI;
                        }
                        for (int i = 0; i < 12 * impactScale; i++)
                        {
                            Vector2 randomStart = Main.rand.NextVector2CircularEdge(6.5f, 6.5f) * impactScale;
                            Dust gd = GlowDustHelper.DrawGlowDustPerfect(projectile.Center + randomStart, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), colToUse, 0.2f, 0.4f, 0f, dustShader2);
                            gd.fadeIn = 50 + Main.rand.NextFloat(-3f, 4f);
                        }
                        break;
                }

                /*
                for (int j = 0; j < 6; j++)
                {
                    Dust d = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                        Color.Gold, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader2);
                    d.velocity *= 1f;

                }
                */

                //int a = Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkillCritImpact>(), 0, 0, Main.myPlayer);
                //Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
                //shouldHideCT = false;

                /*
                //Combat text is done before OnHitNPC is called, so we iterate through every text in reverse to find the latest one that is true and turn it off
                //Luckily the max amount of combat text is 100, so this is not horribly slow
                for (int i = Main.maxCombatText - 1; i >= 0; i--)
                {
                    Main.NewText(i);
                    if (Main.combatText[i].active)
                    {
                        Main.combatText[i].active = false;
                        break;
                    }
                }
                */
                //Main.NewText(Main.combatText[0].ToString());

                //Main.NewText(Main.maxCombatText);
                //Main.NewText(Main.combatText[1].active);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (SkillStrike)
            {
                shouldHideCT = false;
            }
        }

        #region combatText begone
        /*
        private int CombatText_NewText_Rectangle_Color_string_bool_bool(On.Terraria.CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Microsoft.Xna.Framework.Rectangle location, Microsoft.Xna.Framework.Color color, string text, bool dramatic, bool dot)
        {
            return orig(location, Color.Purple * 1f, text, dramatic, dot);
        }

        private int CombatText_NewText_Rectangle_Color_int_bool_bool(On.Terraria.CombatText.orig_NewText_Rectangle_Color_int_bool_bool orig, Microsoft.Xna.Framework.Rectangle location, Microsoft.Xna.Framework.Color color, int amount, bool dramatic, bool dot)
        {
            return CombatText.NewText(location, Color.Purple * 1f, amount.ToString(), dramatic, dot);
        }

        private int baseCombatText_NewText_Rectangle_Color_string_bool_bool(On.Terraria.CombatText.orig_NewText_Rectangle_Color_string_bool_bool orig, Microsoft.Xna.Framework.Rectangle location, Microsoft.Xna.Framework.Color color, string text, bool dramatic, bool dot)
        {
            return orig(location, Color.Blue * 1f, text, dramatic, dot);
        }

        private int baseCombatText_NewText_Rectangle_Color_int_bool_bool(On.Terraria.CombatText.orig_NewText_Rectangle_Color_int_bool_bool orig, Microsoft.Xna.Framework.Rectangle location, Microsoft.Xna.Framework.Color color, int amount, bool dramatic, bool dot)
        {
            return CombatText.NewText(location, Color.Blue * 1f, amount.ToString(), dramatic, dot);
        }
        */
        #endregion

    }
}