using System;
using System.Linq;
using Sandbox;

namespace TTT.Items
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class ItemAttribute : Attribute
	{
		public ItemAttribute() : base()
		{

		}
	}

	public interface IItem
	{
		static string ITEM_TAG => "TTT_ITEM";
		string LibraryTitle => Library.GetAttributes<LibraryAttribute>().First().Title;
		void Delete();
	}
}
