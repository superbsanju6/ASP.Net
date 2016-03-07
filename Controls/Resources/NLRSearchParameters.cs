using System.Collections.Generic;

namespace Thinkgate.Controls.Resources
{
	internal class NLRSearchParameters
	{
		public string titleAndDescriptionSearchText { get; set; }
		public string created { get; set; }
		public string creator { get; set; }
		public string description { get; set; }
		//public int intID { get; set; }
		public string publisher { get; set; }
		public string title { get; set; }
		public string url { get; set; }
		public string usageRightsURL { get; set; }
		public string keywords { get; set; }
		public List<string> educationalUse { get; set; }
		public string author { get; set; }
		public List<string> interactivityType { get; set; }
		public string accessibilityFeature { get; set; }
		public string accessibilityHazard { get; set; }
		public string copyrightHolder { get; set; }
		public string bookFormat { get; set; }
		public string learningResourceType { get; set; }
		public string accessibilityControl { get; set; }
		public string inLanguage { get; set; }
		public string typicalAgeRange { get; set; }
		public List<string> educationalRole { get; set; }

	}
}