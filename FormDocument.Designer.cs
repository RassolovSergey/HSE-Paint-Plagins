﻿namespace WinForms_v1
{
    partial class FormDocument
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
            this.SuspendLayout();
            // 
            // FormDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.DoubleBuffered = true;
            this.Name = "FormDocument";
            this.TabText = "FormDocument";
            this.Text = "FormDocument";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormDocument_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}