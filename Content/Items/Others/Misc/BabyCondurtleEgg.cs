using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Content.NPCs.CrystalCaverns;
using AerovelenceMod.Content.NPCs.TownNPC.BabyCondurtleTownPet;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Misc
{
    public class BabyCondurtleEgg : TranslatableModItem
    {
        internal const byte HatchPacket = 230;
        private const string Description = "Hatches a Baby Condurtle town pet\nPermanently welcomes it to this world; it can share a home with a townsperson\nOccasionally laid by living Condurtles";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Baby Condurtle Egg", Description)
                .AddName(Language.Spanish, "Huevo de Condurtle Bebé")
                .AddTooltip(Language.Spanish, "Hace nacer a un Condurtle Bebé como mascota del pueblo\nLo acoge permanentemente en este mundo; puede compartir casa con un habitante\nLos Condurtles vivos ponen estos huevos de vez en cuando");
            Item.ResearchUnlockCount = 1;
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 45;
            Item.consumable = true;
            Item.UseSound = SoundID.Item4;
        }
        public override bool CanUseItem(Player player) => !BabyCondurtleWorld.Unlocked;
        public override bool ConsumeItem(Player player) => Main.netMode == NetmodeID.SinglePlayer;
        public override bool? UseItem(Player player)
        {
            if (!player.ItemAnimationJustStarted || player.whoAmI != Main.myPlayer || BabyCondurtleWorld.Unlocked)
                return false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write(HatchPacket);
                packet.Send();
            }
            else
                Hatch(player);
            return true;
        }
        internal static void ReceiveHatch(int sender)
        {
            if (Main.netMode != NetmodeID.Server || sender < 0 || sender >= Main.maxPlayers || BabyCondurtleWorld.Unlocked)
                return;
            Player player = Main.player[sender];
            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<BabyCondurtleEgg>() || player.HeldItem.stack <= 0)
                return;
            if (!Hatch(player))
                return;
            player.HeldItem.stack--;
            if (player.HeldItem.stack <= 0)
                player.HeldItem.TurnToAir();
            NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, sender, player.selectedItem);
        }
        private static bool Hatch(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || BabyCondurtleWorld.Unlocked)
                return false;
            BabyCondurtleWorld.Unlocked = true;
            int type = ModContent.NPCType<BabyCondurtle>();
            if (!NPC.AnyNPCs(type))
            {
                int index = NPC.NewNPC(player.GetSource_ItemUse(player.HeldItem), (int)player.Center.X, (int)player.Bottom.Y, type);
                if (index < Main.maxNPCs)
                {
                    NPC pet = Main.npc[index];
                    pet.homeless = true;
                    pet.GivenName = pet.getNewNPCName();
                    pet.netUpdate = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, index);
                }
            }
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }

    public class CondurtleEggLaying : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private int cooldown = 3600;
        private int rollClock;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.ModNPC is Condurtle;
        public override void PostAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.life <= 0 || npc.IsABestiaryIconDummy)
                return;
            if (cooldown > 0)
            {
                cooldown--;
                return;
            }
            if (++rollClock < 60)
                return;
            rollClock = 0;
            if (!Collision.SolidCollision(npc.BottomLeft + new Vector2(2f, 1f), npc.width - 4, 3) || npc.lavaWet || !Main.rand.NextBool(180))
                return;
            int eggType = ModContent.ItemType<BabyCondurtleEgg>();
            foreach (Item item in Main.ActiveItems)
                if (item.type == eggType && Vector2.DistanceSquared(item.Center, npc.Center) < 320f * 320f)
                    return;
            int index = Item.NewItem(npc.GetSource_FromAI(), new Rectangle((int)npc.Center.X - npc.direction * 16 - 8, (int)npc.Bottom.Y - 18, 16, 18), eggType);
            if (index >= 0 && index < Main.maxItems)
            {
                Main.item[index].velocity = new Vector2(-npc.direction * 0.8f, -1.5f);
                Main.item[index].noGrabDelay = 45;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
                cooldown = 18000;
            }
        }
    }
}
