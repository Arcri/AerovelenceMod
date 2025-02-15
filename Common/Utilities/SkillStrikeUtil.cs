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

namespace AerovelenceMod.Common.Utilities
{
	public static class SkillStrikeUtil
	{
        public static void setSkillStrike(Projectile projectile, float multiplier, int timesToStrike = 1, float impactVolume = 0f, float impactScale = 0f)
        {
			SkillStrikeGProj ssGlobProjectile;
			if (!projectile.TryGetGlobalProjectile<SkillStrikeGProj>(out ssGlobProjectile))
				return;

			//Player player = Main.player[projectile.owner];
			SkillStrikePlayer ssPlayer;
			if (!Main.player[projectile.owner].TryGetModPlayer<SkillStrikePlayer>(out ssPlayer))
				return;

			ssGlobProjectile.SkillStrike = true;
			ssGlobProjectile.skillStrikeMultiplier = multiplier * ssPlayer.skillStrikeMultiplier;
			ssGlobProjectile.superCritMultiplier = multiplier * ssPlayer.superCritMultiplier;
			ssGlobProjectile.skillStrikeAmount = timesToStrike;

			ssGlobProjectile.impactVolume = impactVolume;
			ssGlobProjectile.impactScale = impactScale;

        }

        public static void setSkillStrikeWithImpactType(Projectile projectile, float multiplier, int timesToStrike = 1, 
            SkillStrikeImpactType impactType = SkillStrikeImpactType.Basic, float impactVolume = 0f, float impactScale = 0f)
        {
			SkillStrikeGProj ssGlobProjectile;
			if (!projectile.TryGetGlobalProjectile<SkillStrikeGProj>(out ssGlobProjectile))
				return;

			//Player player = Main.player[projectile.owner];
			SkillStrikePlayer ssPlayer;
			if (!Main.player[projectile.owner].TryGetModPlayer<SkillStrikePlayer>(out ssPlayer))
				return;

			ssGlobProjectile.SkillStrike = true;
			ssGlobProjectile.skillStrikeMultiplier = multiplier * ssPlayer.skillStrikeMultiplier;
			ssGlobProjectile.superCritMultiplier = multiplier * ssPlayer.superCritMultiplier;
			ssGlobProjectile.skillStrikeAmount = timesToStrike;

			ssGlobProjectile.impactType = impactType;
			ssGlobProjectile.impactVolume = impactVolume;
			ssGlobProjectile.impactScale = impactScale;

        }


        // TODO:
        // We are unable to do the usual process of setting Skill Strikes
        // for StrikeNPC() because it is neither a projectile or item.
        // So instead we simulate one happening here.
        public static void fakeSkillStrike()
		{

		}

		// TODO
		public static float getFakeSkillStrikeDamage(Player player, float strikeDamage)
		{
			return strikeDamage;
		}


        public enum StrikeEffectMode
        {
            A = 1,
            B = 2,
            C = 3,
        }

        public static void GenericStrikeEffect(StrikeEffectMode mode)
        {

        } 
    }
}
