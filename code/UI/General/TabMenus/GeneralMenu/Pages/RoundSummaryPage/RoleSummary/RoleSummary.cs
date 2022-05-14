using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class RoleSummary : Panel
{
	public static RoleSummary Instance;

	private Panel Empty { get; init; }
	private Panel Innocents { get; init; }
	private Panel Detectives { get; init; }
	private Panel Traitors { get; init; }

	public RoleSummary()
	{
		Instance = this;
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
				Innocents.AddChild( new RoleList( new Innocent(), GeneralMenu.Instance.LastRoleSummaryData.Innocents ) );

			if ( !GeneralMenu.Instance.LastRoleSummaryData.Detectives.IsNullOrEmpty() )
				Detectives.AddChild( new RoleList( new Detective(), GeneralMenu.Instance.LastRoleSummaryData.Detectives ) );

			if ( !GeneralMenu.Instance.LastRoleSummaryData.Traitors.IsNullOrEmpty() )
				Traitors.AddChild( new RoleList( new Traitor(), GeneralMenu.Instance.LastRoleSummaryData.Traitors ) );
		}

		Empty.Enabled( !Innocents.Children.Any() && !Detectives.Children.Any() && !Traitors.Children.Any() );
	}
}
