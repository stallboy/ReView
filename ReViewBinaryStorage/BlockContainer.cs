using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewBinaryStorage
{
	public enum BlockType
	{
		Default = 0,
		Generic,
		Count
	}

	public class BlockContainer
	{
		public BlockContainer(BlockContainer parent, BlockType type)
		{
			Parent = parent;
			Type = type;

			children = new ObservableCollection<BlockContainer>();
			Children = new ReadOnlyCollection<BlockContainer>(children);
		}

		public BlockContainer Parent
		{
			get
			{
				return parent;
			}
			set
			{
				if (parent == value)
				{
					return; // No change
				}

				if (parent != null)
				{
					parent.RemoveChild(this);
				}

				parent = value;

				parent.AddChild(this);
			}
		}

		private void RemoveChild(BlockContainer child)
		{
			children.Remove(child);
		}

		private void AddChild(BlockContainer child)
		{
			if (!Children.Contains(child))
			{
				children.Add(child);
			}
		}

		public BlockType Type
		{
			get;
			set;
		}

		public ReadOnlyCollection<BlockContainer> Children
		{
			get;
			set;
		}

		private BlockContainer parent;
		private ObservableCollection<BlockContainer> children;
	}
}
