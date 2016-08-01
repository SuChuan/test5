namespace pixChange
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.axMapControl1 = new ESRI.ArcGIS.Controls.AxMapControl();
            this.AddRaster = new System.Windows.Forms.Button();
            this.RasterExtent = new System.Windows.Forms.Button();
            this.getPix = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // axMapControl1
            // 
            this.axMapControl1.Location = new System.Drawing.Point(153, 12);
            this.axMapControl1.Name = "axMapControl1";
            this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
            this.axMapControl1.Size = new System.Drawing.Size(747, 445);
            this.axMapControl1.TabIndex = 0;
            // 
            // AddRaster
            // 
            this.AddRaster.Location = new System.Drawing.Point(25, 45);
            this.AddRaster.Name = "AddRaster";
            this.AddRaster.Size = new System.Drawing.Size(98, 34);
            this.AddRaster.TabIndex = 1;
            this.AddRaster.Text = "添加栅格数据";
            this.AddRaster.UseVisualStyleBackColor = true;
            this.AddRaster.Click += new System.EventHandler(this.AddRaster_Click);
            // 
            // RasterExtent
            // 
            this.RasterExtent.Location = new System.Drawing.Point(25, 106);
            this.RasterExtent.Name = "RasterExtent";
            this.RasterExtent.Size = new System.Drawing.Size(98, 33);
            this.RasterExtent.TabIndex = 2;
            this.RasterExtent.Text = "获取栅格范围";
            this.RasterExtent.UseVisualStyleBackColor = true;
            this.RasterExtent.Click += new System.EventHandler(this.RasterExtent_Click);
            // 
            // getPix
            // 
            this.getPix.Location = new System.Drawing.Point(25, 168);
            this.getPix.Name = "getPix";
            this.getPix.Size = new System.Drawing.Size(98, 33);
            this.getPix.TabIndex = 3;
            this.getPix.Text = "读取像素值";
            this.getPix.UseVisualStyleBackColor = true;
            this.getPix.Click += new System.EventHandler(this.getPix_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 248);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "改变像素值";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(25, 334);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "分级渲染";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.classy_click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 469);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.getPix);
            this.Controls.Add(this.RasterExtent);
            this.Controls.Add(this.AddRaster);
            this.Controls.Add(this.axMapControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        private System.Windows.Forms.Button AddRaster;
        private System.Windows.Forms.Button RasterExtent;
        private System.Windows.Forms.Button getPix;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

    }
}

