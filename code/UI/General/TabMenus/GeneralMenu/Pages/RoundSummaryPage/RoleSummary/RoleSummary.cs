using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleSummary : Panel
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

		if ( GeneralMenu.Instance == null )
			return;

		if ( !GeneralMenu.Instance.Data.InnocentClientIds.IsNullOrEmpty() )
			Innocents.AddChild( new RoleList( new Innocent(), GeneralMenu.Instance.Data.InnocentClientIds ) );

		if ( !GeneralMenu.Instance.Data.DetectiveClientIds.IsNullOrEmpty() )
			Detectives.AddChild( new RoleList( new Detective(), GeneralMenu.Instance.Data.DetectiveClientIds ) );

		if ( !GeneralMenu.Instance.Data.TraitorClientIds.IsNullOrEmpty() )
			Traitors.AddChild( new RoleList( new Traitor(), GeneralMenu.Instance.Data.TraitorClientIds ) );

		Empty.Enabled( !Innocents.Children.Any() && !Detectives.Children.Any() && !Traitors.Children.Any() );
	}
}
