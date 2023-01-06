using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.AreaPistols
{
	public class AmmoPlayer : ModPlayer
	{
        //Want to keep ammo in a player so it is synced between instances of a weapon
        //I don't want people to just have 20 of the weapon to overcome its downside

        public int ERIN_GUN_MAX_AMMO = 12; //not const cuz testing currently
        public int ErinAmmoCount = 12;

        public float reloadProgress = 0f;

        public override void ResetEffects()
        {
            ResetVariables();
        }
        public override void UpdateDead()
        {
            ResetVariables();
        }
        private void ResetVariables()
        {
            //ErinAmmoCount = ERIN_GUN_MAX_AMMO;
        }

        public override void PostUpdateMiscEffects()
        {
            Update();
        }

        private void Update()
        {
            //if the player is not holding the weapon, inc
        }
    }
}