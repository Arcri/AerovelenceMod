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
	public class SkillStrikeProj : ModProjectile
	{
        int combatTextIndex = 0;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
            Projectile.hide = true;
            Projectile.scale = 1.1f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            combatTextIndex = (int)Projectile.ai[1];
            CombatText anchor = Main.combatText[combatTextIndex];

            //Main.NewText("My anchor is: " + combatTextIndex);

            if (!anchor.active)
                Projectile.active = false;

            Projectile.scale = anchor.scale * 1.1f;
            Projectile.Center = anchor.position + new Vector2(0,10);
            
        }


    }
}