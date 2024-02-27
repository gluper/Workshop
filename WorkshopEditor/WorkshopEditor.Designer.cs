
namespace WorkshopEditor
{
    partial class WorkshopEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _textBox1
            // 
            this._textBox1.Location = new System.Drawing.Point(67, 93);
            this._textBox1.Multiline = true;
            this._textBox1.Name = "_textBox1";
            this._textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._textBox1.Size = new System.Drawing.Size(255, 126);
            this._textBox1.TabIndex = 0;
            this._textBox1.TextChanged += new System.EventHandler(this._textBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Text value for Workshopobject";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // _textBox2
            // 
            this._textBox2.Location = new System.Drawing.Point(67, 270);
            this._textBox2.Multiline = true;
            this._textBox2.Name = "_textBox2";
            this._textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._textBox2.Size = new System.Drawing.Size(255, 76);
            this._textBox2.TabIndex = 3;
            this._textBox2.TextChanged += new System.EventHandler(this._textBox_TextChanged);
            // 
            // WorkshopEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._textBox1);
            this.Name = "WorkshopEditor";
            this.Size = new System.Drawing.Size(541, 438);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textBox2;
    }
}
