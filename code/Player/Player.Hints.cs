using Sandbox;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 20480f;
	private const float MaxHintRadius = 100f;

	private UI.EntityHintPanel _currentHintPanel;
	private IEntityHint _currentHint;

	private void DisplayWorldHints( Vector2 screenSize )
	{
		foreach ( var ent in Entity.FindInSphere( Position, MaxHintRadius ) )
		{
			if ( ent is not Carriable && ent is not TTT.Ammo )
				continue;

			var trace = Trace.Ray( Position, ent.Position )
				.WorldOnly()
				.Run();

			if ( trace.Hit )
				continue;

			var pos = ent.Position.ToScreen( screenSize );
			if ( !pos.HasValue )
				continue;

			var label = DisplayInfo.For( ent ).Name;
			Render.Draw2D.FontFamily = "Poppins";
			Render.Draw2D.FontWeight = 1000;
			Render.Draw2D.FontSize = 14;

			var textRect = Render.Draw2D.TextSize( pos.Value, label );

			Render.Draw2D.BlendMode = BlendMode.Normal;
			Render.Draw2D.Color = Color.Black.WithAlpha( 0.7f );
			Render.Draw2D.BoxWithBorder( textRect.Expand( 16, 12 ), 2.0f, Color.Black.WithAlpha( 0.2f ), new Vector4( 4.0f ) );

			Render.Draw2D.Color = Color.White;
			Render.Draw2D.Text( pos.Value, label );
		}
	}

	private void DisplayEntityHints()
	{
		if ( !CurrentPlayer.IsFirstPersonMode )
		{
			DeleteHint();
			return;
		}

		var hint = FindHintableEntity();
		if ( hint is null || !hint.CanHint( CurrentPlayer ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( CurrentPlayer );
			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( CurrentPlayer );
		_currentHintPanel.Parent = UI.HintDisplay.Instance;
		_currentHintPanel.Enabled( true );

		_currentHint = hint;
	}

	private void DeleteHint()
	{
		_currentHintPanel?.Delete( true );
		_currentHintPanel = null;
		UI.FullScreenHintMenu.Instance?.Close();

		_currentHint = null;
	}

	private IEntityHint FindHintableEntity()
	{
		var trace = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * MaxHintDistance )
				.HitLayer( CollisionLayer.Debris )
				.Ignore( CurrentPlayer )
				.UseHitboxes()
				.Run();

		if ( trace.Entity is IEntityHint hint && trace.StartPosition.Distance( trace.EndPosition ) <= hint.HintDistance )
			return hint;

		return null;
	}
}
