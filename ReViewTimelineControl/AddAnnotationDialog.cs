using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReView
{
	public partial class AddAnnotationDialog : Form
	{
		public AddAnnotationDialog()
		{
			InitializeComponent();
		}

		public string AnnotationText
		{
			get { return annotationText.Text; }
			set { annotationText.Text = value; }
		}

		private void addButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
