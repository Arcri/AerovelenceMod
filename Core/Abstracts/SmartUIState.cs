#region Using directives

using System.Collections.Generic;

using Terraria.UI;

#endregion

namespace AerovelenceMod.Core.Abstracts
{
	public abstract class SmartUIState : UIState
	{
		public abstract int InsertionIndex(List<GameInterfaceLayer> layers);

		public virtual bool Visible { get; set; } = false;

		public virtual InterfaceScaleType Scale { get; set; } = InterfaceScaleType.UI;
	}
}
