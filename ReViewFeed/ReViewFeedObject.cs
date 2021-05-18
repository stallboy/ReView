using System;
using System.Collections.Generic;
using System.Text;

namespace ReView
{
	public class ReViewFeedObject
	{
		/// <summary>
		/// Debug type is used to determine if ReViewFeedObject is a track or an item. Type cannot be changed once ReViewFeedObject is created.
		/// </summary>
		public enum EDebugType
		{
			Track,
			Item,
			GenericItem
		};

		/// <summary>
		/// Construct new ReViewFeedObject with given parent (can be null if type is track), name, time when created and type.
		/// If type is track no log output can be added for it.
		/// Throws InvalidOperationException if parent is null and type is item / generic item.
		/// </summary>
		/// <param name="parent">Parent object, if type is item / generic item then parent is the containing track. If track then parent is the parent track or null if this is the root.</param>
		/// <param name="debugName">Name for this debug object, will be track or item / generic item name.</param>
		/// <param name="time">Time when this debug object starts.</param>
		/// <param name="type">Either track or item / generic item, only items can contain log output.</param>
		public ReViewFeedObject(ReViewFeedObject parent, string debugName, int time, EDebugType type)
		{
			this.parent = parent;
			if (type == EDebugType.Item)
			{
				if (parent == null)
				{
					throw new InvalidOperationException("Cannot have null parent for item type.");
				}
				debugID = ReViewFeedManager.Instance.AddItem(parent.DebugID, time, debugName);
			}
			else if (type == EDebugType.GenericItem)
			{
				if (parent == null)
				{
					throw new InvalidOperationException("Cannot have null parent for item type.");
				}
				debugID = ReViewFeedManager.Instance.AddGenericItem(parent.DebugID, time, debugName);
			}
			else if (type == EDebugType.Track)
			{
				debugID = ReViewFeedManager.Instance.AddTrack(parent == null ? -1 : parent.DebugID, debugName);
			}
			else
			{
				throw new InvalidOperationException("Invalid type!");
			}
		}

		/// <summary>
		/// End this debug object, cannot append more log messages after this
		/// </summary>
		/// <param name="time">Time to end this debug object</param>
		public void EndBlock(int time)
		{
			if (DebugID >= 0)
			{
				// When ending item we're setting debugID to -1 to prevent further logging
				ReViewFeedManager.Instance.EndItem(debugID, time);
				debugID = -1;
			}
		}

		/// <summary>
		/// Append log entry, flags are used to color code messages and items they are added to.
		/// ReViewTool UI can specify different colors per each bit flag.
		/// If this block has been terminated with EndBlock log will not be appended.
		/// </summary>
		/// <param name="message">Message (utf8) to append for this item.</param>
		/// <param name="time">Time for this addition.</param>
		/// <param name="flags">Flags for this message, will also set the flag for the containing item.</param>
		public void Log(String message, int time, uint flags)
		{
			if (DebugID >= 0)
			{
				ReViewFeedManager.Instance.AppendLog(debugID, time, flags, message);
			}
		}

		/// <summary>
		/// DebugID created for this ReViewFeedObject
		/// </summary>
		public long DebugID
		{
			get 
			{ 
				return debugID; 
			}
		}
		
		private ReViewFeedObject parent = null;
		private long debugID = -1;
	}
}
