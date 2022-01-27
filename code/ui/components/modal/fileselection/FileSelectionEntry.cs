using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public partial class FileSelectionEntry : Panel
	{
		public readonly Label FileNameLabel;

		public readonly IconPanel FileTypeIcon;

		public bool IsFolder { get; set; } = false;

		private FileSelection _fileSelection;

		public FileSelectionEntry( Panel parent ) : base( parent )
		{
			FileTypeIcon = Add.Icon( null, "filetype" );
			FileNameLabel = Add.Label( "", "filename" );
		}

		public void SetFileSelection( FileSelection fileSelection )
		{
			_fileSelection = fileSelection;
		}

		protected override void OnClick( MousePanelEvent e )
		{
			_fileSelection?.OnSelect( this );
		}

		protected override void OnDoubleClick( MousePanelEvent e )
		{
			_fileSelection?.OnConfirm( this );
		}
	}
}

namespace Sandbox.UI.Construct
{
	using TTT.UI;

	public static class FileSelectionEntryConstructor
	{
		public static FileSelectionEntry FileSelectionEntry( this PanelCreator self, string text = "", string icon = null )
		{
			FileSelectionEntry fileSelectionEntry = new( self.panel );
			fileSelectionEntry.FileNameLabel.Text = text;
			fileSelectionEntry.FileTypeIcon.Text = icon;

			return fileSelectionEntry;
		}
	}
}
