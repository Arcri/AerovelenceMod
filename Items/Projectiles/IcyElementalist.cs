using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Projectiles
{
	public class IcyElementalist : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 6;
		}
		
        public override void SetDefaults()
        {
            projectile.width = 96;
            projectile.height = 54;  
			projectile.CloneDefaults(ProjectileID.UFOMinion);
		}
	}
}