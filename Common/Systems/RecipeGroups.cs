using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;

namespace AerovelenceMod.Common.Systems
{
    public class RecipeGroups : ModSystem
    {
        //TODO add more groups
        public override void AddRecipeGroups()
        {
            RecipeGroup group;

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Evil Bars",
            [
                    ItemID.DemoniteBar,
                    ItemID.CrimtaneBar
            ]);
            RecipeGroup.RegisterGroup("AerovelenceMod:EvilBars", group);

            group = new RecipeGroup(() => "Gold or Platinum",
            [
                    ItemID.GoldBar,
                    ItemID.PlatinumBar,
            ]);
            RecipeGroup.RegisterGroup("AerovelenceMod:GoldOrPlatinum", group);

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Mech Souls",
            [
                    ItemID.SoulofSight,
                    ItemID.SoulofFright,
                    ItemID.SoulofMight,
            ]);
            RecipeGroup.RegisterGroup("AerovelenceMod:MechSouls", group);
        }
    }
}