using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using AerovelenceMod.Common.Utilities;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class Solsear : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Solsear", "Fires a continuous laser that deals more damage at the tip\nRight-Click to fire a bomb\nAim the tip of the laser into the bomb to increase its damage and size\n'With great firepower comes great irresponsibility'")
            .AddName(Language.Default, "Solsear").AddTooltip(Language.Default, "Fires a continuous laser that deals more damage at the tip\nRight-Click to fire a bomb\nAim the tip of the laser into the bomb to increase its damage and size\n'With great firepower comes great irresponsibility'")
            .AddSkillStrike(Language.Default, "Bomb Skill Strikes at maximum size")

            .AddName(Language.Spanish, "Abrasolador").AddTooltip(Language.Spanish, "Dispara un láser continuo que hace más daño en la punta\nHaz clic derecho para disparar una bomba\nApunta la punta del láser a la bomba para aumentar su daño y tamaño\n'Con un gran poder de fuego viene una gran irresponsabilidad'").AddSkillStrike(Language.Spanish, "Las Bombas realizan Golpes de Habilidad en su tamaño máximo")
            .AddName(Language.French, "Solbrûlure").AddTooltip(Language.French, "Tire un laser continu qui inflige plus de dégâts à son extrémité\nClic droit pour tirer une bombe\nVisez la pointe du laser dans la bombe pour augmenter ses dégâts et sa taille\n'Un grand pouvoir de feu implique une grande irresponsabilité'").AddSkillStrike(Language.French, "Les Bombes déclenchent des Coups de Compétence à leur taille maximale")
            .AddName(Language.German, "Sonnensengen").AddTooltip(Language.German, "Feuert einen kontinuierlichen Laser, der an der Spitze mehr Schaden verursacht\nRechtsklick zum Abfeuern einer Bombe\nRichte die Spitze des Lasers auf die Bombe, um deren Schaden und Größe zu erhöhen\n'Mit großer Feuerkraft kommt große Verantwortungslosigkeit'").AddSkillStrike(Language.German, "Bomben führen Fähigkeitsschläge bei maximaler Größe aus")
            .AddName(Language.Italian, "Scottasole").AddTooltip(Language.Italian, "Spara un laser continuo che infligge più danni alla punta\nTasto destro per sparare una bomba\nMira la punta del laser sulla bomba per aumentarne il danno e la dimensione\n'Con grande potenza di fuoco viene grande irresponsabilità'").AddSkillStrike(Language.Italian, "Le Bombe eseguono Colpi dell'Abilità alla massima dimensione")
            //.AddName(Language.Polish, "Słoneczny Przypał").AddTooltip(Language.Polish, "Strzela ciągłym laserem, który zadaje większe obrażenia na końcu\nPrawy przycisk, aby wystrzelić bombę\nSkieruj koniec lasera na bombę, aby zwiększyć jej obrażenia i rozmiar\n'Z wielką siłą ognia przychodzi wielka nieodpowiedzialność'").AddSkillStrike(Language.Polish, "Bomby wykonują Ciosy Umiejętności na maksymalnym rozmiarze")
            //.AddName(Language.PortugueseBrazil, "Sol Ardente").AddTooltip(Language.PortugueseBrazil, "Dispara um laser contínuo que causa mais dano na ponta\nBotão direito para disparar uma bomba\nAponte a ponta do laser para a bomba para aumentar seu dano e tamanho\n'Com grande poder de fogo vem grande irresponsabilidade'").AddSkillStrike(Language.PortugueseBrazil, "As Bombas realizam Golpes de Habilidade no tamanho máximo")
            .AddName(Language.Russian, "Солнцежог").AddTooltip(Language.Russian, "Стреляет непрерывным лазером, который наносит больше урона на кончике\nПКМ, чтобы выстрелить бомбой\nНаправьте кончик лазера на бомбу, чтобы увеличить её урон и размер\n'С великой огневой мощью приходит великая безответственность'").AddSkillStrike(Language.Russian, "Бомбы активируют Навык Удара при максимальном размере");
            //.AddName(Language.ChineseTraditional, "日灼").AddTooltip(Language.ChineseTraditional, "發射持續的雷射，末端傷害更高\n右鍵發射炸彈\n將雷射尖端對準炸彈可增加其傷害和大小\n'強大火力帶來極大不負責任'").AddSkillStrike(Language.ChineseTraditional, "炸彈在最大尺寸時觸發技能打擊")
            //.AddName(Language.ChineseSimplified, "日灼").AddTooltip(Language.ChineseSimplified, "发射持续的激光，末端伤害更高\n右键发射炸弹\n将激光尖端对准炸弹可增加其伤害和大小\n'强大火力带来极大不负责任'").AddSkillStrike(Language.ChineseSimplified, "炸弹在最大尺寸时触发技能打击");
        }


        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.knockBack = 1f; //Very weak
            Item.width = 92;
            Item.height = 30;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.shootSpeed = 4f; 
            Item.scale = 1.15f;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<SolsearHeldProj>();
            Item.rare = ItemRarities.EarlyHardmode;
            Item.value = Item.sellPrice(0, 4, 50, 0);

            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ItemID.SoulofLight, 7).
                AddRecipeGroup("AerovelenceMod:MechSouls", 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                damage = (int)(damage * 0.5f);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {

                Vector2 offset = Vector2.Normalize(velocity).RotatedBy(-1.57f * player.direction) * 10;
                offset += position;
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SolsearHeld2>(), 0, 0, player.whoAmI);

                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                {
                    position += muzzleOffset;
                }
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SolsearBomb>(), damage, 0, player.whoAmI);
                player.velocity += velocity * -1;

                SoundStyle styleba = new SoundStyle("AerovelenceMod/Sounds/Effects/fireLoopBad") with { Volume = .12f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleba, player.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/Item125Trim") with { Volume = .45f, Pitch = .93f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(styleb, player.Center);

                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .44f, Volume = 0.9f, PitchVariance = 0.11f};
                SoundEngine.PlaySound(styla, player.Center);


                for (int i22 = 0; i22 < 10; i22++) //4 //2,2
                {
                    Color col = Main.rand.NextBool(2) ? new Color(255, 45, 0) : Color.OrangeRed;

                    Dust p = Dust.NewDustPerfect(position + velocity.SafeNormalize(Vector2.UnitX) * 5, ModContent.DustType<LineSpark>(),
                        velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-1.8f, 1.8f)) * Main.rand.Next(4, 12),
                        newColor: col, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);
                    p.velocity += velocity * (2.45f + Main.rand.NextFloat(-0.1f, -0.2f));

                    p.customData = AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                        1f, 0.75f);

                }

                for (int i = 0; i < 2; i++)
                {
                    int b = Projectile.NewProjectile(null, position, velocity * 0.65f, ModContent.ProjectileType<CirclePulse>(), 0, 0, Main.myPlayer);
                    Main.projectile[b].rotation = velocity.ToRotation();
                    if (Main.projectile[b].ModProjectile is CirclePulse pulseb)
                    {
                        pulseb.color = new Color(255, 60, 5);
                        pulseb.size = 0.3f;
                    }
                }

                return false;
            }
            else
            {
                SoundStyle styla = new SoundStyle("Terraria/Sounds/Item_122") with { Pitch = .86f, PitchVariance = 0.11f};
                SoundEngine.PlaySound(styla, player.Center);
                return true;
            }
        }
        public override void HoldItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noUseGraphic = true;
                Item.useTime = 40; //10
                Item.useAnimation = 40; //5
            }
            else
            {
                Item.noUseGraphic = true;
            }
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;
            GlowmaskUtilities.DrawItemGlowmask(spriteBatch, glowMask, this.Item, rotation, scale);
        }
    }

    public class SolsearRiseDust : GlowCircleRiseFlare
    {
        public override string Texture => "AerovelenceMod/Content/Dusts/GlowDusts/DustTextures/Flare";

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color black = Color.Black;
            Color gray = new Color(25, 25, 25);
            Color ret;
            if (dust.alpha < 80)
            {
                ret = Color.Lerp(Color.Black, Color.Gray, dust.alpha / 80f * 0.5f);
            }
            else if (dust.alpha < 140)
            {
                ret = Color.Lerp(Color.Gray, Color.LightGray * 0.5f, (dust.alpha - 80) / 80f * 0.5f);
            }
            else
                ret = gray;
            return ret * ((255 - dust.alpha) / 255f);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.color = dust.GetAlpha(Color.Black);           
            dust.alpha += 2;
            dust.velocity.Y += -0.02f;

            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }
    }

    public class SolsearHeld2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        //This class exists literally to just draw the glowmask on m2
        private bool initialized = false;

        private Vector2 currentDirection => Projectile.rotation.ToRotationVector2();

        Player owner => Main.player[Projectile.owner];

        //public override void SetStaticDefaults() => DisplayName.SetDefault("Solsear");

        float justShotPower = 1f;
        float justShotPowerWeaker = 1f;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            owner.heldProj = Projectile.whoAmI;

            if (owner.itemTime <= 1)
                Projectile.active = false;

            Projectile.Center = owner.Center;

            if (!initialized)
            {
                initialized = true;
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.25f, 0.2f), 0f, 1f);
            justShotPowerWeaker = Math.Clamp(MathHelper.Lerp(justShotPowerWeaker, -0.2f, 0.1f), 0f, 1f);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/Solsear").Value;
            Texture2D glowMask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Ember/SolesearGlowMask").Value;

            Vector2 position = (owner.Center + (currentDirection * 22)) - Main.screenPosition;

            Vector2 scale = new Vector2(1f - (0.15f * justShotPower), 1f) * 1f;

            if (owner.direction == 1)
            {
                SpriteEffects effects1 = SpriteEffects.None;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation(), texture.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPowerWeaker * 2f, currentDirection.ToRotation(), glowMask.Size() / 2, scale, effects1, 0.0f);

            }
            else
            {
                SpriteEffects effects1 = SpriteEffects.FlipHorizontally;
                Main.spriteBatch.Draw(texture, position, null, lightColor, currentDirection.ToRotation() - 3.14f, texture.Size() / 2, scale, effects1, 0.0f);
                Main.spriteBatch.Draw(glowMask, position, null, Color.White, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

                Main.spriteBatch.Draw(glowMask, position, null, Color.White with { A = 0 } * justShotPowerWeaker * 2f, currentDirection.ToRotation() - 3.14f, glowMask.Size() / 2, scale, effects1, 0.0f);

            }

            return false;
        }
    }
}