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

        //Remember to do this shit
        public bool spawnDustTargetCenter = false;

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

        public override void AI(Projectile projectile)
		{
			if (SkillStrike)
			{

                shouldHideCT = true;
                if (shouldHideCT)
                {
                    //On.Terraria.CombatText.NewText_Rectangle_Color_int_bool_bool += CombatText_NewText_Rectangle_Color_int_bool_bool;
                    //On.Terraria.CombatText.NewText_Rectangle_Color_string_bool_bool += CombatText_NewText_Rectangle_Color_string_bool_bool;
                }

                if (firstFrame)
                {
                    storedDamage = projectile.damage * 2;
                    firstFrame = false;
                    projectile.CritChance = 0;
                }

                projectile.damage = storedDamage;

                if (Main.rand.NextBool(10))
                {
                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                    int p = GlowDustHelper.DrawGlowDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<GlowCircleQuadStar>(), 0f, 0f,
                        Color.Gold, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader);
                }

            }

		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
            if (SkillStrike)
			{
                AerovelenceMod.shouldHide = true;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_wither_beast_death_1") with { Pitch = .26f, PitchVariance = .12f, MaxInstances = 1, };
                SoundEngine.PlaySound(style, target.Center);


                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int j = 0; j < 6; j++)
                {
                    Dust d = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleQuadStar>(), Vector2.One.RotatedByRandom(6) * Main.rand.NextFloat(1, 4),
                        Color.Gold, Main.rand.NextFloat(0.4f, 0.6f), 0.6f, 0f, dustShader2);
                    d.velocity *= 1f;

                }

                shouldHideCT = false;

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