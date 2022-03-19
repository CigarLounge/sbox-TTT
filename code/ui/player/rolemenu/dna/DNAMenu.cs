using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class DNAMenu : Panel
{
	private readonly Panel _sampleContainer;
	private readonly Panel _infoContainer;

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
		var scanButton = _infoContainer.Add.ButtonWithIcon( "Scan", "radar", "scan-button" );

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
