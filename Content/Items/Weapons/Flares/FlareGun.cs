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
using System;
using Terraria.Audio;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Flares
{
    public class FlareGun : TranslatableModItem
    {
        public int lockOutTimer;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("CombatFlareGun", "Does not require ammo\n5% summon tag crit chance\nClick again with good timing to fire faster")
            .AddName(Language.Default, "Combat Flare Gun").AddTooltip(Language.Default, "Does not require ammo\n5% summon tag crit chance\nClick again with good timing to fire faster")
            .AddSkillStrike(Language.Default, "Skill Strikes at long range")

            .AddName(Language.Spanish, "Pistola de Bengalas de Combate").AddTooltip(Language.Spanish, "No requiere munición\n5% de probabilidad de crítico con etiqueta de invocador\nHaz clic de nuevo con buen tiempo para disparar más rápido").AddSkillStrike(Language.Spanish, "Golpes de Habilidad a larga distancia")
            .AddName(Language.French, "Pistolet de Détresse de Combat").AddTooltip(Language.French, "Ne nécessite pas de munitions\n5% de chance de coup critique pour les sbires\nCliquez à nouveau avec un bon timing pour tirer plus vite").AddSkillStrike(Language.French, "Les Coups de Compétence se déclenchent à longue portée")
            .AddName(Language.German, "Kampfsignalpistole").AddTooltip(Language.German, "Benötigt keine Munition\n5% kritische Trefferchance für Beschwörer\nKlicke erneut mit gutem Timing, um schneller zu feuern").AddSkillStrike(Language.German, "Fähigkeitsschläge treten auf große Entfernung auf")
            .AddName(Language.Italian, "Pistola Razzo da Combattimento").AddTooltip(Language.Italian, "Non richiede munizioni\n5% di probabilità di critico per tag degli evocatori\nClicca di nuovo con il giusto tempismo per sparare più velocemente").AddSkillStrike(Language.Italian, "I Colpi dell'Abilità si attivano a lunga distanza")
            //.AddName(Language.Polish, "Bojowa Raca").AddTooltip(Language.Polish, "Nie wymaga amunicji\n5% szans na krytyk dla tagów przywołańców\nKliknij ponownie we właściwym momencie, aby strzelać szybciej").AddSkillStrike(Language.Polish, "Ciosy Umiejętności występują na dalekim zasięgu")
            //.AddName(Language.PortugueseBrazil, "Sinalizador de Combate").AddTooltip(Language.PortugueseBrazil, "Não requer munição\n5% de chance de crítico para invocadores\nClique novamente com bom tempo para disparar mais rápido").AddSkillStrike(Language.PortugueseBrazil, "Os Golpes de Habilidade ocorrem a longa distância")
            .AddName(Language.Russian, "Боевой сигнальный пистолет").AddTooltip(Language.Russian, "Не требует боеприпасов\n5% шанс критического удара для призывателей\nНажмите снова с хорошим таймингом, чтобы стрелять быстрее").AddSkillStrike(Language.Russian, "Навык Удара активируется на дальнем расстоянии");
            //.AddName(Language.ChineseTraditional, "戰鬥照明槍").AddTooltip(Language.ChineseTraditional, "不需要彈藥\n5%召喚標籤暴擊率\n適時點擊可更快開火").AddSkillStrike(Language.ChineseTraditional, "技能打擊發生在遠距離")
            //.AddName(Language.ChineseSimplified, "战斗信号枪").AddTooltip(Language.ChineseSimplified, "不需要弹药\n5%召唤标记暴击率\n适时点击可更快开火").AddSkillStrike(Language.ChineseSimplified, "技能打击发生在远距离");
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.knockBack = KnockbackTiers.ExtremelyWeak;

            Item.width = 46;
            Item.height = 28;
            Item.useTime = 70;
            Item.useAnimation = 70;

            Item.UseSound = SoundID.Item110;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarities.MidPHM;

            Item.shoot = ModContent.ProjectileType<FlareGunHeldProjectile>();
            Item.shootSpeed = 17f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FlareGun).
                AddIngredient(ItemID.DemoniteBar, 5).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<FireFlare>(), damage, knockback, Main.myPlayer);
            
            float aim = velocity.ToRotation() + MathHelper.Pi;

            for (int m = 0; m < 8; m++) // m < 9
            {
                float dustRot = aim + 1.57f * 1.5f + Main.rand.NextFloat(-0.4f, 0.4f);

                Dust d = GlowDustHelper.DrawGlowDustPerfect(player.Center - aim.ToRotationVector2() * 35, ModContent.DustType<GlowCircleDust>(), Vector2.One.RotatedBy(dustRot) * (Main.rand.NextFloat(4) + 1),
                    new Color(255, 75, 50), 0.60f + Main.rand.NextFloat(0,0.2f), 0.7f, 0f, dustShader); // 0.6
                d.velocity *= 0.75f;
                d.fadeIn = 1;
            }
            
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (player.controlUseItem == true && player.ownedProjectileCounts[ModContent.ProjectileType<FlareGunHeldProjectile>()] >= 1 && player.channel == false)
            {
                //Projectile p = Main.projectile[player.heldProj];

                if (lockOutTimer < 5) //5
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Menu_Close") with { Pitch = -1f, MaxInstances = 0, Volume = 1f };

                    SoundEngine.PlaySound(style, player.Center);
                    SoundEngine.PlaySound(style, player.Center);

                    CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 2, 2), Color.Red, "Too Early", false, true);
                }

                lockOutTimer = 70;
            }
            lockOutTimer = Math.Clamp(lockOutTimer - 1, 0, 70);
        }

        public override bool CanUseItem(Player player)
        {
            if (lockOutTimer > 0)
            {
                return false;
            }

            return true;
        }
    }
}