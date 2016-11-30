using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRpg
{
	public class Species
	{
		public uint   Id          { get; set; }
		public string Name        { get; set; }
		public string NameSub     { get; set; }
		public string Description { get; set; }
		public bool   IsPlayable  { get; set; }
	}
}
