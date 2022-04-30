using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleSummary : Panel
{
	public static RoleSummary Instance;

	private Panel Innocents { get; set; }
	private Panel Detectives { get; set; }
	private Panel Traitors { get; set; }

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

		if ( !GeneralMenu.Instance.Data.Innocents.IsNullOrEmpty() )
			Innocents.AddChild( new RoleList( new Innocent(), GeneralMenu.Instance.Data.Innocents ) );

		if ( !GeneralMenu.Instance.Data.Detectives.IsNullOrEmpty() )
			Detectives.AddChild( new RoleList( new Detective(), GeneralMenu.Instance.Data.Detectives ) );

		if ( !GeneralMenu.Instance.Data.Traitors.IsNullOrEmpty() )
			Traitors.AddChild( new RoleList( new Traitor(), GeneralMenu.Instance.Data.Traitors ) );
	}
}
