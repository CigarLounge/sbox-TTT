using System.Collections.Generic;
using Sandbox;
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
	}

	public void CreateRoleList( BaseRole role, Player[] players )
	{
		Roles.AddChild( new RoleList( role, players ) );
	}

	public void Reset()
	{
		Roles.DeleteChildren( true );
	}

	[ClientRpc]
	public static void LoadSummaryData( Player[] innocents, Player[] detectives, Player[] traitors )
	{
		if ( Instance == null )
			return;

		Instance.Reset();

		if ( !innocents.IsNullOrEmpty() )
			Instance.CreateRoleList( new Innocent(), innocents );

		// if ( !detectives.IsNullOrEmpty() )
		// 	Instance.CreateRoleList( new Detective(), detectives );

		if ( !traitors.IsNullOrEmpty() )
			Instance.CreateRoleList( new Traitor(), traitors );
	}
}
