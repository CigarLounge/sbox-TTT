using System;
using System.Collections.Generic;
using System.Reflection;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Settings;

namespace TTT.UI.Menu
{
	[UseTemplate]
	public partial class SettingsPage : Panel
	{
		private Button ServerSettingsButton { get; set; }

		public SettingsPage()
		{
			if ( Local.Client.HasPermission( "serversettings" ) )
			{
				ServerSettingsButton.RemoveClass( "inactive" );
			}
		}

		public void GoToClientSettings()
		{
			TTTMenu.Instance.AddPage( new ClientSettingsPage() );
		}

		public void GoToServerSettings()
		{
			// Call to server which sends down server data and then adds the ServerSettingsPage.
			SettingFunctions.RequestServerSettings();
		}

		public static void CreateSettings( TabContainer tabContainer, Settings.Settings settings, Type settingsType = null )
		{
			settingsType ??= settings.GetType();

			Type baseSettingsType = typeof( Settings.Settings );

			if ( settingsType != baseSettingsType )
			{
				CreateSettings( tabContainer, settings, settingsType.BaseType );
			}

			PropertyInfo[] properties = settingsType.GetProperties();
			string nsp = typeof( Settings.Categories.Round ).Namespace;

			foreach ( PropertyInfo propertyInfo in properties )
			{
				if ( propertyInfo.DeclaringType.BaseType != baseSettingsType && settingsType != baseSettingsType || !propertyInfo.PropertyType.Namespace.Equals( nsp ) )
				{
					continue;
				}

				string categoryName = propertyInfo.Name;
				object propertyObject = propertyInfo.GetValue( settings );

				if ( propertyObject == null )
				{
					continue;
				}

				Panel tab = new();
				tab.AddClass( "root" );

				foreach ( PropertyInfo subPropertyInfo in propertyInfo.PropertyType.GetProperties() )
				{
					foreach ( object attribute in subPropertyInfo.GetCustomAttributes() )
					{
						if ( attribute is not SettingAttribute settingAttribute )
						{
							continue;
						}

						string propertyName = subPropertyInfo.Name;

						switch ( settingAttribute )
						{
							case SwitchSettingAttribute:
								CreateSwitchSetting( tab, settings, categoryName, propertyName, propertyObject );

								break;

							case InputSettingAttribute:
								CreateInputSetting( tab, settings, categoryName, propertyName, propertyObject );

								break;

							case DropdownSettingAttribute:
								CreateDropdownSetting( tab, settings, categoryName, propertyName, propertyObject, propertyInfo, subPropertyInfo );

								break;
						}

						break;
					}

					tab.Add.LineBreak();
				}

				tabContainer.AddTab( tab, categoryName );
			}
		}

		private static void CreateSwitchSetting( Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject )
		{
			Checkbox checkbox = new();
			checkbox.SetContent( propertyName );
			checkbox.Parent = parent;
			checkbox.Checked = Utils.GetPropertyValue<bool>( propertyObject, propertyName );
			checkbox.AddEventListener( "onchange", ( panelEvent ) =>
			 {
				 UpdateSettingsProperty( settings, propertyObject, propertyName, checkbox.Checked );
			 } );
		}

		private static void CreateInputSetting( Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject )
		{
			CreateSettingsEntry( parent, propertyName, Utils.GetPropertyValue( propertyObject, propertyName ).ToString(), ( value ) =>
			{
				UpdateSettingsProperty( settings, propertyObject, propertyName, value );
			} );
		}

		private static void CreateDropdownSetting( Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject, PropertyInfo propertyInfo, PropertyInfo subPropertyInfo )
		{
			parent.Add.Panel( categoryName.ToLower() );
			parent.Add.Label( propertyName, "h3" );

			DropDown dropdownSelection = new( parent );

			foreach ( PropertyInfo possibleDropdownPropertyInfo in propertyInfo.PropertyType.GetProperties() )
			{
				foreach ( object possibleDropdownAttribute in possibleDropdownPropertyInfo.GetCustomAttributes() )
				{
					if ( possibleDropdownAttribute is DropdownOptionsAttribute dropdownOptionsAttribute && dropdownOptionsAttribute.DropdownSetting.Equals( subPropertyInfo.Name ) )
					{
						dropdownSelection.AddEventListener( "onchange", ( e ) =>
						 {
							 UpdateSettingsProperty( settings, propertyObject, propertyName, dropdownSelection.Selected.Value );
						 } );

						foreach ( KeyValuePair<string, object> keyValuePair in Utils.GetPropertyValue<Dictionary<string, object>>( propertyObject, possibleDropdownPropertyInfo.Name ) )
						{
							dropdownSelection.Options.Add( new Option( new string( keyValuePair.Key ), keyValuePair.Value ) );
						}
					}
				}
			}
		}

		private static void UpdateSettingsProperty<T>( Settings.Settings settings, object propertyObject, string propertyName, T value )
		{
			Utils.SetPropertyValue( propertyObject, propertyName, value );

			if ( Gamemode.Game.Instance.Debug )
			{
				Log.Debug( $"Set {propertyName} to {value}" );
			}

			if ( settings is ServerSettings serverSettings )
			{
				SettingFunctions.SendSettingsToServer( serverSettings );
			}
			else
			{
				Event.Run( Events.TTTEvent.Settings.Change );
			}
		}

		public static TextEntry CreateSettingsEntry<T>( Panel parent, string title, T defaultValue, Action<T> OnSubmit = null, Action<T> OnChange = null )
		{
			Label textLabel = parent.Add.Label( new string( title ) );

			TextEntry textEntry = parent.Add.TextEntry( defaultValue.ToString() );

			textEntry.AddEventListener( "onsubmit", ( panelEvent ) =>
			 {
				 try
				 {
					 textEntry.Text.TryToType( typeof( T ), out object value );

					 if ( value.ToString().Equals( textEntry.Text ) )
					 {
						 T newValue = (T)value;

						 OnSubmit?.Invoke( newValue );

						 defaultValue = newValue;
					 }
				 }
				 catch ( Exception ) { }

				 textEntry.Text = defaultValue.ToString();
			 } );

			textEntry.AddEventListener( "onchange", ( panelEvent ) =>
			 {
				 try
				 {
					 if ( string.IsNullOrEmpty( textEntry.Text ) )
					 {
						 return;
					 }

					 textEntry.Text.TryToType( typeof( T ), out object value );

					 if ( value.ToString().Equals( textEntry.Text ) )
					 {
						 T newValue = (T)value;

						 OnChange?.Invoke( newValue );

						 defaultValue = newValue;
					 }
				 }
				 catch ( Exception ) { }

				 textEntry.Text = defaultValue.ToString();
			 } );

			return textEntry;
		}
	}
}
