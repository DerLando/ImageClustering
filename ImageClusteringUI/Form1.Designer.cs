namespace ImageClusteringUI
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Next = new System.Windows.Forms.Button();
            this.pB_Result = new System.Windows.Forms.PictureBox();
            this.btn_Reset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pB_Result)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Next
            // 
            this.btn_Next.Location = new System.Drawing.Point(416, 415);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(372, 23);
            this.btn_Next.TabIndex = 0;
            this.btn_Next.Text = "Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // pB_Result
            // 
            this.pB_Result.Location = new System.Drawing.Point(12, 12);
            this.pB_Result.Name = "pB_Result";
            this.pB_Result.Size = new System.Drawing.Size(776, 397);
            this.pB_Result.TabIndex = 1;
            this.pB_Result.TabStop = false;
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(12, 414);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(398, 23);
            this.btn_Reset.TabIndex = 2;
            this.btn_Reset.Text = "Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.pB_Result);
            this.Controls.Add(this.btn_Next);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pB_Result)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.PictureBox pB_Result;
        private System.Windows.Forms.Button btn_Reset;
    }
}

