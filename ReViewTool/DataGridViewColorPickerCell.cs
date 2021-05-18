using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReViewTool
{
	/// <summary>
	/// Used to render log-flag color cells
	/// </summary>
	public class DataGridViewColorPickerCell : DataGridViewButtonCell
	{
		protected override void Paint(	Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, 
										int rowIndex, DataGridViewElementStates cellState,
										object value, object formattedValue, string errorText, 
										DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle
										advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

			Font font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			Color color = (Color)value;
			Brush backgroundBrush = new SolidBrush(color);
			Brush textBrush = new SolidBrush(color.GetBrightness() <= 0.5f ? Color.White : Color.Black);
			Rectangle cellRectangle = new Rectangle(cellBounds.X, cellBounds.Y, cellBounds.Width - 1, cellBounds.Height - 1);
			string text = "***";
			SizeF size = graphics.MeasureString(text, font);
			graphics.FillRectangle(backgroundBrush, cellRectangle);
			graphics.DrawString(text, font, textBrush, cellBounds.X + cellBounds.Width / 2 - size.Width / 2, cellBounds.Y + cellBounds.Height / 2 - size.Height / 2);
			
			backgroundBrush.Dispose();
			textBrush.Dispose();
			font.Dispose();
		}
	}
}
