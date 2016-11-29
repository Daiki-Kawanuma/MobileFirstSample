using System;
namespace MobileFirstSample
{
	public class CommandItem
	{
		public string CommandName { get; set; }
		public string Image { get; set; }
		public Action Command { get; set; }
	}
}
