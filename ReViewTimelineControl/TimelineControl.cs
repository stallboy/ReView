using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ReView
{
	public partial class TimelineControl : UserControl, HorizontalScrollBar, INotifyPropertyChanged
	{
		public TimelineControl()
		{
			InitializeComponent();

			SuspendNotifications = false;

			this.MouseMove += new System.Windows.Forms.MouseEventHandler(ReViewTimelineControl_MouseMove);
			this.MouseHover += new System.EventHandler(ReViewTimelineControl_MouseHover);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(ReViewTimelineControl_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(ReViewTimelineControl_MouseUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ReViewTimelineControl_MouseWheel);

			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

			MajorTimeInterval = GetTickTimeInterval();

			font = new Font("Tahoma", 8, FontStyle.Regular);

			toolTip.AutoPopDelay = 60000;
			toolTip.InitialDelay = 250;
			toolTip.ReshowDelay = 50;
			toolTip.ShowAlways = true;
		}

		~TimelineControl()
		{
			font.Dispose();
		}

		public HorizontalScrollBarPlacement ScrollBarPlacement
		{
			get { return OrientationUp ? HorizontalScrollBarPlacement.HSP_Top : HorizontalScrollBarPlacement.HSP_Bottom; }
			set { }
		}

		private void ReViewTimelineControl_MouseHover(object sender, System.EventArgs e)
		{
			foreach (Annotation annotation in annotationList)
			{
				RectangleF annotationRect = GetAnnotationRectangle(annotation);
				if (annotationRect.Contains(PointToClient(Cursor.Position)))
				{
					toolTip.Show(annotation.Content, this, (int)(annotationRect.X + annotationRect.Width), (int)annotationRect.Y, 60000);
					toolTipRect = annotationRect;
					break;
				}
			}
		}

		private void ReViewTimelineControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int time = PixelsToTime(e.X - panOffset.X);
			float ratio = TimePixelRatio + Math.Sign(e.Delta) * Math.Min(512.0f, Math.Max(0.1f, Math.Abs(((float)e.Delta / (1.0f / TimePixelRatio * 6000.0f)))));

			if (ratio >= 1.0f)
			{
				ratio = (float)Math.Round(ratio * 10) / 10;
			}
			TimePixelRatio = (float)Math.Min(512.0f, Math.Max(1.0f / 8.0f, ratio));

			float newPos = TimeToPixels(time) + panOffset.X;
			PanOffset = new Point(PanOffset.X + (int)(e.X - newPos), PanOffset.Y);

			Invalidate();
		}

		private void ReViewTimelineControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (toolTip.Active && !toolTipRect.Contains(e.X, e.Y))
			{
				toolTip.Hide(this);
				ResetMouseEventArgs();
			}
			if (PlaybackHeaderDrag)
			{
				if (model != null)
				{
					AutoFollowTail = false;

					int newPlaybackPosition = Math.Max(0, PixelsToTime(e.X - panOffset.X));
					bool positionChanged = Model.PlaybackPosition != newPlaybackPosition;
					Model.PlaybackPosition = newPlaybackPosition;

					if (positionChanged)
					{
						NotifyPlaybackPositionChanged(true);
					}
				}
				Invalidate();
			}
			else if (AnnotationDrag)
			{
				selectedAnnotation.Time = Math.Max(0, PixelsToTime(e.X - panOffset.X));
				Invalidate();
			}
			else if (Pan)
			{
				PanOffset = new Point(panOffsetPrevious.X + (e.X - dragStart.X), panOffsetPrevious.Y + (e.Y - dragStart.Y));

				Invalidate();
			}
			else
			{
				if (GetPlaybackHeaderRect().Contains(e.X, e.Y))
				{
					this.Cursor = Cursors.Hand;
				}
				else
				{
					this.Cursor = Cursors.Default;
				}
			}

			currentTime = Math.Max(0, PixelsToTime(e.X - panOffset.X));
		}

		private void ReViewTimelineControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			bool bSelectionChanged = false;
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				selectedAnnotation = null;
				foreach (Annotation annotation in annotationList)
				{
					RectangleF annotationRect = GetAnnotationRectangle(annotation);
					if (annotationRect.Contains(e.X, e.Y))
					{
						bSelectionChanged = true;
						selectedAnnotation = annotation;
						if (e.Button == MouseButtons.Left)
						{
							AnnotationDrag = true;
						}
						break;
					}
				}
			}

			if (e.Button == MouseButtons.Left)
			{
				if (e.X >= 0)
				{
					if (!bSelectionChanged)
					{
						PlaybackHeaderDrag = true;
						if (model != null)
						{
							AutoFollowTail = false;

							Model.PlaybackPosition = Math.Max(0, PixelsToTime(e.X - panOffset.X));

							NotifyPlaybackPositionChanged(true);
						}
					}
					Invalidate();
				}
			}
			if (e.Button == MouseButtons.Middle)
			{
				if (e.X >= 0)
				{
					Pan = true;
					panOffsetPrevious.X = panOffset.X;
					panOffsetPrevious.Y = panOffset.Y;
					this.Cursor = Cursors.Hand;
				}
			}
			dragStart.X = e.X;
			dragStart.Y = e.Y;
		}

		private void ReViewTimelineControlContextMenu_Opening(object sender, CancelEventArgs e)
		{
			ReViewTimelineControlContextMenu.Items.Clear();
			ReViewTimelineControlContextMenu.Items.Add("Add Annotation", null, new System.EventHandler(this.ReViewTimelineControl_AddAnnnotation));
			if (selectedAnnotation != null)
			{
				ReViewTimelineControlContextMenu.Items.Add("Edit Annotation", null, new System.EventHandler(this.ReViewTimelineControl_EditAnnotation));
				ReViewTimelineControlContextMenu.Items.Add("Remove Annotation", null, new System.EventHandler(this.ReViewTimelineControl_RemoveAnnotation));
			}
		}

		protected void ReViewTimelineControl_AddAnnnotation(System.Object sender, System.EventArgs e)
		{
			addAnnotationDialog.StartPosition = FormStartPosition.CenterParent;
			addAnnotationDialog.AnnotationText = "";
			if (addAnnotationDialog.ShowDialog() == DialogResult.OK)
			{
				annotationList.Add(new Annotation(addAnnotationDialog.AnnotationText, currentTime));
				Invalidate();
			}
		}

		protected void ReViewTimelineControl_RemoveAnnotation(System.Object sender, System.EventArgs e)
		{
			if (selectedAnnotation != null)
			{
				annotationList.Remove(selectedAnnotation);
				selectedAnnotation = null;
				Invalidate();
			}
		}

		protected void ReViewTimelineControl_EditAnnotation(System.Object sender, System.EventArgs e)
		{
			if (selectedAnnotation != null)
			{
				addAnnotationDialog.StartPosition = FormStartPosition.CenterParent;
				addAnnotationDialog.AnnotationText = selectedAnnotation.Content;
				if (addAnnotationDialog.ShowDialog() == DialogResult.OK)
				{
					selectedAnnotation.Content = addAnnotationDialog.AnnotationText;
				}
				Invalidate();
			}
		}

		private void ReViewTimelineControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (Pan)
			{
				this.Cursor = Cursors.Default;
			}

			Pan = false;
			PlaybackHeaderDrag = false;
			AnnotationDrag = false;

			Invalidate();
		}


		protected int GetTickTimeInterval()
		{
			float MIN_MAJORTICK_INTERVAL = 100.0f;
			float currentInterval = 0.0f;
			float intervalAdd = 0.5f;
			for (int i = 0; i < 1024; i++)
			{
				if (TimePixelRatio * currentInterval > MIN_MAJORTICK_INTERVAL)
					return (int)(currentInterval * 1000.0f);

				if (currentInterval == 1.0f)
				{
					intervalAdd = 1.0f;
					currentInterval = 2.0f;
				}
				else if (currentInterval == 2.0f)
				{
					intervalAdd = 5.0f;
					currentInterval = 5.0f;
				}
				else if (currentInterval == 30.0f)
				{
					intervalAdd = 60.0f;
					currentInterval = 60.0f;
				}
				else if (currentInterval == 120.0f)
				{
					intervalAdd = 300.0f;
					currentInterval = 300.0f;
				}
				else if (currentInterval == 300.0f)
				{
					intervalAdd = 600.0f;
					currentInterval = 600.0f;
				}
				else
				{
					currentInterval += intervalAdd;
				}
			}
			return 0;
		}

		private bool SuspendNotifications
		{
			get;
			set;
		}

		public void ForceTimePixelRatio(float ratio)
		{
			SuspendNotifications = true;

			TimePixelRatio = ratio;

			SuspendNotifications = false;
		}

		public void ForcePanOffset(Point offset)
		{
			SuspendNotifications = true;

			PanOffset = offset;

			SuspendNotifications = false;
		}

		public float TimePixelRatio
		{
			get
			{
				return timePixelRatio;
			}
			set
			{
				timePixelRatio = value;
				MajorTimeInterval = GetTickTimeInterval();

				PanOffset = panOffset; // Update PanOffset

				Invalidate();

				if (!SuspendNotifications)
				{
					ZoomChanged handler = OnZoomChanged;
					if (handler != null)
					{
						handler(this);
					}

					NotifyPropertyChanged("TimePixelRatio");
				}
			}
		}

		public Point PanOffset
		{
			get { return panOffset; }
			set
			{
				panOffset = value;
	
				// Clamp PanOffset to active region
				if (model != null)
				{
					int width = Bounds.Width;
					int overSizeX = Math.Min(0, width - ((int)TimeToPixels(model.Duration) + width / 4));
					panOffset.X = Math.Min(0, Math.Max(overSizeX, panOffset.X));
				}

				Invalidate();

				if (!SuspendNotifications)
				{
					NotifyPanOffsetChanged(Pan);

					NotifyPropertyChanged("PanOffset");
				}
			}
		}

		public bool OrientationUp
		{
			get { return orientationUp; }
			set
			{
				orientationUp = value;
				NotifyPropertyChanged("OrientationUp");
			}
		}

		protected bool Pan
		{
			get { return pan; }
			set { pan = value; }
		}

		[Category("TimelineColors")]
		public Color TimelineBackColor
		{
			get { return timelineBackColor; }
			set
			{
				timelineBackColor = value;
				Invalidate();
				NotifyPropertyChanged("TimelineBackColor");
			}
		}

		[Category("TimelineColors")]
		public Color ShadowColor
		{
			get { return shadowColor; }
			set
			{
				shadowColor = value;
				Invalidate();
				NotifyPropertyChanged("ShadowColor");
			}
		}

		[Category("TimelineColors")]
		public Color ForegroundColor
		{
			get { return foregroundColor; }
			set
			{
				foregroundColor = value;
				Invalidate();
				NotifyPropertyChanged("ForegroundColor");
			}
		}

		[Category("TimelineColors")]
		public Color AnnotationColor
		{
			get { return annotationColor; }
			set
			{
				annotationColor = value;
				Invalidate();
				NotifyPropertyChanged("AnnotationColor");
			}
		}

		[Category("TimelineColors")]
		public Color SelectedAnnotationColor
		{
			get { return selectedAnnotationColor; }
			set
			{
				selectedAnnotationColor = value;
				Invalidate();
				NotifyPropertyChanged("SelectedAnnotationColor");
			}
		}

		[Category("TimelineColors")]
		public Color PlaybackHeaderColor
		{
			get { return playbackHeaderColor; }
			set
			{
				playbackHeaderColor = value;
				Invalidate();
				NotifyPropertyChanged("PlaybackHeaderColor");
			}
		}

		protected bool AnnotationDrag
		{
			get { return annotationDrag; }
			set { annotationDrag = value; }
		}

		protected bool PlaybackHeaderDrag
		{
			get { return playbackHeaderDrag; }
			set { playbackHeaderDrag = value; }
		}

		protected int MajorTimeInterval
		{
			get { return majorTimeInterval; }
			set { majorTimeInterval = value; }
		}

		protected Rectangle GetPlaybackHeaderRect()
		{
			Rectangle rect = new Rectangle();
			rect.X = 0;
			if (model != null)
			{
				rect.X = (int)(TimeToPixels(model.PlaybackPosition) + panOffset.X - 4.5f);
			}
			rect.Y = 0;
			rect.Width = 9;
			rect.Height = Bounds.Height;
			return rect;
		}

		protected float TimeToPixels(int time)
		{
			return ((float)time / 1000.0f * TimePixelRatio);
		}

		protected int PixelsToTime(float pixels)
		{
			return (int)(pixels / TimePixelRatio * 1000.0f);
		}

		public void UpdatePanOffset(int timeNow)
		{
			PanOffset = new Point(-(int)TimeToPixels(timeNow), PanOffset.Y);			
		}

		protected String TimeToString(int time)
		{
			/*
			TimeSpan span = TimeSpan.FromMilliseconds(time);
			return span.ToString(@"dd\:hh\:mm\:ss\,fff");
			 */
			String s = "";
			float timeInSeconds = (float)Math.Round((float)time / 1000.0f);
			float timeInDoubleSeconds = (float)Math.Round((float)time / 500.0f);
			float hours = (float)Math.Floor(timeInSeconds / 3600.0f);
			float minutes = (float)Math.Floor(timeInSeconds / 60.0f) % 60.0f;
			float seconds = (float)timeInDoubleSeconds / 2.0f % 60.0f;
			if (hours < 10)
				s = s + "0" + hours + ":";
			else
				s = s + hours + ":";
			if (minutes < 10)
				s = s + "0" + minutes + ":";
			else
				s = s + minutes + ":";
			if (seconds < 10)
				s = s + "0" + seconds;
			else
				s = s + seconds;
			return s;
		}

		private void PaintInfoArea(PaintEventArgs e)
		{
		}

		private void PaintReViewTimelineControl(PaintEventArgs e)
		{
			// Create pens and brushes
			Pen foregroundPen = new Pen(ForegroundColor);
			Brush foregroundBrush = new SolidBrush(ForegroundColor);
			Brush playbackheaderBrush = new SolidBrush(PlaybackHeaderColor);
			Brush backgroundBrush = new SolidBrush(TimelineBackColor);

			Rectangle rect = new Rectangle((int)e.Graphics.VisibleClipBounds.X, (int)e.Graphics.VisibleClipBounds.Y, (int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height);

			e.Graphics.FillRectangle(backgroundBrush, rect);

			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			int startY = rect.Y + rect.Height;
			int direction = -1;

			if (!OrientationUp)
			{
				startY = rect.Y;
				direction = 1;
			}

			int majorTimeInterval = MajorTimeInterval;
			float majorPixelInterval = TimeToPixels(majorTimeInterval);
			float minorPixelInterval = (majorPixelInterval / 10.0f);

			float startX = panOffset.X % majorPixelInterval + rect.X;
			float endX = startX + rect.Width - panOffset.X % majorPixelInterval;

			int count = 0;
			for (float xC = startX; xC <= endX; xC += minorPixelInterval)
			{
				if (count % 10 == 0)
				{
					// Render major tick
					int time = (int)(Math.Round(PixelsToTime((float)Math.Round(xC - rect.X - panOffset.X)) * 10.0f) / 10.0f);
					e.Graphics.DrawString(TimeToString(time), font, foregroundBrush, xC, (rect.Y + rect.Height / 2) + (OrientationUp ? 0 : 5), format);
					e.Graphics.DrawLine(foregroundPen, xC, startY, xC, startY + direction * 8);
				}
				else
				{
					// Render minor tick
					e.Graphics.DrawLine(foregroundPen, xC, startY, xC, startY + direction * 4);
				}
				count++;
			}

			startX = rect.X + panOffset.X;
			if (model != null)
			{
				e.Graphics.FillRectangle(playbackheaderBrush, startX, startY + direction * 2, (int)TimeToPixels(model.Duration), 2);
			}

			Rectangle triangleRect = GetPlaybackHeaderRect();
			triangleRect.Y += OrientationUp ? (triangleRect.Height - triangleRect.Width) : 0;
			triangleRect.Height = triangleRect.Width;
			DrawingUtils.FillTriangle(e.Graphics, triangleRect, PlaybackHeaderColor, OrientationUp ? 0.0f : 180.0f);

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			foreach (Annotation annotation in annotationList)
			{
				RectangleF annotationRect = GetAnnotationRectangle(annotation);

				Brush annotationBrush = new System.Drawing.Drawing2D.LinearGradientBrush(annotationRect, annotation == selectedAnnotation ? SelectedAnnotationColor : AnnotationColor, Color.White, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);

				e.Graphics.FillEllipse(annotationBrush, annotationRect);
				e.Graphics.DrawEllipse(foregroundPen, annotationRect);
				e.Graphics.DrawLine(foregroundPen, annotationRect.X + annotationRect.Width / 2.0f, 15.0f, annotationRect.X + annotationRect.Width / 2.0f, 100.0f);

				annotationBrush.Dispose();
			}
			e.Graphics.SmoothingMode = SmoothingMode.None;

			foregroundBrush.Dispose();
			foregroundPen.Dispose();
		}

		private RectangleF GetAnnotationRectangle(Annotation annotation)
		{
			float annotationX = TimeToPixels(annotation.Time) + panOffset.X;
			RectangleF annotationRect = new RectangleF(annotationX - 6.0f, 3.0f, 12.0f, 12.0f);
			return annotationRect;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle infoRect = new Rectangle(0, 0, 0, Bounds.Height);
			e.Graphics.SetClip(infoRect);
			PaintInfoArea(e);

			Rectangle ReViewTimelineControlRect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
			e.Graphics.SetClip(ReViewTimelineControlRect);
			PaintReViewTimelineControl(e);
		}

		public bool AutoFollowTail
		{
			get 
			{
				return autoFollowTail; 
			}
			set 
			{
				if (autoFollowTail != value)
				{
					autoFollowTail = value;
					NotifyPropertyChanged("AutoFollowTail");
				}
			}
		}

		public bool AutoPanToPlaybackHeader
		{
			get 
			{
				return autoPanToPlaybackHeader; 
			}
			set 
			{
				if (autoPanToPlaybackHeader != value)
				{
					autoPanToPlaybackHeader = value;
					NotifyPropertyChanged("AutoPanToPlaybackHeader");
				}
			}
		}

		public TimelineModel Model
		{
			get
			{
				return model;
			}
			set
			{
				if (model != null)
				{
					model.OnPlaybackPositionChanged -= Model_OnPlaybackPositionChanged;
					model.OnDurationChanged -= OnDurationChanged;
				}
				model = value;
				if (model != null)
				{
					model.OnPlaybackPositionChanged += Model_OnPlaybackPositionChanged;
					model.OnDurationChanged += OnDurationChanged;
				}

				NotifyPropertyChanged("Model");
			}
		}

		private void Model_OnPlaybackPositionChanged(int newTime)
		{
			Model.SuspendPlaybackPositionChangedNotifications = true;
			if (AutoPanToPlaybackHeader)
			{
				UpdatePanOffset(Model.PlaybackPosition);
			}

			NotifyPlaybackPositionChanged(false);

			Model.SuspendPlaybackPositionChangedNotifications = false;
			Invalidate();
		}

		private void OnDurationChanged()
		{
			Model.SuspendDurationChangedNotifications = true;
			if (AutoFollowTail)
			{
				Model.PlaybackPosition = Model.Duration;
			}
			Model.SuspendDurationChangedNotifications = false;
			Invalidate();
		}

		private void NotifyPlaybackPositionChanged(bool userChange)
		{
			if (userChange)
			{
				AutoPanToPlaybackHeader = false;
				AutoFollowTail = false;
			}

			PlaybackPositionChanged handler = OnPlaybackPositionChanged;
			if (handler != null)
			{
				handler(userChange);
			}
		}

		private void NotifyPanOffsetChanged(bool userChange)
		{
			if (userChange)
			{
				AutoPanToPlaybackHeader = false;
			}

			PanOffsetChanged handler = OnPanOffsetChanged;
			if (handler != null)
			{
				handler(this, userChange);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public delegate void ZoomChanged(TimelineControl sender);
		public delegate void PanOffsetChanged(TimelineControl sender, bool userAction);
		public delegate void PlaybackPositionChanged(bool userAction);

		public event ZoomChanged OnZoomChanged;
		public event PanOffsetChanged OnPanOffsetChanged;
		public event PlaybackPositionChanged OnPlaybackPositionChanged;

		private Color timelineBackColor = Color.LightGray;
		private Color foregroundColor = Color.Black;
		private Color shadowColor = Color.FromArgb(192, 0, 0, 0);
		private Color annotationColor = Color.Red;
		private Color selectedAnnotationColor = Color.Orange;
		private Color playbackHeaderColor = Color.Red;
		private Font font;

		private TimelineModel model = null;

		private float timePixelRatio = 10.9f;
		private int majorTimeInterval;
		private bool orientationUp = true;
		private bool pan = false;
		private bool playbackHeaderDrag = false;
		private bool annotationDrag = false;
		private bool autoFollowTail = true;
		private bool autoPanToPlaybackHeader = true;
		private Point dragStart = new Point(0, 0);

		private Point panOffsetPrevious = new Point(0, 0);
		private Point panOffset = new Point(0, 0);

		private List<Annotation> annotationList = new List<Annotation>();
		private Annotation selectedAnnotation = null;
		private AddAnnotationDialog addAnnotationDialog = new AddAnnotationDialog();

		private int currentTime = 0;

		private System.Windows.Forms.ToolTip toolTip = new ToolTip();
		private RectangleF toolTipRect;
 	}
}
