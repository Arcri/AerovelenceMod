using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.SkillStrikes;
using LocalizedText = Terraria.Localization.LocalizedText;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Potions
{
    public class OnTheRocks : TranslatableModItem
    {
        private const string EnglishTooltip = "Makes you Tipsy and increases melee Skill Strike damage by 4% for 5 minutes\n'Stone-Aged'";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("OnTheRocks", EnglishTooltip)
            .AddName(Language.Default, "On The Rocks").AddTooltip(Language.Default, EnglishTooltip)
            .AddName(Language.Spanish, "En Las Rocas").AddTooltip(Language.Spanish, "Te pone achispado y aumenta un 4% el daño de los Golpes de Habilidad cuerpo a cuerpo durante 5 minutos\n'Añejado desde la Edad de Piedra'")
            .AddName(Language.French, "Sur Les Roches").AddTooltip(Language.French, "'Mûri depuis l'Âge de Pierre'")
            .AddName(Language.German, "Auf Den Felsen").AddTooltip(Language.German, "'Seit der Steinzeit gereift'")
            .AddName(Language.Italian, "Sulle Rocce").AddTooltip(Language.Italian, "'Invecchiato dall'Età della Pietra'")
            .AddName(Language.Polish, "Na Skałach").AddTooltip(Language.Polish, "'Dojrzewało od Epoki Kamienia'")
            .AddName(Language.PortugueseBrazil, "Nas Rochas").AddTooltip(Language.PortugueseBrazil, "'Envelhecido desde a Idade da Pedra'")
            .AddName(Language.Russian, "На Камнях").AddTooltip(Language.Russian, "'Настоящая древность'");
            Item.ResearchUnlockCount = 20;
            base.SetStaticDefaults();
            //.AddName(Language.ChineseTraditional, "石上").AddTooltip(Language.ChineseTraditional, "'石器時代'")
            //.AddName(Language.ChineseSimplified, "石上").AddTooltip(Language.ChineseSimplified, "'石器时代'");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Tipsy, 18000);
            return true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToFood(34, 34, ModContent.BuffType<OnTheRocksBuff>(), 18000);
            Item.width = 34;
            Item.height = 34;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarities.EarlyPHM;
        }
    }
    public class OnTheRocksBuff : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Potions/OnTheRocks";
        public override LocalizedText DisplayName => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.OnTheRocksBuff.DisplayName", () => "On the Rocks");
        public override LocalizedText Description => Terraria.Localization.Language.GetOrRegister("Mods.AerovelenceMod.Buffs.OnTheRocksBuff.Description", () => "4% increased melee Skill Strike damage");
        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<OnTheRocksPlayer>().Active = true;
    }

    public class OnTheRocksPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects() => Active = false;
        public override void ModifyHitNPCWithProj(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            var skill = projectile.GetGlobalProjectile<SkillStrikeGProj>();
            if (Active && projectile.CountsAsClass(DamageClass.Melee) && skill.SkillStrike && skill.skillStrikeAmount >= 0)
                modifiers.FinalDamage *= 1.04f;
        }
    }
}