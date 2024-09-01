using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Common.Utilities.StructureStamper
{
    public class StructureStampItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 MousePos = Main.MouseWorld.ToTileCoordinates().ToVector2();
            if (player.altFunctionUse == 2)
            {
                StructureStamperSystem system = ModContent.GetInstance<StructureStamperSystem>();
                system.SetPoint2(MousePos);
                Dust.QuickBox(MousePos * 16, new Vector2(MousePos.X + 1, MousePos.Y + 1) * 16, 8, Color.CadetBlue, null);
            }
            else
            {
                StructureStamperSystem system = ModContent.GetInstance<StructureStamperSystem>();
                system.SetPoint1(Main.MouseWorld.ToTileCoordinates().ToVector2());
                Dust.QuickBox(MousePos * 16, new Vector2(MousePos.X + 1, MousePos.Y + 1) * 16, 8, Color.OrangeRed, null);
            }
            return true;
        }
    }
}