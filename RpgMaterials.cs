namespace XenoRPGLib
{
	// Material types
	// • Affects item quality and durability
	public enum MaterialType
	{
		Wood,
		Bone,
		Stone,
		Glass,
		Copper,
		Tin,
		Bronze,
		Iron,
		Steel,
		Silver,
		Gold,
		Ceramic,
	}

	public class RpgMaterial
	{
		public MaterialType m_type;
		public int m_iStrength; // Hardness?
		public int m_iDurability;
	}
}