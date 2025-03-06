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
    public class CrystalStompers : ModItem
    {
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
            if (!mp.DashActive)
                return;

            if (mp.DashTimer == CrystalStompersPlayer.MAX_DASH_TIMER)
            {
                player.velocity.Y = mp.DashVelocity;
                player.immune = true;
                player.immuneNoBlink = true;
                player.immuneTime = 10;
            }

            Rectangle rectangle = new((int)(player.position.X + player.velocity.X * 0.5 - 4.0), (int)(player.position.Y + player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage && !npc.friendly && player.eocHit != i)
                {
                    Rectangle npcRect = npc.getRect();
                    if (rectangle.Intersects(npcRect) && (npc.noTileCollide || player.CanHit(npc)))
                    {
                        if (player.kbGlove)
                            Item.knockBack *= 2f;
                        if (player.kbBuff)
                            Item.knockBack *= 1.5f;

                        int direction = player.direction;
                        if (player.whoAmI == Main.myPlayer)
                            player.ApplyDamageToNPC(npc, Item.damage, Item.knockBack, direction, false);
                        player.velocity.X = -direction * 2;
                        player.velocity.Y = -8f;
                        player.immune = true;
                        player.immuneNoBlink = true;
                        player.immuneTime = 20;
                        player.eocHit = i;
                    }
                }
            }
            player.eocDash = mp.DashTimer;
            player.armorEffectDrawShadowEOCShield = true;

            Dust gd = Dust.NewDustDirect(
                player.position + new Vector2(0, 32),
                player.width,
                player.height - 32,
                ModContent.DustType<GlowPixelCross>(),
                player.velocity.X * 0.2f,
                player.velocity.Y * 0.2f,
                100,
                Color.DeepSkyBlue,
                0.5f
            );
            gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5, preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            mp.DashTimer--;
            mp.DashDelay--;
            if (mp.DashDelay == 0)
            {
                mp.DashDelay = CrystalStompersPlayer.MAX_DASH_DELAY;
                mp.DashTimer = CrystalStompersPlayer.MAX_DASH_TIMER;
                mp.DashActive = false;
                player.eocHit = -1;
            }
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

        public override void ResetEffects()
        {
            if (DashActive)
                return;

            bool dashAccessoryEquipped = false;
            for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
            {
                Item item = Player.armor[i];

                if (item.type == ModContent.ItemType<CrystalStompers>())
                    dashAccessoryEquipped = true;
            }

            if (!dashAccessoryEquipped || Player.setSolar || Player.mount.Active)
                return;

            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15 && !Player.pulley)
            {
                DashDir = DashDown;
                DashActive = true;
                DashTimer = MAX_DASH_TIMER;
                DashDelay = MAX_DASH_DELAY;
            }
        }

        public override void PreUpdate()
        {
            if (DashActive && Player.velocity.Y == 0)
            {
                DashActive = false;
                DashDelay = 0;
            }
        }
    }
}