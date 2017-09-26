namespace XRpg
{
	// DB table: ItemTypes
	public class ItemTypeClass
	{
		public ulong  Value       { get; set; }
		public string Name        { get; set; }
		public ushort IconIdx     { get; set; }
		public string Description { get; set; }

		public string ValueAsHex => "0x" + Value.ToString( "X16" );
	}
}
