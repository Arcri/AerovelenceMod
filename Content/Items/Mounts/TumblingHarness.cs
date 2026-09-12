using AerovelenceMod.Common.Systems.Language;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Mounts
{
    public class TumblingHarness : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            const string englishTooltip = "Summons a rideable tumblerock\nHold Up while grounded to conjure an electric ramp; keep holding to loop\nRelease Up to leap off along the ramp's direction\nElectrifies the ball while ramp riding and briefly after launching, dealing contact damage\nNegates fall damage while mounted";
            this.ModifyLocalization("Tumbling Harness", englishTooltip)
                .AddName(Language.Default, "Tumbling Harness").AddTooltip(Language.Default, englishTooltip)
                .AddName(Language.Spanish, "Arnés Rodante").AddTooltip(Language.Spanish, "Invoca una roca rodante que puedes montar\nMantén pulsado Arriba mientras estás en el suelo para conjurar una rampa eléctrica; sigue pulsando para hacer bucles\nSuelta Arriba para salir impulsado en la dirección de la rampa\nLa roca se electrifica al recorrer la rampa y brevemente tras salir impulsada, e inflige daño por contacto\nAnula el daño por caída mientras montas");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 32;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item79;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.mountType = ModContent.MountType<TumblingMount>();
        }
    }

    public class TumblingMountBuff : ModBuff
    {
        public override string Texture => "AerovelenceMod/Content/Items/Mounts/TumblingHarness";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<TumblingMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
