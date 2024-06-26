using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class CyverBotBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Bot");
            // Description.SetDefault("Cyver bots will fight for you");

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //int projType = ModContent.ProjectileType<TrojanForceBot>();
            //if (player.ownedProjectileCounts[projType] < 1)
            //{
            //    var source = Projectile.InheritSource(player);
            //    var pos = player.Center;
            //    Projectile.NewProjectile(source, pos, Vector2.Zero, projType, 100, player.HeldItem.knockBack, player.whoAmI);
            //}
        }
    }
}
