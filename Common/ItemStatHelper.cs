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

namespace AerovelenceMod.Common
{
	public static class ItemStatHelper
	{
        //Yoinked from Aequus because this is really smart and there really isn't a better way to do it
        public const int RarityBanner = ItemRarityID.Blue;
        public const int RarityBossMasks = ItemRarityID.Blue;
        public const int RarityDemoniteCrimtane = ItemRarityID.Blue;

        public const int RarityDungeon = ItemRarityID.Green;

        public const int RarityQueenBee = ItemRarityID.Orange;
        public const int RarityJungle = ItemRarityID.Orange;
        public const int RarityMolten = ItemRarityID.Orange;
        public const int RarityPet = ItemRarityID.Orange;

        public const int RarityWallofFlesh = ItemRarityID.LightRed;
        public const int RarityEarlyHardmode = ItemRarityID.LightRed;
        public const int RarityPreMechs = ItemRarityID.LightRed;

        public const int RarityMechs = ItemRarityID.Pink;

        public const int RarityPlantera = ItemRarityID.Lime;

        public const int RarityHardmodeDungeon = ItemRarityID.Yellow;
        public const int RarityMartians = ItemRarityID.Yellow;
        public const int RarityDukeFishron = ItemRarityID.Yellow;

        public const int RarityLunaticCultist = ItemRarityID.Cyan;
        public const int RarityMoonLord = ItemRarityID.Red;
    }
}
