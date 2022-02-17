using System;

namespace TTT.Items
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public class PrecachedAttribute : Attribute
	{
		public readonly string[] PrecachedFiles;

		public PrecachedAttribute( params string[] precachedFiles ) : base()
		{
			PrecachedFiles = precachedFiles;
		}
	}
}
