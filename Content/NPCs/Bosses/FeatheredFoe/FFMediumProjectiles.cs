using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe
{
    public class TalonSlash : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 170;

        }

        public float accelTime = 40f;

        float animProgress = 0;
        float alpha = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();

                //SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_ogre_spit") with { Pitch = 1f, PitchVariance = .33f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style, Projectile.Center);

                //SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_ballista_tower_shot_1") with { Pitch = .54f, PitchVariance = 0.2f, Volume = 0.3f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style3, Projectile.Center);

                //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_42") with { Pitch = .2f, PitchVariance = .2f, Volume = 0.55f, MaxInstances = -1 };
                //SoundEngine.PlaySound(style2, Projectile.Center);

                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity = Vector2.Zero;
            }


            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D Feather = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/Feather").Value;
            Texture2D FeatherGray = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherGray").Value;
            Texture2D FeatherWhite = Mod.Assets.Request<Texture2D>("Content/Projectiles/TempVFX/FeatherWhite").Value;


            return false;
        }

        public override void OnKill(int timeLeft)
        {

            base.OnKill(timeLeft);
        }
    }

}