using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Buffs.Pets
{
    public class LightningFishBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{ 
			DisplayName.SetDefault("Fish Partner");
			Description.SetDefault("\"I now pronounce you Terrarian and Fish\"");
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 18000;
			player.GetModPlayer<AeroPlayer>().FishPartner = true;
			bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.LightningFish>()] <= 0;
			if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, ModContent.ProjectileType<Projectiles.Pets.LightningFish>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
		}
	}
}