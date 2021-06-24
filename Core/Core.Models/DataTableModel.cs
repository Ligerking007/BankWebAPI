using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
	public class DataTableModel
	{
		public class GridAjaxPostModel
		{
			// properties are not capital due to json mapping
			public int GridDraw { get; set; }
			public int GridStart { get; set; }
			public int GridLength { get; set; }
			public List<Column> GridColumns { get; set; }
			public Search GridSearch { get; set; }
			public List<Order> GridOrder { get; set; }
		}

		public class Column
		{
			public string data { get; set; }
			public string name { get; set; }
			public bool searchable { get; set; }
			public bool orderable { get; set; }
			public Search search { get; set; }
		}

		public class Search
		{
			public string value { get; set; }
			public string regex { get; set; }
		}

		public class Order
		{
			public int column { get; set; }
			public string dir { get; set; }
		}

		public class Result
		{
			public int draw { get; set; }
			public int recordsTotal { get; set; }
			public int recordsFiltered { get; set; }
		}
	}
}
