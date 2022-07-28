using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CarriableHint : Panel
{
	public static CarriableHint Instance;

	private Panel PrimaryAttackPanel { get; set; }
	private Panel SecondaryAttackPanel { get; set; }

	private Label PrimaryAttackLabel { get; set; }
	private Label SecondaryAttackLabel { get; set; }

	public CarriableHint()
	{
		Instance = this;
		Hide();
	}

	public void Show( string primaryAttackHint, string secondaryAttackHint )
	{
		if ( !string.IsNullOrEmpty( primaryAttackHint ) )
		{
			PrimaryAttackLabel.Text = primaryAttackHint;
			PrimaryAttackPanel.Enabled( true );
		}

		if ( !string.IsNullOrEmpty( secondaryAttackHint ) )
		{
			SecondaryAttackLabel.Text = secondaryAttackHint;
			SecondaryAttackPanel.Enabled( true );
		}
	}

	public void Hide()
	{
		PrimaryAttackPanel.Enabled( false );
		SecondaryAttackPanel.Enabled( false );
	}
}
