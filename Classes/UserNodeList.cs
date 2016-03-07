using System;
using System.Collections.Generic;

namespace Thinkgate.Classes
{
    [Serializable()]
	public class UserNodeList
	{
		public string NodeName { get; set; }
		public string NodeAliasPath { get; set; }
		public string Description { get; set; }
		public string NodeId { get; set; }
		public bool IsEditable { get; set; }
		public int DocumentId { get; set; }
		public IEnumerable<int> AssociatedCurriculaIds { get; set; }
		public IEnumerable<int> AssociatedStandardIds { get; set; }
		public int ResourceType { get; set; }
		public int ResourceSubType { get; set; }
		public DateTime LastModified { get; set; }
	    public DateTime ExpirationDate { get; set; }

        public int StudentCount { get; set; }
        public string ClassName { get; set; }
        public string FriendlyName { get; set; }

        public DateTime CreatedDate { get; set; }

		public UserNodeList()
		{
			AssociatedCurriculaIds = new List<int>();
			AssociatedStandardIds = new List<int>();
		}
	}
}