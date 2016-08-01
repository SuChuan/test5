namespace pixChange
{
    partial class ProListView
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
            this.DataGrdView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DataGrdView)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGrdView
            // 
            this.DataGrdView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGrdView.Location = new System.Drawing.Point(2, 12);
            this.DataGrdView.Name = "DataGrdView";
            this.DataGrdView.RowTemplate.Height = 23;
            this.DataGrdView.Size = new System.Drawing.Size(772, 356);
            this.DataGrdView.TabIndex = 0;
            // 
            // ProListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 367);
            this.Controls.Add(this.DataGrdView);
            this.Name = "ProListView";
            this.Text = "ProListView";
            ((System.ComponentModel.ISupportInitialize)(this.DataGrdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGrdView;

    }
}