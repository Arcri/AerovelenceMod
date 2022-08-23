using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace AerovelenceMod.Effects.Dyes
{
	public class GlowDustDye : ModItem
	{
		//This is neccesary for the glow dust system to work
		public override void SetStaticDefaults()
		{
			// Avoid loading assets on dedicated servers. They don't use graphics cards.
			if (!Main.dedServ)
			{
				// The following code creates an effect (shader) reference and associates it with this item's type Id.
				GameShaders.Armor.BindShader(
					Item.type,
					new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic") // Be sure to update the effect path and pass name here.
				);
			}

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			// Item.dye will already be assigned to this item prior to SetDefaults because of the above GameShaders.Armor.BindShader code in Load().
			// This code here remembers Item.dye so that information isn't lost during CloneDefaults.
			int dye = Item.dye;

			Item.dye = dye;
		}
	}
}