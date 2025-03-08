using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Armor.Msc
{
    class AeroPlayerArmorMisc : AeroPlayerArmor
    {
        public bool isWearingJellyfishHat = false;
        
        private int jellyFrameCount = 0;
        private int jellyFrameTotal = 480;
        private int activeJellyProjectileIndex = -1;

        public override void UpdateEquips()
        {
            //Main.NewText("update equip firing");

            if (Player.armor[0].type == ModContent.ItemType<JellyfishHat>())
            {
                isWearingJellyfishHat = true;
                //Main.NewText("hat equiped");
            }

            if (isWearingJellyfishHat)
            {
                //Main.NewText("is wearing hat");
                JellyfishHatAbility();
            }


            //Main.NewText(isWearingJellyfishHat);
        }

        private void JellyfishHatAbility()
        {
            jellyFrameCount++;
            //Main.NewText(jellyFrameCount);

            if (jellyFrameCount > jellyFrameTotal)
            {
                jellyFrameCount = 0;
            }
            
            if (jellyFrameCount == 0)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    bool projectileExists = false;
                    //Main.NewText($"Frame: {jellyFrameCount}, Index: {activeJellyProjectileIndex}, Exists: {projectileExists}");

                    if (activeJellyProjectileIndex >= 0 && activeJellyProjectileIndex < Main.maxProjectiles)
                    {
                        Projectile exsistingProjectile = Main.projectile[activeJellyProjectileIndex];
                        projectileExists = exsistingProjectile.active && exsistingProjectile.owner == Player.whoAmI && exsistingProjectile.type == ModContent.ProjectileType<JellyfishAuraProjectile>();
                    }

                    if (!projectileExists)
                    {
                        var source = Player.GetSource_Misc("ModPlayerProjectile");

                        Vector2 position = Player.Center;
                        Vector2 velocity = Vector2.Zero;

                        int damage = 10;
                        float knockback = 5f;

                        activeJellyProjectileIndex = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<JellyfishAuraProjectile>(), damage, knockback, Player.whoAmI);

                        //Main.NewText("projectile exsists");
                    }
                }   
            }
        }

        public bool JellyfishHatEquiped()
        {
            return isWearingJellyfishHat;
        }

        public override void ResetEffects()
        {
            isWearingJellyfishHat = false;
            //Main.NewText(isWearingJellyfishHat);
        }
    }
}
