using System;
using System.Collections.Generic;

namespace Cambridge.Demo.ResourceServer
{
	public static class DocumentRepository
	{
		public static List<Document> FreeDocuments => new List<Document>()
		{
			new Document
			{
				Name = "Welcome Document",
				Content = "Welcome to dark side of Programming"
			},
			new Document
			{
				Name = "Dummy Note",
				Content = "I like C# more than my wife! ( I'm joking :-) )"
			}
		};

		public static List<Document> PersonalDocuments => new List<Document>()
		{
			new Document
			{
				Name = "Software Document",
				Owner = Guid.Parse("d860efca-22d9-47fd-8249-791ba61b07c7"),
				Content = "double result = param / 0"
			},
			new Document
			{
				Name = "Employer Document",
				Owner = Guid.Parse("24f56fef-5cfc-44e0-b77a-8bae834ed030"),
				Content = "Who do you want to fire today :-) ?"
			}
		};
	}
}