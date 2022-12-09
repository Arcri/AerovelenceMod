using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged;
using ReLogic.Content;
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;

namespace AerovelenceMod.Content.Items.Weapons.Desert
{
    public class SandBolt : ModProjectile
    {
        public override bool ShouldUpdatePosition() => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Bolt");
        }
        int timer = 0;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");


            if (Projectile.timeLeft % 2 == 0 || Main.rand.NextBool())
            {
                int dust1 = GlowDustHelper.DrawGlowDust(Projectile.position, 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0,
                    Color.SandyBrown, Main.rand.NextFloat(0.05f, 0.15f), 0.6f, 0f, dustShader1);

                Main.dust[dust1].velocity *= 0.75f;
            }


            //Looks neat but a lil too dust spammy
            
            int dust3 = GlowDustHelper.DrawGlowDust(Projectile.position, 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0,
                Color.SandyBrown, 0.1f, 0.6f, 0f, dustShader1);
            Main.dust[dust3].velocity *= 0.1f;
            

            if (Projectile.timeLeft % 2 == 0 || Main.rand.NextBool())
            {
                int dust2 = GlowDustHelper.DrawGlowDust(Projectile.position, 1, 1, ModContent.DustType<GlowCircleSoft>(), 0, 0,
                    Color.SandyBrown, Main.rand.NextFloat(0.1f, 0.15f), 1f, 0f, dustShader2);

                Main.dust[dust2].velocity *= 0.70f;
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_100") with { Volume = .29f, Pitch = 0.95f, PitchVariance = .08f, MaxInstances = 1 };
            SoundEngine.PlaySound(style, Projectile.Center);

            ArmorShaderData dustShader1 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            for (int i = 0; i < 4; i++)
            {
                int dust1 = GlowDustHelper.DrawGlowDust(Projectile.position, 1, 1, ModContent.DustType<GlowCircleDust>(), 0, 0,
                    Color.SandyBrown, Main.rand.NextFloat(0.1f, 0.3f), 0.6f, 0f, dustShader1);
            }

            //SoundEngine.PlaySound(SoundID.GuitarAm with { Pitch = 0.4f, Volume = 0.35f, PitchVariance = 0.1f }, Projectile.Center);
            //SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollide") with { Volume = .3f, PitchVariance = 0.3f, Pitch = -0.3f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            return false;
        }

    }
}
