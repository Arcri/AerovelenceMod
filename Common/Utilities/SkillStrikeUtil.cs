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
		public static void setSkillStrike(Projectile projectile, float multiplier, int timesToStrike = 1)
		{
			Player player = Main.player[projectile.owner];

            projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = true;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().skillStrikeMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().superCritMultiplier = multiplier * player.GetModPlayer<SkillStrikePlayer>().superCritMultiplier;
            projectile.GetGlobalProjectile<SkillStrikeGProj>().skillStrikeAmount = timesToStrike;
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
