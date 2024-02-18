using System;
using System.Collections.Generic;

namespace Webshop.Models
{
	public class ProductExtended : Product
	{
		public List<Stock> Stock { get; set; }
	}
}