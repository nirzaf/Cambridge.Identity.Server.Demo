using System;

namespace Cambridge.Demo.ResourceServer
{
	public class Document
	{
		public string Name { get; set; }

		public string Content { get; set; }

		public Guid Owner { get; set; }
	}
}