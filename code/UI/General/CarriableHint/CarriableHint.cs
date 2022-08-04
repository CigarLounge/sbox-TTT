using Sandbox;
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
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( player.ActiveCarriable is not null );
		if ( !this.IsEnabled() )
			return;

		var primaryHintEnabled = !string.IsNullOrEmpty( player.ActiveCarriable.PrimaryAttackHint );
		PrimaryAttackPanel.Enabled( primaryHintEnabled );
		if ( primaryHintEnabled )
			PrimaryAttackLabel.Text = player.ActiveCarriable.PrimaryAttackHint;

		var secondaryHintEnabled = !string.IsNullOrEmpty( player.ActiveCarriable.SecondaryAttackHint );
		SecondaryAttackPanel.Enabled( secondaryHintEnabled );
		if ( secondaryHintEnabled )
			SecondaryAttackLabel.Text = player.ActiveCarriable.SecondaryAttackHint;
	}
}
