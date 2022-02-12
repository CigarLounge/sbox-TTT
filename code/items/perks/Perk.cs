using Sandbox;

namespace TTT.Items
{
	public enum PerkCategory
	{
		Passive,
		Cooldown,
		Boolean,
	}

	public abstract class Perk : BaseNetworkable
	{
		public abstract PerkCategory GetCategory();
	}
}
