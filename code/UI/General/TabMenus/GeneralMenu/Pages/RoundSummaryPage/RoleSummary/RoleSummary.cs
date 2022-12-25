using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

public partial class RoleSummary : Panel
{
	public static RoleSummary Instance;

	private Panel Empty { get; set; }
	private Panel Innocents { get; set; }
	private Panel Detectives { get; set; }
	private Panel Traitors { get; set; }

	public RoleSummary() => Instance = this;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		Init();
	}

	public void Init()
	{
		Innocents.DeleteChildren( true );
		Detectives.DeleteChildren( true );
		Traitors.DeleteChildren( true );

		if ( GeneralMenu.Instance is not null )
		{
			if ( !GeneralMenu.Instance.LastRoleSummaryData.Innocents.IsNullOrEmpty() )
				Innocents.AddChild( new RoleList() { Role = new Innocent(), Players = GeneralMenu.Instance.LastRoleSummaryData.Innocents } );

			if ( !GeneralMenu.Instance.LastRoleSummaryData.Detectives.IsNullOrEmpty() )
				Detectives.AddChild( new RoleList() { Role = new Detective(), Players = GeneralMenu.Instance.LastRoleSummaryData.Detectives } );

			if ( !GeneralMenu.Instance.LastRoleSummaryData.Traitors.IsNullOrEmpty() )
				Traitors.AddChild( new RoleList() { Role = new Traitor(), Players = GeneralMenu.Instance.LastRoleSummaryData.Traitors } );
		}

		Empty.Enabled( !Innocents.Children.Any() && !Detectives.Children.Any() && !Traitors.Children.Any() );
	}
}
