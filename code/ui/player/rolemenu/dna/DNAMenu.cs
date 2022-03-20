using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class DNAMenu : Panel
{
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

		// TODO: Remove.
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
		AddSample();
		AddSample();
		AddSample();
		AddSample();
	}

	public override void Tick()
	{
		_button.SetClass( "inactive", _checkbox.Checked );
	}

	public void AddSample()
	{
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
