using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntryTagger : Panel
{
	private readonly Client _client;


	public ScoreboardEntryTagger( Panel parent, Client client ) : base( parent )
	{
		_client = client;
	}

	public void Update()
	{

	}

	public void OnClick()
	{
	}

	private void HandleExpand()
	{
	}
}
