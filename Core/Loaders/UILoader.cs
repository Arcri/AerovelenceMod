#region Using directives

using System;
using System.Linq;
using System.Collections.Generic;

using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using AerovelenceMod.Core.Abstracts;

#endregion

namespace AerovelenceMod.Core.Loaders
{
    internal sealed class UILoader : ILoadable
	{
		public float Priority => 1f;

		public bool LoadOnDedServer => false;

		public static List<SmartUIState> UIStates = new List<SmartUIState>();
		public static List<UserInterface> UserInterfaces = new List<UserInterface>();

		public void Load(Mod mod)
		{
			UIStates = new List<SmartUIState>();
			UserInterfaces = new List<UserInterface>();

			foreach (Type t in mod.Code.GetTypes())
			{
				if (t.IsSubclassOf(typeof(SmartUIState)))
				{
					var userInterface = new UserInterface();
					var state = (SmartUIState)Activator.CreateInstance(t, null);

					userInterface.SetState(state);

					UIStates.Add(state);
					UserInterfaces.Add(userInterface);
				}
			}
		}

		public void Unload()
		{
			UIStates = null;
			UserInterfaces = null;
		}

		public static void AddLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, InterfaceScaleType scale)
		{
			string name = state == null ? "Unknown" : state.ToString();
			layers.Insert(index, new LegacyGameInterfaceLayer("EH: " + name,
				delegate
				{
					if (visible)
					{
						userInterface.Update(Main._drawInterfaceGameTime);
						state.Draw(Main.spriteBatch);
					}
					return true;
				}, scale));
		}

		public static T GetUIState<T>() where T : SmartUIState => UIStates.FirstOrDefault(n => n is T) as T;

		public static void ReloadState<T>() where T : SmartUIState
		{
			var index = UIStates.IndexOf(GetUIState<T>());
			UIStates[index] = (T)Activator.CreateInstance(typeof(T), null);
			UserInterfaces[index] = new UserInterface();
			UserInterfaces[index].SetState(UIStates[index]);
		}
	}
}
