using Sandbox;

namespace TTT;

public partial class SavedClient : BaseNetworkable
{
	[Net]
	public string Name { get; set; }

	[Net]
	public float ActiveKarma { get; set; }

	[Net]
	public float BaseKarma { get; set; }

	[Net]
	public int RoundScore { get; set; }

	public static SavedClient CopyFrom( Player player )
	{
		return new SavedClient()
		{
			Name = player.Client.Name,
			ActiveKarma = player.ActiveKarma,
			BaseKarma = player.BaseKarma,
			RoundScore = player.RoundScore
		};
	}
}
