using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Text.RegularExpressions;

namespace ReView
{
	public partial class SequencerControl : UserControl, ViewportPanel, OverviewInterface, INotifyPropertyChanged
	{
		public SequencerControl()
		{
			InitializeComponent();

			SuspendNotifications = false;

			logBrushes = new Brush[33]; // No flags + 32 flags
			logTextBrushes = new Brush[33]; // No flags + 32 flags
			for (int i = 0; i < 33; i++)
			{
				logBrushes[i] = new SolidBrush(Color.White);
				logTextBrushes[i] = new SolidBrush(Color.Black);
			}

			UpdateRects();

			this.Resize += new EventHandler(Tracker_Resize);

			this.MouseMove += new System.Windows.Forms.MouseEventHandler(Tracker_MouseMove);
			this.MouseHover += new System.EventHandler(Tracker_MouseHover);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(Tracker_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(Tracker_MouseUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(Tracker_MouseWheel);
			this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(Tracker_MouseDoubleClick);
			this.KeyDown += new KeyEventHandler(Tracker_KeyDown);
	
			MinimumSize = new Size(64, 64);

			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

			layoutUpdateEvent = new AutoResetEvent(false);
			layoutRefreshThread = new Thread(new ThreadStart(UpdateLayout));
			layoutRefreshThread.IsBackground = true;
			layoutRefreshThread.Start();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			layoutUpdateRunning = false;
			layoutUpdateEvent.Set();
			layoutRefreshThread.Join();
			layoutRefreshThread = null;
		}

		#region CustomScrollablePanel

		public HorizontalMargin HorizontalScrollBarMargins 
		{
			get { return new HorizontalMargin((int)(splitterFraction * this.Bounds.Width + SplitterWidth / 2), 0); }
			set { }
		}

		public VerticalMargin VerticalScrollBarMargins 
		{
			get { return new VerticalMargin(0, 0); }
			set { } 
		}

		public event ScrollBarMarginsChanged OnScrollBarMarginsChanged;

		#endregion

		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}

		#region General

		protected float TimeToPixels(int time)
		{
			return ((float)time / 1000.0f * TimePixelRatio);
		}

		protected int PixelsToTime(float pixels)
		{
			return (int)(pixels / TimePixelRatio * 1000.0f);
		}

		private void UpdatePanOffset(int timeNow)
		{
			PanOffset = new Point(-(int)TimeToPixels(timeNow), PanOffset.Y);
		}

		private bool SuspendNotifications
		{
			get;
			set;
		}

		public void ForcePanOffset(Point offset)
		{
			SuspendNotifications = true;

			PanOffset = offset;

			SuspendNotifications = false;
		}

		public void ForceTimePixelRatio(float ratio)
		{
			SuspendNotifications = true;

			TimePixelRatio = ratio;

			SuspendNotifications = false;
		}

		/// <summary>
		/// Update rectangles for different drawing areas (track header, track area and splitter)
		/// Will also update visible track count and overview bitmap as they are both dependent on the rectangles
		/// </summary>
		private void UpdateRects()
		{
			trackHeaderRect.X = 0;
			trackHeaderRect.Y = 0;
			trackHeaderRect.Width = (int)(splitterFraction * this.Bounds.Width) - splitterWidth / 2;
			trackHeaderRect.Height = this.Bounds.Height;

			trackAreaRect.X = (int)(splitterFraction * this.Bounds.Width) + splitterWidth / 2;
			trackAreaRect.Y = 0;
			trackAreaRect.Width = (int)((1.0 - splitterFraction) * this.Bounds.Width) - splitterWidth / 2;
			trackAreaRect.Height = this.Bounds.Height;

			splitterRect.X = (int)(splitterFraction * this.Bounds.Width) - splitterWidth / 2;
			splitterRect.Y = 0;
			splitterRect.Width = splitterWidth;
			splitterRect.Height = this.Bounds.Height;

			UpdateVisibleTrackCount();

			UpdateOverviewBitmap();
		}

		/// <summary>
		/// Draw given track onto the overview bitmap
		/// </summary>
		private void UpdateOverviewBitmap(Track track, Graphics g, int depth, ref int row)
		{
			if (track.Visible)
			{
				Color color = Color.White;
				Color textColor = Color.White;
				GetTrackColors(depth, ref color, ref textColor);

				using (Brush brush = new SolidBrush(color))
				{
					g.FillRectangle(brush, depth * 2, row * 2, 16, 2);
				}
				row++;
				if (!track.Collapsed)
				{
					// Recurse
					foreach (Track children in track.Children)
					{
						UpdateOverviewBitmap(children, g, depth + 1, ref row);
					}
				}
			}
		}

		/// <summary>
		/// Update overview bitmap based on visible tracks
		/// </summary>
		private void UpdateOverviewBitmap()
		{
			Bitmap oldOverviewBitmap = overviewBitmap;
			if (visibleTrackCount > 0)
			{
				overviewBitmap = new Bitmap(16, visibleTrackCount * 2);
				Graphics g = Graphics.FromImage(overviewBitmap);

				if (model != null)
				{
					try
					{
						int row = 0;
						foreach (Track track in model.Tracks)
						{
							UpdateOverviewBitmap(track, g, 0, ref row);
						}
					}
					catch (Exception exception)
					{
						System.Console.Out.WriteLine("Failed!" + exception.Message);
					}
				}
			}
			else
			{
				overviewBitmap = null;
			}

			if (oldOverviewBitmap != overviewBitmap && OnOverviewImageChanged != null)
			{
				OnOverviewImageChanged(overviewBitmap);
			}
		}

		/// <summary>
		/// Content height in pixels (visible tracks)
		/// </summary>
		public int ContentHeight
		{
			get { return visibleTrackCount * (trackHeight + trackMargin * 2); }
		}

		public int ViewportHeight
		{
			get { return Height; }
		}

		public void SetVerticalPanOffset(int y)
		{
			PanOffset = new Point(PanOffset.X, y);
		}

		/// <summary>
		/// Update visible track count recursive method, count is output param and expected to be reset before calling from outside
		/// </summary>
		private void UpdateVisibleTrackCount(Track track, ref int count)
		{
			if (track.Visible)
			{
				count++;
				if (!track.Collapsed)
				{
					// Recurse
					foreach (Track children in track.Children)
					{
						UpdateVisibleTrackCount(children, ref count);
					}
				}
			}
		}

		/// <summary>
		/// Update visible track count, also will update visibleStartTime and visibleEndTime based on the current zoom factor
		/// </summary>
		private void UpdateVisibleTrackCount()
		{
			visibleTrackCount = 0;
			if (model != null)
			{
				try
				{
					lock (model)
					{
						foreach (Track section in model.Tracks)
						{
							UpdateVisibleTrackCount(section, ref visibleTrackCount);
						}
					}
				}
				catch (Exception exception)
				{
					System.Console.Out.WriteLine("Failed!" + exception.Message);
				}
			}

			visibleStartTime = PixelsToTime(-trackAreaPanOffset.X);
			visibleEndTime = PixelsToTime(-trackAreaPanOffset.X + trackAreaRect.Width);
		}

		#endregion

		#region Event Handlers

		private void Tracker_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
			{
				// Left pressed (collapse selected track)
				if (SelectedTrack != null)
				{
					SelectedTrack.Collapsed = true;
				}
			}
			else if (e.KeyCode == Keys.Right)
			{
				// Right pressed (expand selected track)
				if (SelectedTrack != null)
				{
					SelectedTrack.Collapsed = false;
				}
			}
			else if (e.KeyCode == Keys.Down)
			{
				// Down pressed (cycle selection to next visible track)
				List<Track> visibleTracks = GetVisibleTracks();
				for (int i = 0; i < visibleTracks.Count; i++)
				{
					if (visibleTracks[i] == SelectedTrack && i < visibleTracks.Count - 1)
					{
						SelectedTrack = visibleTracks[i + 1];
						break;
					}
				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				// Down pressed (cycle selection to previous visible track)
				List<Track> visibleTracks = GetVisibleTracks();
				for (int i = 0; i < visibleTracks.Count; i++)
				{
					if (visibleTracks[i] == SelectedTrack && i > 0)
					{
						SelectedTrack = visibleTracks[i - 1];
						break;
					}
				}
			}

			PanOffset = trackAreaPanOffset; // Update PanOffset

			RequestLayoutUpdate();
		}

		/// <summary>
		/// Get list of visible tracks
		/// </summary>
		private List<Track> GetVisibleTracks()
		{
			List<Track> tracks = new List<Track>();
			foreach (Track track in model.Tracks)
			{
				GetVisibleTracks(tracks, track);
			}
			return tracks;
		}

		/// <summary>
		/// Fill list with visible tracks by recursing into children of the given track (recursive method)
		/// </summary>
		private void GetVisibleTracks(List<Track> tracks, Track current)
		{
			if (current.Visible)
			{
				tracks.Add(current);
				if (!current.Collapsed)
				{
					// Recurse
					foreach (Track children in current.Children)
					{
						GetVisibleTracks(tracks, children);
					}
				}
			}
		}

		private void Tracker_Resize(object sender, EventArgs ee)
		{
			// Force update splitter fraction
			SplitterFraction = splitterFraction;

			RequestLayoutUpdate();
		}

		private void Tracker_MouseHover(object sender, System.EventArgs e)
		{
		}

		private void Tracker_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Calculate new zoom factor (TimePixelRatio)
			int time = PixelsToTime(e.X - trackAreaRect.X - trackAreaPanOffset.X);
			float ratio = TimePixelRatio + Math.Sign(e.Delta) * Math.Min(512.0f, Math.Max(0.1f, Math.Abs(((float)e.Delta / (1.0f / TimePixelRatio * 6000.0f)))));

			if (ratio >= 1.0f)
			{
				ratio = (float)Math.Round(ratio * 10) / 10;
			}
			TimePixelRatio = (float)Math.Min(512.0f, Math.Max(1.0f / 8.0f, ratio));

			float newPos = TimeToPixels(time) + trackAreaRect.X + trackAreaPanOffset.X;
			PanOffset = new Point(PanOffset.X + (int)(e.X - newPos), PanOffset.Y);

			RequestLayoutUpdate();
		}

		private void Tracker_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (SplitterDrag)
			{
				// Dragging splitter
				this.Cursor = Cursors.VSplit;
				float newSplitterFraction = (float)e.X / (float)Bounds.Width;
				if (SplitterFraction != newSplitterFraction)
				{
					SplitterFraction = newSplitterFraction;
				}
			}
			else if (TrackAreaPan)
			{
				// Panning track area
				if ((e.X - dragStart.X) != 0.0f || (e.Y - dragStart.Y) != 0.0f)
				{
					PanOffset = new Point(trackAreaPanOffsetPrevious.X + (e.X - dragStart.X), trackAreaPanOffsetPrevious.Y + (e.Y - dragStart.Y));
				}
			}
			else
			{
				// Update cursor based on where it is
				if (trackAreaRect.Contains(e.X, e.Y) || trackHeaderRect.Contains(e.X, e.Y))
				{
					// Over track area or header area
					Selection selection = GetSelectionFromCoordinate(e.X, e.Y);
					if (selection != null && (selection.CollapseAreaHit || selection.Item != null))
					{
						// Something that can be selected under cursor
						Cursor = Cursors.Hand;
					}
					else
					{
						// Nothing that can be selected under cursor
						Cursor = Cursors.Default;
					}
				}
				else if (splitterRect.Contains(e.X, e.Y))
				{
					// On top of splitter
					this.Cursor = Cursors.VSplit;
				}
				else
				{
					// Nothing special under cursor
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void Tracker_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (!splitterRect.Contains(e.X, e.Y))
				{
					// Left button double clicked and we're not over splitter
					Selection selection = GetSelectionFromCoordinate(e.X, e.Y);
					if (selection != null && selection.Track != null && !selection.CollapseAreaHit)
					{
						// We're over a track so change the collapse/expanded state
						selection.Track.Collapsed = !selection.Track.Collapsed;
						SelectedItem = selection.Item;
						SelectedTrack = selection.Track;

						RequestLayoutUpdate();
					}
				}
			}
		}

		private void Tracker_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (splitterRect.Contains(e.X, e.Y))
				{
					// Starting to drag splitter
					SplitterDrag = true;
				}
				else
				{
					Selection selection = GetSelectionFromCoordinate(e.X, e.Y);
					if (selection != null)
					{
						if (selection.CollapseAreaHit)
						{
							// We hit collapse / expand button
							selection.Track.Collapsed = !selection.Track.Collapsed;
						}
						else
						{
							// We hit item and or track
							SelectedTrack = selection.Track;
							SelectedItem = selection.Item;
						}
					}
					if (trackAreaRect.Contains(e.X, e.Y) && selectedItem == null && (selectedTrack == null || !selectedTrack.HasChildren()))
					{
						// ReViewTimelineControl, trackarea (empty), or track (no item) hit
						TrackAreaPan = true;
						trackAreaPanOffsetPrevious.X = trackAreaPanOffset.X;
						trackAreaPanOffsetPrevious.Y = trackAreaPanOffset.Y;
						this.Cursor = Cursors.Hand;
					}

					RequestLayoutUpdate();
				}
			}
			if (e.Button == MouseButtons.Middle && trackAreaRect.Contains(e.X, e.Y))
			{
				// We're over track area and holding middle button -> Pan area
				TrackAreaPan = true;
				trackAreaPanOffsetPrevious.X = trackAreaPanOffset.X;
				trackAreaPanOffsetPrevious.Y = trackAreaPanOffset.Y;
			}

			// Remember drag start coordinates
			dragStart.X = e.X;
			dragStart.Y = e.Y;
		}

		private void Tracker_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (TrackAreaPan)
			{
				// If we were panning then lets reset cursor to default
				this.Cursor = Cursors.Default;
			}

			// Reset state
			SplitterDrag = false;
			TrackAreaPan = false;

			RequestLayoutUpdate();
		}

		#endregion

		#region Find Track/Item From Coordinate

		/// <summary>
		/// Helper to figure out rect for track or track header
		/// </summary>
		private int SetRectangle(ref Rectangle rectangle, int row, Track track, bool header)
		{
			int indentationLevel = 0;
			Track temp = track;
			while (temp.Parent != null)
			{
				temp = temp.Parent;
				indentationLevel++;
			}

			rectangle.X = (header ? trackHeaderRect.X : trackAreaRect.X) + TrackIndent * indentationLevel;
			rectangle.Y = (header ? trackHeaderRect.Y : trackAreaRect.Y) + trackAreaPanOffset.Y + row * (trackHeight + trackMargin * 2) + trackMargin;
			rectangle.Width = (header ? trackHeaderRect.Width + trackAreaRect.Width + splitterRect.Width : trackAreaRect.Width) - TrackIndent * indentationLevel;
			rectangle.Height = trackHeight;

			return indentationLevel;
		}

		/// <summary>
		/// Helper to figure out rect for item
		/// </summary>
		private void SetRectangle(ref Rectangle rectangle, int row, Item item)
		{
			rectangle.X = (int)(trackAreaRect.X + trackAreaPanOffset.X + TimeToPixels(item.StartTime));
			rectangle.Y = trackAreaRect.Y + trackAreaPanOffset.Y + row * (trackHeight + trackMargin * 2) + trackMargin;
			rectangle.Width = (int)Math.Max(2.0f, TimeToPixels(item.Length));
			rectangle.Height = trackHeight;
		}

		/// <summary>
		/// Helper to figure out rect for item
		/// </summary>
		private void SetRectangleF(ref RectangleF rectangle, int row, Item item)
		{
			rectangle.X = trackAreaRect.X + trackAreaPanOffset.X + TimeToPixels(item.StartTime);
			rectangle.Y = trackAreaRect.Y + trackAreaPanOffset.Y + row * (trackHeight + trackMargin * 2) + trackMargin;
			rectangle.Width = Math.Max(2.0f, TimeToPixels(item.Length));
			rectangle.Height = trackHeight;
		}

		/// <summary>
		/// Helper to create collapse / expand rect
		/// </summary>
		private void SetCollapseAreaRect(ref Rectangle rectangle)
		{
			rectangle.X += 8;
			rectangle.Y += rectangle.Height / 2 - 4;
			rectangle.Width = 8;
			rectangle.Height = 8;
		}

		/// <summary>
		/// Check if given track or some of its items are under given coordinate
		/// </summary>
		private Selection GetSelectionFromCoordinate(Track track, ref Rectangle rect, ref int row, int x, int y)
		{
			if (track.Visible)
			{
				SetRectangle(ref rect, row, track, true);
				if (rect.Contains(x, y))
				{
					bool collapseAreaHit = false;
					Rectangle collapseAreaRect = rect;
					SetCollapseAreaRect(ref collapseAreaRect);
					if (collapseAreaRect.Contains(x, y) && track.HasChildren())
						collapseAreaHit = true;

					// Found track -> Check items
					foreach (Item item in track.Items)
					{
						if (item.EndTime < VisibleStartTime)
							continue;
						if (item.StartTime > VisibleEndTime)
							break; // Item out of bounds (no need to continue iterate items in this track)

						SetRectangle(ref rect, row, item);
						if (rect.Contains(x, y))
						{
							return new Selection(track, item, collapseAreaHit);
						}
					}

					// Only track hit
					return new Selection(track, null, collapseAreaHit);
				}

				row++;

				if (!track.Collapsed)
				{
					// Recurse
					foreach (Track children in track.Children)
					{
						Selection selection = GetSelectionFromCoordinate(children, ref rect, ref row, x, y);
						if (selection != null)
							return selection;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Check what is under given coordinate (track / item / collapse button)
		/// </summary>
		private Selection GetSelectionFromCoordinate(int x, int y)
		{
			Selection selection = null;
			if (model != null)
			{
				try
				{
					Rectangle rect = new Rectangle();
					int row = 0;
					foreach (Track section in model.Tracks)
					{
						selection = GetSelectionFromCoordinate(section, ref rect, ref row, x, y);
						if (selection != null)
							break;
					}
				}
				catch (Exception exception)
				{
					System.Console.Out.WriteLine("Failed!" + exception.Message);
				}
			}
			return selection;
		}

		#endregion

		#region Paint

		/// <summary>
		/// Draw track header for given track
		/// </summary>
		private void DrawTrackHeader(Graphics g, Rectangle rect, Color backgroundColor, Color frontColor, Track track)
		{
			if (track == null || track.Visible)
			{
				Point fontPosition = new Point(rect.X + 20, rect.Y + rect.Height / 2);
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Near;
				format.LineAlignment = StringAlignment.Center;

				DrawingUtils.FillRectangle(g, rect, backgroundColor, frontColor, track == null ? null : track.Title, format, fontPosition);

				Rectangle collapseAreaRect = rect;
				SetCollapseAreaRect(ref collapseAreaRect);

				if (track != null && track.HasChildren())
				{
					Pen pen = new Pen(frontColor);
					g.DrawRectangle(pen, collapseAreaRect);
					g.DrawLine(pen, collapseAreaRect.X + 2, collapseAreaRect.Y + collapseAreaRect.Height / 2, collapseAreaRect.X + collapseAreaRect.Width - 2, collapseAreaRect.Y + collapseAreaRect.Height / 2);
					if (track.Collapsed)
						g.DrawLine(pen, collapseAreaRect.X + collapseAreaRect.Width / 2, collapseAreaRect.Y + 2, collapseAreaRect.X + +collapseAreaRect.Width / 2, collapseAreaRect.Y + collapseAreaRect.Height - 2);
					pen.Dispose();
				}
			}
		}

		/// <summary>
		/// Set new color for given log flag index
		/// </summary>
		public void SetFlagColor(int flagIndex, Color backColor)
		{
			Rectangle gradientRect = new Rectangle(0, 0, 1, trackHeight);
			if (logBrushes[flagIndex] != null)
			{
				logBrushes[flagIndex].Dispose();
			}
			if (logTextBrushes[flagIndex] != null)
			{
				logTextBrushes[flagIndex].Dispose();
			}
			logBrushes[flagIndex] = new System.Drawing.Drawing2D.LinearGradientBrush(gradientRect, backColor, DrawingUtils.BlendColors(backColor, Color.White, 0.2f), System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
			logTextBrushes[flagIndex] = backColor.GetBrightness() <= 0.5 ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);

			RequestLayoutUpdate();
		}

		/// <summary>
		/// Paint items for given track
		/// </summary>
		private void PaintTrackItems(Graphics g, Track track, int row)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;

			Rectangle itemHeightRect = new Rectangle(trackAreaRect.X, trackAreaRect.Y + trackAreaPanOffset.Y + row * (trackHeight + trackMargin * 2) + trackMargin, 1, trackHeight);

			using (Brush selectedItemBrush = new System.Drawing.Drawing2D.LinearGradientBrush(itemHeightRect, selectedItemBackColor, DrawingUtils.BlendColors(selectedItemBackColor, Color.White, 0.2f), System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal),
							activeItemBrush = new System.Drawing.Drawing2D.LinearGradientBrush(itemHeightRect, activeItemBackColor, DrawingUtils.BlendColors(activeItemBackColor, Color.White, 0.2f), System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
			{
				using (StringFormat format = new StringFormat())
				{
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					RectangleF newClip = new RectangleF();
					RectangleF itemRectangleF = new RectangleF();
					Rectangle itemRectangle = new Rectangle();

					int startItemIndex = GetFirstVisibleItemIndex(track, VisibleStartTime);
					int endItemIndex = GetLastVisibleItemIndex(track, VisibleEndTime);
					for (int i = startItemIndex; i <= endItemIndex && i < track.Items.Count; i++)
					{
						Item item = track.Items[i];

						if (item.Visible)
						{
							SetRectangleF(ref itemRectangleF, row, item);
							itemRectangleF.Y++;
							itemRectangleF.Height -= 4;

							itemRectangle.X = (int)itemRectangleF.X;
							itemRectangle.Y = (int)itemRectangleF.Y;
							itemRectangle.Width = (int)itemRectangleF.Width;
							itemRectangle.Height = (int)itemRectangleF.Height;

							int flagIndex = (item.Log.HasFlags() ? (item.Log.GetHighestFlagBit() + 1) : 0);

							// Draw content filled rectangle
							LinearGradientBrush itemBrush = (selectedItem == item	? selectedItemBrush :
																		item.Active ? activeItemBrush :
																					  logBrushes[flagIndex]) as LinearGradientBrush;

							itemBrush.ResetTransform();
							itemBrush.TranslateTransform(itemRectangleF.X, itemRectangleF.Y);
							itemBrush.ScaleTransform(itemRectangleF.Width, itemRectangleF.Height);

							g.FillRectangle(selectedItem == item ? selectedItemBrush : itemBrush, itemRectangleF);

							// Draw text
							if (itemRectangleF.Width > 5.0f && item.Title != null)
							{
								PointF textPosition = new PointF(itemRectangleF.X + itemRectangleF.Width / 2, itemRectangleF.Y + itemRectangleF.Height / 2);

								newClip.X = itemRectangleF.X + 2;
								newClip.Y = itemRectangleF.Y;
								newClip.Width = itemRectangleF.Width - 5;
								newClip.Height = itemRectangleF.Height;
								Region oldClip = g.Clip;

								newClip.Intersect(g.ClipBounds);
								g.SetClip(newClip);

								g.DrawString(item.Title, itemFont, logTextBrushes[flagIndex], textPosition, format);

								g.Clip = oldClip;
							}
						}
					}
				}
			}

			g.SmoothingMode = SmoothingMode.None;
		}

		/// <summary>
		/// Get back / front color for given track indentation level
		/// </summary>
		private void GetTrackColors(int level, ref Color backColor, ref Color frontColor)
		{
			switch (Math.Min(level, 3))
			{
				case 0:
					backColor = firstLevelTrackBackColor;
					frontColor = firstLevelTrackTextColor;
					break;
				case 1:
					backColor = secondLevelTrackBackColor;
					frontColor = secondLevelTrackTextColor;
					break;
				case 2:
					backColor = thirdLevelTrackBackColor;
					frontColor = thirdLevelTrackTextColor;
					break;
				case 3:
					backColor = fourthLevelTrackBackColor;
					frontColor = fourthLevelTrackTextColor;
					break;
			}
		}

		/// <summary>
		/// Paint track header for given track
		/// </summary>
		private bool PaintTrackHeader(PaintEventArgs e, Track track, ref Rectangle rect, ref int row)
		{
			if (track.Visible)
			{
				int indentationLevel = SetRectangle(ref rect, row, track, true);
				if (rect.Y > e.ClipRectangle.Y + e.ClipRectangle.Height) // Stop iterating as tracks not visible
					return false;

				Rectangle collapseAreaRect = rect;
				SetCollapseAreaRect(ref collapseAreaRect);

				Color color = Color.White;
				Color textColor = Color.White;
				GetTrackColors(indentationLevel, ref color, ref textColor);
				if (rect.Y + rect.Height >= e.ClipRectangle.Y)
					DrawTrackHeader(e.Graphics, rect, track == selectedTrack ? selectedColor : color, textColor, track);

				row++;

				if (!track.Collapsed)
				{
					// Recurse
					foreach (Track children in track.Children)
					{
						if (!PaintTrackHeader(e, children, ref rect, ref row))
							return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Paint track headers
		/// </summary>
		private void PaintTrackHeader(PaintEventArgs e)
		{
			if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
				return;

			Color backgroundColor = trackHeaderBackColor;
			using (Brush background = new SolidBrush(backgroundColor))
			{
				e.Graphics.FillRectangle(background, e.ClipRectangle);

				if (model != null)
				{
					try
					{
						Rectangle rect = new Rectangle();
						int row = 0;
						foreach (Track section in model.Tracks)
						{
							PaintTrackHeader(e, section, ref rect, ref row);
						}
					}
					catch (Exception exception)
					{
						Log.WriteLine("SequencerControl::PaintTrackHeader() -> Failed!");
						Log.WriteException(exception);
					}
				}
			}

			if (model == null || model.Tracks.Count == 0)
			{
				using (Brush emptyBrush = new System.Drawing.SolidBrush(Color.FromArgb(trackAreaBackColor.A, (byte)(trackAreaBackColor.R * 0.5), (byte)(trackAreaBackColor.G * 0.5), (byte)(trackAreaBackColor.B * 0.5))))
				{
					using (StringFormat format = new StringFormat())
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;
						String emptyText = "Track hierarchy";
						Point position = new Point(e.ClipRectangle.X + e.ClipRectangle.Width / 2, e.ClipRectangle.Y + e.ClipRectangle.Height / 2);
						e.Graphics.DrawString(emptyText, this.itemFont, emptyBrush, position, format);
					}
				}
			}
		}

		/// <summary>
		/// Get first visible item index for given track and time (to limit the item count to draw)
		/// </summary>
		private int GetFirstVisibleItemIndex(Track track, float visibleTime)
		{
			if (track.Items.Count == 0)
			{
				return 0;
			}
			int lowBound = 0;
			int highBound = track.Items.Count;
			int index = highBound / 2;
			while (true)
			{
				int newIndex = index;
				bool bVisible = (track.Items[index].EndTime > visibleTime);
				if (bVisible)
				{
					newIndex = (index - lowBound) / 2 + lowBound;
					highBound = index;
				} else {
					newIndex = (highBound - index) / 2 + index;
					lowBound = index;
				}
				if (newIndex == index)
				{
					break;
				}
				index = newIndex;
			}
			return index;
		}

		/// <summary>
		/// Get last visible item index for given track and time (to limit the item count to draw)
		/// </summary>
		private int GetLastVisibleItemIndex(Track track, float visibleTime)
		{
			if (track.Items.Count == 0)
			{
				return 0;
			}
			int lowBound = 0;
			int highBound = track.Items.Count;
			int index = highBound / 2;
			while (true)
			{
				int newIndex = index;
				bool bVisible = (track.Items[index].StartTime < visibleTime);
				if (bVisible)
				{
					newIndex = (highBound - index) / 2 + index;
					lowBound = index;
				}
				else
				{
					newIndex = (index - lowBound) / 2 + lowBound;
					highBound = index;
				}
				if (newIndex == index)
				{
					break;
				}
				index = newIndex;
			}
			return index;
		}

		/// <summary>
		/// Paint track and its items on the track area for given track
		/// </summary>
		private bool PaintTrackArea(PaintEventArgs e, Track track, ref Rectangle rect, ref int row)
		{
			if (track.Visible)
			{
				int indentationLevel = SetRectangle(ref rect, row, track, true);
				if (rect.Y > e.ClipRectangle.Y + e.ClipRectangle.Height) // Stop iterating as tracks not visible
					return false;

				Color color = Color.White;
				Color textColor = Color.White;
				GetTrackColors(indentationLevel, ref color, ref textColor);

				using (Brush trackBackColorBrush = new System.Drawing.SolidBrush(track == selectedTrack ? selectedColor : color))
				{
					e.Graphics.FillRectangle(trackBackColorBrush, rect);
				}

				if (track.HasChildren())
				{
					if (rect.Y + rect.Height >= e.ClipRectangle.Y)
						DrawTrackHeader(e.Graphics, rect, track == selectedTrack ? selectedColor : color, textColor, null);
				}
				else
				{
					PaintTrackItems(e.Graphics, track, row);
				}

				row++;

				if (!track.Collapsed)
				{
					// Recurse
					foreach (Track children in track.Children)
					{
						if (!PaintTrackArea(e, children, ref rect, ref row))
							return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Paint entire track area with all of its tracks and items
		/// </summary>
		private void PaintTrackArea(PaintEventArgs e)
		{
			if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
				return;

			using (Brush background = new System.Drawing.SolidBrush(trackAreaBackColor))
			{
				e.Graphics.FillRectangle(background, e.ClipRectangle);

				if (model != null)
				{
					try
					{
						Rectangle rect = new Rectangle();
						int row = 0;
						foreach (Track section in model.Tracks)
						{
							PaintTrackArea(e, section, ref rect, ref row);
						}
					}
					catch (Exception exception)
					{
						Log.WriteLine("SequencerControl::PaintTrackArea() -> Failed !");
						Log.WriteException(exception);
					}
				}

				if (model == null || model.Tracks.Count == 0)
				{
					using (Brush emptyBrush = new System.Drawing.SolidBrush(Color.FromArgb(trackAreaBackColor.A, (byte)(trackAreaBackColor.R * 0.5), (byte)(trackAreaBackColor.G * 0.5), (byte)(trackAreaBackColor.B * 0.5))))
					{
						using (StringFormat format = new StringFormat())
						{
							format.Alignment = StringAlignment.Center;
							format.LineAlignment = StringAlignment.Center;
							String emptyText = "Track items area";
							Point position = new Point(e.ClipRectangle.X + e.ClipRectangle.Width / 2, e.ClipRectangle.Y + e.ClipRectangle.Height / 2);
							e.Graphics.DrawString(emptyText, this.itemFont, emptyBrush, position, format);
						}
					}
				}

				if (model != null)
				{
					using (Pen blackPen = new Pen(Color.FromArgb(128, 0, 0, 0)))
					{
						int playbackOffset = (int)TimeToPixels(model.PlaybackPosition) + e.ClipRectangle.X + trackAreaPanOffset.X;
						e.Graphics.DrawLine(blackPen, playbackOffset, e.ClipRectangle.Y, playbackOffset, e.ClipRectangle.Y + e.ClipRectangle.Height);
					}
				}
			}
		}

		/// <summary>
		/// Paint splitter
		/// </summary>
		private void PaintSplitter(PaintEventArgs e)
		{
			if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
				return;
			
			if (SplitterDrag)
			{
				using (Brush background = new System.Drawing.SolidBrush(selectedColor))
				{
					e.Graphics.FillRectangle(background, e.ClipRectangle);
				}
			}
			else
			{
				using (Brush background = new System.Drawing.SolidBrush(this.BackColor))
				{
					e.Graphics.FillRectangle(background, e.ClipRectangle);
				}
			}
		}

		/// <summary>
		/// Paint all (track headers, splitter and track area)
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.SetClip(splitterRect);
			PaintEventArgs splitterArgs = new PaintEventArgs(e.Graphics, splitterRect);
			PaintSplitter(splitterArgs);
			splitterArgs.Dispose();

			if (model == null)
			{
				// No model, nothing to render -> Bail out
				return;
			}
				
			lock (model)
			{
				e.Graphics.SetClip(trackHeaderRect);
				PaintEventArgs trackHeaderArgs = new PaintEventArgs(e.Graphics, trackHeaderRect);
				PaintTrackHeader(trackHeaderArgs);
				trackHeaderArgs.Dispose();

				e.Graphics.SetClip(trackAreaRect);
				PaintEventArgs trackAreaArgs = new PaintEventArgs(e.Graphics, trackAreaRect);
				PaintTrackArea(trackAreaArgs);
				trackAreaArgs.Dispose();
			}
		}

		#endregion

		#region Public accessors

		private Bitmap OverviewBitmap
		{
			get { return overviewBitmap; }
		}

		public Item SelectedItem
		{
			get { return selectedItem; }
			set 
			{
				if (selectedItem != value)
				{
					bool bNotifySelectionChanged = (selectedItem != value);
					selectedItem = value;
					if (bNotifySelectionChanged && OnItemSelectionChanged != null)
					{
						OnItemSelectionChanged(this);
					}
					NotifyPropertyChanged("SelectedItem");

					RequestLayoutUpdate();
				}
			}
		}

		public Track SelectedTrack
		{
			get { return selectedTrack; }
			set 
			{
				if (selectedTrack != value)
				{
					bool bNotifySelectionChanged = (selectedTrack != value);
					selectedTrack = value;
					if (bNotifySelectionChanged && OnTrackSelectionChanged != null)
					{
						OnTrackSelectionChanged(this);
					}
					NotifyPropertyChanged("SelectedTrack");

					RequestLayoutUpdate();
				}
			}
		}

		public SequencerDataModel Model
		{
			get
			{
				return model;
			}
			set
			{
				if (model != value)
				{
					if (model != null)
					{
						model.OnTrackDataChanged -= session_TrackDataChanged;
						model.OnItemDataChanged -= session_ItemDataChanged;
						model.OnPlaybackPositionChanged -= session_PlaybackPositionChanged;
					}
					model = value;
					if (model != null)
					{
						model.OnTrackDataChanged += session_TrackDataChanged;
						model.OnItemDataChanged += session_ItemDataChanged;
						model.OnPlaybackPositionChanged += session_PlaybackPositionChanged;
					}
					NotifyPropertyChanged("Model");
				}
			}
		}

		public float SplitterFraction
		{
			get { return splitterFraction; }
			set
			{
				if (splitterFraction != value)
				{
					splitterFraction = Math.Max(Math.Min((value), 1.0f - 48.0f / Bounds.Width), (float)0.01f);

					PanOffset = trackAreaPanOffset; // Update PanOffset

					RequestLayoutUpdate();

					if (OnScrollBarMarginsChanged != null)
					{
						OnScrollBarMarginsChanged();
					}
					NotifyPropertyChanged("SplitterFraction");
				}
			}
		}

		public int TrackHeight
		{
			get { return trackHeight; }
			set
			{
				if (trackHeight != value)
				{
					trackHeight = value;
					RequestLayoutUpdate();
					NotifyPropertyChanged("TrackHeight");
				}
			}
		}

		public int TrackMargin
		{
			get { return trackMargin; }
			set
			{
				if (trackMargin != value)
				{
					trackMargin = value;
					RequestLayoutUpdate();
					NotifyPropertyChanged("TrackMargin");
				}
			}
		}

		public int TrackIndent
		{
			get { return trackIndent; }
			set
			{
				if (trackIndent != value)
				{
					trackIndent = value;
					RequestLayoutUpdate();
					NotifyPropertyChanged("TrackIndent");
				}
			}
		}

		public int SplitterWidth
		{
			get { return splitterWidth; }
			set
			{
				if (splitterWidth != value)
				{
					splitterWidth = value;

					PanOffset = trackAreaPanOffset; // Update PanOffset

					RequestLayoutUpdate();

					NotifyPropertyChanged("SplitterWidth");
				}
			}
		}

		public float TimePixelRatio
		{
			get
			{
				return timePixelRatio;
			}
			set
			{
				if (timePixelRatio != value)
				{
					timePixelRatio = value;

					PanOffset = trackAreaPanOffset; // Update PanOffset

					RequestLayoutUpdate();

					if (!SuspendNotifications)
					{
						if (OnZoomChanged != null)
							OnZoomChanged(this);

						NotifyPropertyChanged("TimePixelRatio");
					}
				}
			}
		}

		public Point PanOffset
		{
			get { return trackAreaPanOffset; }
			set
			{
				if (trackAreaPanOffset != value)
				{
					trackAreaPanOffset = value;

					// Clamp offset to be within active region
					if (model != null)
					{
						int visibleTrackAreaHeight = visibleTrackCount * (trackHeight + trackMargin * 2);
						int overSizeY = Math.Min(0, trackAreaRect.Height - visibleTrackAreaHeight);
						int overSizeX = Math.Min(0, trackAreaRect.Width - ((int)TimeToPixels(model.Duration) + trackAreaRect.Width / 4));
						trackAreaPanOffset.X = Math.Min(0, Math.Max(overSizeX, trackAreaPanOffset.X));
						trackAreaPanOffset.Y = Math.Min(0, Math.Max(overSizeY, trackAreaPanOffset.Y));
					}

					RequestLayoutUpdate();

					if (!SuspendNotifications)
					{
						if (OnPanOffsetChanged != null)
							OnPanOffsetChanged(trackAreaPanOffset.X, trackAreaPanOffset.Y, TrackAreaPan);

						NotifyPropertyChanged("PanOffset");
					}
				}
			}
		}

		public void SetShowFlag(int flagIndex)
		{
			uint mask = (uint)(1 << flagIndex);
			showFlags = (showFlags | mask);
			UpdateVisibilityFilters();
		}

		public void ClearShowFlag(int flagIndex)
		{
			uint mask = (uint)(1 << flagIndex);
			showFlags = (showFlags & ~mask);
			UpdateVisibilityFilters();
		}

		public bool HasShowFlag(int flagIndex)
		{
			return (showFlags & flagIndex) != 0;
		}

		public void ToggleShowFlag(int flagIndex)
		{
			if (HasShowFlag(flagIndex))
			{
				ClearShowFlag(flagIndex);
			}
			else
			{
				SetShowFlag(flagIndex);
			}
		}

		public bool ShowGenericTracks
		{
			get { return showGenericTracks; }
			set
			{
				showGenericTracks = value;
				UpdateVisibilityFilters();
				NotifyPropertyChanged("ShowGenericTracks");
			}
		}

		public string LogFilter
		{
			get { return logFilter == null ? "" : logFilter.ToString(); }
			set 
			{
				logFilter = value;
				UpdateVisibilityFilters();
				NotifyPropertyChanged("LogFilter");
			}
		}

		public void session_PlaybackPositionChanged(int newPosition)
		{
			RequestLayoutUpdate();
		}

		public void session_TrackDataChanged(Track source, DataChangedEventArgs args)
		{
			if (args.Type == DataChangeType.DCT_Added)
			{
				source.Visible = false;
			}

			RequestLayoutUpdate();
		}

		public void session_ItemDataChanged(Item source, DataChangedEventArgs args)
		{
			if (args.Type != DataChangeType.DCT_Removed)
			{
				UpdateItemVisibility(source);
			}

			RequestLayoutUpdate();
		}

		public void RequestLayoutUpdate()
		{
			layoutUpdateEvent.Set();
		}

		private void UpdateLayout()
		{
			layoutUpdateRunning = true;
			while (layoutUpdateRunning)
			{
				UpdateRects();
				Invalidate();

				layoutUpdateEvent.WaitOne();
			}
		}

		private void UpdateItemVisibility(Item item)
		{
			bool typeMatch = item.Log.FlagsMatch(showFlags);
			item.Visible = item.Log.Match(logFilter) && typeMatch;

			if (item.Visible && item.Parent != null && !item.Parent.Visible)
			{
				// Item is visible but parent track is not -> update track and its parents to be visible
				Track current = item.Parent;
				while (current != null)
				{
					current.Visible = showGenericTracks || !current.IsGenericTrack;
					current = current.Parent;
				}
			}
		}

		private bool UpdateVisibilityFilters(Track track)
		{
			bool hasVisibleItems = false;
			foreach (Item item in track.Items)
			{
				bool typeMatch = item.Log.FlagsMatch(showFlags);
				item.Visible = item.Log.Match(logFilter) && typeMatch;
				hasVisibleItems = hasVisibleItems || item.Visible;
			}

			bool childrenHaveVisibleItems = false;
			// Recurse
			foreach (Track childTrack in track.Children)
			{
				bool visibleChildren = UpdateVisibilityFilters(childTrack);
				childrenHaveVisibleItems = childrenHaveVisibleItems || visibleChildren;
			}

			track.Visible = (hasVisibleItems || childrenHaveVisibleItems) && (showGenericTracks || !track.IsGenericTrack);
			return track.Visible;
		}

		public void UpdateVisibilityFilters()
		{
			if (model != null)
			{
				try
				{
					lock (model)
					{
						foreach (Track track in model.Tracks)
						{
							UpdateVisibilityFilters(track);
						}
					}
				}
				catch (Exception exception)
				{
					System.Console.Out.WriteLine("Failed!" + exception.Message);
				}
			}
			RequestLayoutUpdate();
		}

		private bool GetAbsoluteTrackRectangle(Track trackToSearch, Track currentTrack, ref int currentRow, ref Rectangle outRectangle)
		{
			if (!currentTrack.Visible)
			{
				return false;
			}

			if (currentTrack.Id == trackToSearch.Id)
			{
				SetRectangle(ref outRectangle, currentRow, currentTrack, false);
				outRectangle.Y -= trackAreaPanOffset.Y;
				return true;
			}

			if (!currentTrack.Collapsed)
			{
				// Recurse
				foreach (Track children in currentTrack.Children)
				{
					if (!children.Visible)
					{
						continue; // Skip invisible children
					}
					currentRow++;
					if (GetAbsoluteTrackRectangle(trackToSearch, children, ref currentRow, ref outRectangle))
					{
						return true;
					}
				}
			}

			return false;
		}

		public void ScrollToSelection()
		{
			Track track = selectedTrack != null ? selectedTrack : selectedItem != null ? selectedItem.Parent : null;
			if (track == null || !track.Visible)
			{
				// No track or track is not visible -> Bail out
				return;
			}

			if (model != null)
			{
				Rectangle rect = new Rectangle();
				bool found = false;
				int row = 0;
				foreach (Track section in model.Tracks)
				{
					found = GetAbsoluteTrackRectangle(track, section, ref row, ref rect);
					if (found)
						break;
					row++;
				}
				if (found)
				{
					// Found rectangle -> Calculate pan offset to get to screen
					SetVerticalPanOffset(-rect.Y);
				}
			}
		}
		
		#endregion

		#region Private Accessors

		protected bool SplitterDrag
		{
			get { return splitterDrag; }
			set { splitterDrag = value; }
		}

		protected bool TrackAreaPan
		{
			get { return trackAreaPan; }
			set { trackAreaPan = value; }
		}

		protected float VisibleStartTime
		{
			get { return visibleStartTime; }
			set { visibleStartTime = value; }
		}

		protected float VisibleEndTime
		{
			get { return visibleEndTime; }
			set { visibleEndTime = value; }
		}

		#endregion

		#region Color accessors

		[Category("Track Colors")]
		public Color TrackFirstLevelBackColor
		{
			get	{ return firstLevelTrackBackColor; }
			set
			{
				firstLevelTrackBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackFirstLevelBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackSecondLevelBackColor
		{
			get { return secondLevelTrackBackColor; }
			set
			{
				secondLevelTrackBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackSecondLevelBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackThirdLevelBackColor
		{
			get { return thirdLevelTrackBackColor; }
			set
			{
				thirdLevelTrackBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackThirdLevelBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackFourthLevelBackColor
		{
			get { return fourthLevelTrackBackColor; }
			set
			{
				fourthLevelTrackBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackFourthLevelBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackFirstLevelTextColor
		{
			get { return firstLevelTrackTextColor; }
			set
			{
				firstLevelTrackTextColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackFirstLevelTextColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackSecondLevelTextColor
		{
			get { return secondLevelTrackTextColor; }
			set
			{
				secondLevelTrackTextColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackSecondLevelTextColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackThirdLevelTextColor
		{
			get { return thirdLevelTrackTextColor; }
			set
			{
				thirdLevelTrackTextColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackThirdLevelTextColor");
			}
		}

		[Category("Track Colors")] 
		public Color TrackFourthLevelTextColor
		{
			get { return fourthLevelTrackTextColor; }
			set
			{
				fourthLevelTrackTextColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackFourthLevelTextColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackHeaderBackColor
		{
			get { return trackHeaderBackColor; }
			set
			{
				trackHeaderBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackHeaderBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackAreaBackColor
		{
			get { return trackAreaBackColor; }
			set
			{
				trackAreaBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackAreaBackColor");
			}
		}

		[Category("Track Colors")]
		public Color ItemSelectedBackColor
		{
			get { return selectedItemBackColor; }
			set
			{
				selectedItemBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("ItemSelectedBackColor");
			}
		}

		[Category("Track Colors")]
		public Color TrackSelectedColor
		{
			get { return selectedColor; }
			set
			{
				selectedColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("TrackSelectedColor");
			}
		}

		[Category("Track Colors")]
		public Color ItemActiveBackColor
		{
			get { return activeItemBackColor; }
			set
			{
				activeItemBackColor = value;
				RequestLayoutUpdate();
				NotifyPropertyChanged("ItemActiveBackColor");
			}
		}
		
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public delegate void ZoomChanged(SequencerControl sender);
		public delegate void ItemSelectionChanged(SequencerControl sender);
		public delegate void TrackSelectionChanged(SequencerControl sender);

		public event OverviewImageChanged OnOverviewImageChanged;
		public event PanOffsetChanged OnPanOffsetChanged;

		private Brush[] logBrushes;
		private Brush[] logTextBrushes;

		private Bitmap overviewBitmap;

		public event ZoomChanged OnZoomChanged;
		public event ItemSelectionChanged OnItemSelectionChanged;
		public event TrackSelectionChanged OnTrackSelectionChanged;

		private Font itemFont = new Font("Tahoma", 8, FontStyle.Regular);

		private Color activeItemBackColor = Color.White;
		private Color selectedItemBackColor = Color.Blue;

		private Color firstLevelTrackBackColor = Color.Black;
		private Color firstLevelTrackTextColor = Color.White;

		private Color secondLevelTrackBackColor = Color.OliveDrab;
		private Color secondLevelTrackTextColor = Color.White;

		private Color thirdLevelTrackBackColor = Color.LightSteelBlue;
		private Color thirdLevelTrackTextColor = Color.Black;

		private Color fourthLevelTrackBackColor = Color.LightGray;
		private Color fourthLevelTrackTextColor = Color.Black;

		private Color trackHeaderBackColor = Color.LightGray;
		private Color trackAreaBackColor = Color.White;

		private Color selectedColor = Color.Lime;

		private SequencerDataModel model = null;
		private Rectangle trackHeaderRect;
		private Rectangle trackAreaRect;
		private Rectangle splitterRect;
		private float splitterFraction = 0.2f;
		private int splitterWidth = 8;
		
		private bool splitterDrag = false;
		private bool trackAreaPan = false;
		private Point dragStart = new Point(0, 0);

		private Point trackAreaPanOffsetPrevious = new Point(0, 0);
		private Point trackAreaPanOffset = new Point(0, 0);

		private int trackMargin = 2;
		private int trackHeight = 24;
		private int trackIndent = 24;

		private float timePixelRatio = 10.9f;

		private int visibleTrackCount;
		private float visibleStartTime;
		private float visibleEndTime;

		private Track selectedTrack = null;
		private Item selectedItem = null;

		// Filters
		private bool showGenericTracks = true;
		private uint showFlags = 0xffffffff;
		private string logFilter = null;

		// Layout update thread and wait handle
		private bool layoutUpdateRunning = false;
		private Thread layoutRefreshThread = null;
		private AutoResetEvent layoutUpdateEvent;
	}
}
