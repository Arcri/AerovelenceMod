using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.Audio;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    //MOSTLY DONE
    public class CyvercryIOU : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = ItemUseStyleID.None;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine IOU = new(Mod, "IOU", "Cyvercry's drops have not been implemented yet, but they hopefully will be soon!")
            {
                OverrideColor = Color.HotPink,
            };
            tooltips.Add(IOU);
        }

    }

}