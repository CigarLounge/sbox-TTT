using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class DNAMenu : Panel
{
	private readonly List<Panel> _samples = new();

	private readonly Panel _sampleContainer;
	private readonly Panel _infoContainer;
	private readonly Button _button;
	private readonly Checkbox _checkbox;

	public DNAMenu()
	{
		StyleSheet.Load( "/ui/player/rolemenu/dna/DNAMenu.scss" );

		var wrapper = Add.Panel();

		_sampleContainer = wrapper.Add.Panel( "sample-container" );
		_infoContainer = wrapper.Add.Panel( "info-container" );

		_infoContainer.AddClass( "rounded" );
		_infoContainer.AddClass( "background-color-primary" );
		var currentCharge = _infoContainer.Add.Label( "95.45%", "charge" );
		var status = _infoContainer.Add.Label( "CHARGING", "charge-status" );
		_button = _infoContainer.Add.ButtonWithIcon( "Scan", "radar", "scan-button" );

		_checkbox = new Checkbox();
		_checkbox.LabelText = "Auto-repeat";
		_infoContainer.AddChild( _checkbox );

		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
		AddSample();
	}

	public override void Tick()
	{
		_button.SetClass( "inactive", _checkbox.Checked );

		if ( Local.Pawn is not Player player )
			return;

		if ( player.ActiveChild is not DNAScanner scanner )
			return;

		if ( scanner.DNACollected.Count == 0 )
		{

		}
	}

	public void AddSample()
	{
		_samples.Add( new Sample() );
		_sampleContainer.AddChild( new Sample() );
	}

	public class Sample : Panel
	{
		public Sample()
		{
			AddClass( "rounded" );
			AddClass( "background-color-primary" );

			var deleteButton = Add.Icon( "cancel", "delete-button" );
			deleteButton.AddEventListener( "onclick", () =>
			{

			} );


			Add.Button( "Sample #1 - 6:11 - Corpse" );
		}
	}
}
