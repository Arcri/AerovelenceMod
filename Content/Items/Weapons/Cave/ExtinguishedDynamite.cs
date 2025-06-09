using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Cave
{
    public class ExtinguishedDynamiteNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Dictionary<int, int> HitCounter = new();

        public override void ResetEffects(NPC npc)
        {
            if (!npc.active)
            {
                HitCounter.Clear();
            }
        }
    }

    public class ExtinguishedDynamite : TranslatableModItem
    {
        private int ExplosionRadius = 120;
        private float ExplosionKnockback = 12f;
        private int ExplosionDamage = 40;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            this.ModifyLocalization("ExtinguishedDynamite", "")
            .AddName(Language.Default, "Extinguished Dynamite")
            .AddSkillStrike(Language.Default, "Every second hit Skill Strikes with an explosion")

            .AddName(Language.Spanish, "Dinamita Extinguida").AddSkillStrike(Language.Spanish, "Cada segundo explota y realiza Golpes de Habilidad")
            .AddName(Language.French, "Dynamite Éteinte").AddSkillStrike(Language.French, "Explose toutes les secondes et déclenche des Coups de Compétence")
            .AddName(Language.German, "Gelöschte Dynamitstange").AddSkillStrike(Language.German, "Explodiert jede Sekunde und führt Fähigkeitsschläge aus")
            .AddName(Language.Italian, "Dinamite Spenta").AddSkillStrike(Language.Italian, "Esplode ogni secondo ed esegue Colpi dell'Abilità")
            //.AddName(Language.Polish, "Zgaszony Dynamit").AddSkillStrike(Language.Polish, "Eksploduje co sekundę i wykonuje Ciosy Umiejętności")
            //.AddName(Language.PortugueseBrazil, "Dinamite Extinta").AddSkillStrike(Language.PortugueseBrazil, "Explode a cada segundo e realiza Golpes de Habilidade")
            .AddName(Language.Russian, "Потухший Динамит").AddSkillStrike(Language.Russian, "Взрывается при каждом втором попадании и активирует Навык Удара");
            //.AddName(Language.ChineseTraditional, "熄滅炸藥").AddSkillStrike(Language.ChineseTraditional, "每秒爆炸並觸發技能打擊")
            //.AddName(Language.ChineseSimplified, "熄灭炸药").AddSkillStrike(Language.ChineseSimplified, "每秒爆炸并触发技能打击");

        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(silver: 75);
            Item.damage = 18;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileID.None;
        }

        //public override void ModifyTooltips(List<TooltipLine> tooltips)
        //{
        //    TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] The Explosion Skill Strikes [i:" + ItemID.FallenStar + "]")
        //    {
        //        OverrideColor = Color.Gold,
        //    };
        //    tooltips.Add(SkillStrike);
        //}

        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Dynamite, 3);
            recipe.AddRecipeGroup("AerovelenceMod:IronBars", 8);
            recipe.AddIngredient(ItemID.Torch, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ExtinguishedDynamiteNPC globalNPC = target.GetGlobalNPC<ExtinguishedDynamiteNPC>();

            if (!globalNPC.HitCounter.ContainsKey(target.whoAmI))
                globalNPC.HitCounter[target.whoAmI] = 0;

            globalNPC.HitCounter[target.whoAmI]++;

            if (globalNPC.HitCounter[target.whoAmI] >= 2)
            {
                globalNPC.HitCounter[target.whoAmI] = 0;
                SoundEngine.PlaySound(SoundID.Item14, player.position);
                player.GetModPlayer<AeroPlayer>().ScreenShakePower = 2f;

                int explosion = Projectile.NewProjectile(null, target.Center, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);
                if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
                {
                    feh.color = Color.OrangeRed;
                    feh.colorIntensity = 1f;
                    feh.fadeSpeed = 0.025f;
                    for (int m = 0; m < 10; m++)
                    {
                        FadeExplosionClass newSmoke = new FadeExplosionClass(Main.projectile[explosion].Center, new Vector2(1f, 0).RotatedByRandom(6) * Main.rand.NextFloat(0.5f, 2f));
                        newSmoke.size = 0.25f + Main.rand.NextFloat(-0.15f, 0.15f);
                        feh.Smokes.Add(newSmoke);
                    }
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nearbyNPC = Main.npc[i];
                    if (nearbyNPC.active && !nearbyNPC.dontTakeDamage && !nearbyNPC.friendly &&
                        Vector2.Distance(target.Center, nearbyNPC.Center) < 100f)
                    {
                        int direction = 0;
                        if (player.Center.X - nearbyNPC.Center.X < 0)
                            direction = 1;
                        else
                            direction = -1;
                        hit.Knockback = Item.knockBack * 2 * Main.npc[i].knockBackResist;
                        hit.HitDirection = direction;
                        int explosionDamage = (int)(Item.damage * 0.5f * Main.rand.NextFloat(0.90f, 1.1f));
                        SkillStrikeOldNPC skillNPC = nearbyNPC.GetGlobalNPC<SkillStrikeOldNPC>();
                        skillNPC.DirectStrike = true;
                        SkillStrikeUtil.fakeSkillStrike(player, nearbyNPC, nearbyNPC.Center, 1.5f, false, explosionDamage);
                    }
                }
            }
        }
    }
}