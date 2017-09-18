namespace XRpg
{
	// DB table: ItemTypes
	public class ItemType
	{
		public string Name    { get; set; }
		public ulong  Value   { get; set; }
		public ushort IconIdx { get; set; }

		public string ValueAsHex => Value.ToString( "X16" );
	}
}
