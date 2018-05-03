using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using AForge.Imaging.Filters; 

namespace CSharpFilters  // 
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Drawing.Bitmap m_Bitmap;
		private System.Drawing.Bitmap m_Undo;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
	
		private System.Windows.Forms.MenuItem FileLoad;
	
		private System.Windows.Forms.MenuItem FileExit;
	
		private double Zoom = 1.0;
	
		private System.Windows.Forms.MenuItem menuItem3;
	//	private System.Windows.Forms.MenuItem FilterSmooth;
		private System.Windows.Forms.MenuItem GaussianBlur;
	
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem Undo;
		
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem EdgeSobell;
        private MenuItem Canny;
        private MenuItem menuItem2;
        private MenuItem menuItem4;
        private MenuItem menuItem7;
        private MenuItem menuItem8;
        private MenuItem menuItem9;
        private MenuItem menuItem10;
        private IContainer components;

		public Form1()
		{
			InitializeComponent();

			m_Bitmap= new Bitmap(2, 2);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.FileLoad = new System.Windows.Forms.MenuItem();
            this.FileExit = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.Undo = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.GaussianBlur = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.EdgeSobell = new System.Windows.Forms.MenuItem();
            this.Canny = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem5,
            this.menuItem3,
            this.menuItem6,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileLoad,
            this.FileExit});
            this.menuItem1.Text = "File";
            // 
            // FileLoad
            // 
            this.FileLoad.Index = 0;
            this.FileLoad.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.FileLoad.Text = "Load";
            this.FileLoad.Click += new System.EventHandler(this.File_Load);
            // 
            // FileExit
            // 
            this.FileExit.Index = 1;
            this.FileExit.Text = "Exit";
            this.FileExit.Click += new System.EventHandler(this.File_Exit);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Undo});
            this.menuItem5.Text = "Edit";
            // 
            // Undo
            // 
            this.Undo.Index = 0;
            this.Undo.Text = "Undo";
            this.Undo.Click += new System.EventHandler(this.OnUndo);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.GaussianBlur});
            this.menuItem3.Text = "Convolution";
            // 
            // GaussianBlur
            // 
            this.GaussianBlur.Index = 0;
            this.GaussianBlur.Text = "Gaussian Blur";
            this.GaussianBlur.Click += new System.EventHandler(this.OnGaussianBlur);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.EdgeSobell,
            this.menuItem7,
            this.menuItem8,
            this.Canny,
            this.menuItem9,
            this.menuItem10});
            this.menuItem6.Text = "Edge Detection";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // EdgeSobell
            // 
            this.EdgeSobell.Index = 0;
            this.EdgeSobell.Text = "Sobel 3 by 3";
            this.EdgeSobell.Click += new System.EventHandler(this.OnEdgeSobell);
            // 
            // Canny
            // 
            this.Canny.Index = 3;
            this.Canny.Text = "Canny 3 by 3";
            this.Canny.Click += new System.EventHandler(this.EdgeCanny);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.Text = "Sobel 5 by 5";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "Sobel 7 by 7";
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 4;
            this.menuItem9.Text = "Canny 5 by 5";
            this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 5;
            this.menuItem10.Text = "Canny 7 by 7";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            this.menuItem2.Text = "Segmentation";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "K_Means";
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(488, 421);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Edge Detection  & K Means Segmentation";
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		protected override void OnPaint (PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.DrawImage(m_Bitmap, new Rectangle(this.AutoScrollPosition.X, this.AutoScrollPosition.Y, (int)(m_Bitmap.Width*Zoom), (int)(m_Bitmap.Height * Zoom)));
		}

		

		private void File_Load(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.InitialDirectory = "c:\\" ;
			openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|All valid files (*.bmp/*.jpg)|*.bmp/*.jpg";
			openFileDialog.FilterIndex = 2 ;
			openFileDialog.RestoreDirectory = true ;

			if(DialogResult.OK == openFileDialog.ShowDialog())
			{
				m_Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName, false);
				this.AutoScroll = true;
				this.AutoScrollMinSize = new Size ((int)(m_Bitmap.Width * Zoom), (int)(m_Bitmap.Height * Zoom));
				this.Invalidate();
			}
		}

	
		private void File_Exit(object sender, System.EventArgs e)
		{
			this.Close();
		}

	
		private void OnFilterSmooth(object sender, System.EventArgs e)
		{
			m_Undo = (Bitmap)m_Bitmap.Clone();
			if(BitmapFilter.Smooth(m_Bitmap, 1))
				this.Invalidate();
		}

		private void OnGaussianBlur(object sender, System.EventArgs e)
		{
			m_Undo = (Bitmap)m_Bitmap.Clone();
			if(BitmapFilter.GaussianBlur(m_Bitmap, 4))
				this.Invalidate();
		}

	
		private void OnUndo(object sender, System.EventArgs e)
		{
			Bitmap temp = (Bitmap)m_Bitmap.Clone();
			m_Bitmap = (Bitmap)m_Undo.Clone();
			m_Undo = (Bitmap)temp.Clone();
			this.Invalidate();
		}
	
		private void OnEdgeSobell(object sender, System.EventArgs e)
		{
			Parameter dlg = new Parameter();
			dlg.nValue = 0;

			if (DialogResult.OK == dlg.ShowDialog())
			{
				m_Undo = (Bitmap)m_Bitmap.Clone();
		    if(BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_SOBEL3X3   ,  (byte)dlg.nValue))
					this.Invalidate();
			}		
		}

        private void EdgeCanny(object sender, System.EventArgs e) 
		{
            
            
            	Parameter dlg = new Parameter();
			dlg.nValue = 0;

            
			if (DialogResult.OK == dlg.ShowDialog())
			{
				m_Undo = (Bitmap)m_Bitmap.Clone();
				
                if (BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_CANNY3X3 , (byte)dlg.nValue))
					this.Invalidate();
			}	
		}

        private void menuItem4_Click(object sender, EventArgs e)
        {

            


        }

        private void menuItem7_Click(object sender, EventArgs e)   // SOBEL 5 BY 5 
        {
            Parameter dlg = new Parameter();
            dlg.nValue = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                m_Undo = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_SOBEL5X5, (byte)dlg.nValue))
                    this.Invalidate();
            }		
        }

        private void menuItem8_Click(object sender, EventArgs e)  // SOBEL 7 by 7 
        {
            Parameter dlg = new Parameter();
            dlg.nValue = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                m_Undo = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_SOBEL7X7, (byte)dlg.nValue))
                    this.Invalidate();
            }		
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {

        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            Parameter dlg = new Parameter();
            dlg.nValue = 0;
            
            if (DialogResult.OK == dlg.ShowDialog())
            {
               
                m_Undo = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_CANNY5X5, (byte)dlg.nValue))
                    this.Invalidate();
            }		
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {
            Parameter dlg = new Parameter();
            dlg.nValue = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {

                m_Undo = (Bitmap)m_Bitmap.Clone();
                if (BitmapFilter.EdgeDetectConvolution(m_Bitmap, BitmapFilter.EDGE_DETECT_CANNY7X7, (byte)dlg.nValue))
                    this.Invalidate();
            }		
        }

        

       


	}
}

