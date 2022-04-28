using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleSummary : Panel
{
	public static RoleSummary Instance;

	private Panel Roles { get; set; }
	private Label Empty { get; set; }

	public RoleSummary()
	{
		Instance = this;
		Init();
	}

	public void Init()
	{
		Roles.DeleteChildren( true );

		CreateRoleList( new Innocent(), TabMenu.Instance?.Innocents );
		CreateRoleList( new Detective(), TabMenu.Instance?.Detectives );
		CreateRoleList( new Traitor(), TabMenu.Instance?.Traitors );

		Empty.Enabled( !Roles.Children.Any() );
	}

	private void CreateRoleList( BaseRole role, Player[] players )
	{
		if ( !players.IsNullOrEmpty() )
			Roles.AddChild( new RoleList( role, players ) );
	}
}
