using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Accessories.SmallAccessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class CrystalStompers : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Crystal Stompers", "Double-Tap Down to perform a crystal stomp")
            .AddName(Language.Spanish, "Pisadores de Cristal").AddTooltip(Language.Spanish, "Toca dos veces abajo para realizar un pisotón de cristal")
            .AddName(Language.French, "Écraseurs de Cristal").AddTooltip(Language.French, "Appuyez deux fois sur bas pour effectuer un écrasement de cristal")
            .AddName(Language.German, "Kristallstampfer").AddTooltip(Language.German, "Doppeltippen nach unten, um einen Kristallstampfer auszuführen")
            .AddName(Language.Italian, "Pestatori di Cristallo").AddTooltip(Language.Italian, "Tocca due volte in giù per eseguire un pestaggio di cristallo")
            //.AddName(Language.Polish, "Kryształowe Stompy").AddTooltip(Language.Polish, "Podwójne naciśnięcie w dół wykonuje kryształowy skok")
            //.AddName(Language.PortugueseBrazil, "Pisoteadores de Cristal").AddTooltip(Language.PortugueseBrazil, "Toque duas vezes para baixo para executar um pisoteio de cristal")
            .AddName(Language.Russian, "Кристальные Топтуны").AddTooltip(Language.Russian, "Дважды нажмите вниз, чтобы выполнить кристальный удар");
            //.AddName(Language.ChineseTraditional, "水晶踐踏者").AddTooltip(Language.ChineseTraditional, "雙擊向下執行水晶踐踏")
            //.AddName(Language.ChineseSimplified, "水晶践踏者").AddTooltip(Language.ChineseSimplified, "双击向下执行水晶践踏");
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarities.EarlyPHM;
            Item.damage = 20;
            Item.crit = 4;
            Item.knockBack = 4;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 6f;
            player.moveSpeed += 0.05f;

            CrystalStompersPlayer mp = player.GetModPlayer<CrystalStompersPlayer>();
            mp.Equipped = true;
            mp.StompDamage = Item.damage;
            mp.StompKnockback = Item.knockBack;
            if (mp.DashActive)
                player.maxFallSpeed = System.Math.Max(player.maxFallSpeed, mp.DashVelocity);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<CavernCrystalItem>(), 15).
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }

    public class CrystalStompersPlayer : ModPlayer
    {
        public static readonly int DashDown = 0;

        public int DashDir = -1;

        public bool DashActive = false;
        public int DashDelay = MAX_DASH_DELAY;
        public int DashTimer = MAX_DASH_TIMER;

        public readonly float DashVelocity = 14f;
        public static readonly int MAX_DASH_DELAY = 50;
        public static readonly int MAX_DASH_TIMER = 35;

        internal bool Equipped;
        internal int StompDamage;
        internal float StompKnockback;

        public override void ResetEffects()
        {
            Equipped = false;
            DashDir = Player.whoAmI == Main.myPlayer && Player.controlDown && Player.releaseDown
                && Player.doubleTapCardinalTimer[DashDown] > 0 && Player.doubleTapCardinalTimer[DashDown] < 15 ? DashDown : -1;
        }

        public override void PreUpdateMovement()
        {
            if (!Equipped || Player.setSolar || Player.mount.Active || Player.dead || Player.CCed || Player.pulley)
            {
                if (DashActive) EndStomp();
                return;
            }
            if (!DashActive && DashDir == DashDown)
            {
                DashActive = true;
                DashTimer = MAX_DASH_TIMER;
                DashDelay = MAX_DASH_DELAY;
                Player.jump = 0;
                Player.velocity.Y = DashVelocity;
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.immuneTime = System.Math.Max(Player.immuneTime, 10);
                Player.eocHit = -1;
            }
            if (!DashActive) return;
            Player.maxFallSpeed = System.Math.Max(Player.maxFallSpeed, DashVelocity);
            Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5f - 4f),
                (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4f), Player.width + 8, Player.height + 8);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.dontTakeDamage || npc.friendly || Player.eocHit == npc.whoAmI || !hitbox.Intersects(npc.getRect())
                    || (!npc.noTileCollide && !Player.CanHit(npc))) continue;
                float knockback = StompKnockback * (Player.kbGlove ? 2f : 1f) * (Player.kbBuff ? 1.5f : 1f);
                int direction = Player.direction;
                if (Player.whoAmI == Main.myPlayer)
                    Player.ApplyDamageToNPC(npc, StompDamage, knockback, direction, false);
                Player.velocity = new Vector2(-direction * 2f, -8f);
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.immuneTime = System.Math.Max(Player.immuneTime, 20);
                EndStomp();
                return;
            }
            Player.eocDash = System.Math.Max(0, DashTimer);
            Player.armorEffectDrawShadowEOCShield = true;
            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustDirect(Player.position + new Vector2(0f, 32f), Player.width,
                    System.Math.Max(1, Player.height - 32), ModContent.DustType<GlowPixelCross>(),
                    Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, Color.DeepSkyBlue, 0.5f);
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
            DashTimer = System.Math.Max(0, DashTimer - 1);
            if (--DashDelay <= 0) EndStomp();
        }

        public override void PostUpdate()
        {
            if (DashActive && Player.velocity.Y == 0f)
                EndStomp();
        }

        internal void EndStomp()
        {
            DashActive = false;
            DashDir = -1;
            DashDelay = MAX_DASH_DELAY;
            DashTimer = MAX_DASH_TIMER;
            Player.eocDash = 0;
            Player.eocHit = -1;
            Player.armorEffectDrawShadowEOCShield = false;
        }

        public override void UpdateDead()
        {
            if (DashActive) EndStomp();
        }
    }
}