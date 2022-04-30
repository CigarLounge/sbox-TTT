using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleSummary : Panel
{
	public static RoleSummary Instance;

	private Panel Roles { get; set; }

	public RoleSummary()
	{
		Instance = this;
		Init();
	}

	public void Init()
	{
		Roles.DeleteChildren( true );

		CreateRoleList( new Innocent(), GeneralMenu.Instance?.Data.Innocents );
		CreateRoleList( new Detective(), GeneralMenu.Instance?.Data.Detectives );
		CreateRoleList( new Traitor(), GeneralMenu.Instance?.Data.Traitors );
	}

	private void CreateRoleList( BaseRole role, Player[] players )
	{
		if ( !players.IsNullOrEmpty() )
			Roles.AddChild( new RoleList( role, players ) );
	}
}
