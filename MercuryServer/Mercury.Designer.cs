namespace MercuryServer
{
    partial class Mercury
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mercury));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.frstatus = new System.Windows.Forms.TextBox();
            this.buttonxreport = new System.Windows.Forms.Button();
            this.buttonzreport = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Сервер Меркурия 119-Ф";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(109, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonConnect.Location = new System.Drawing.Point(9, 10);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(133, 60);
            this.buttonConnect.TabIndex = 1;
            this.buttonConnect.Text = "Подключить регистратор";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(157, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Состояние регистратора";
            // 
            // frstatus
            // 
            this.frstatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.frstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.frstatus.Location = new System.Drawing.Point(161, 42);
            this.frstatus.Multiline = true;
            this.frstatus.Name = "frstatus";
            this.frstatus.ReadOnly = true;
            this.frstatus.Size = new System.Drawing.Size(427, 159);
            this.frstatus.TabIndex = 3;
            // 
            // buttonxreport
            // 
            this.buttonxreport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonxreport.Location = new System.Drawing.Point(9, 75);
            this.buttonxreport.Name = "buttonxreport";
            this.buttonxreport.Size = new System.Drawing.Size(133, 60);
            this.buttonxreport.TabIndex = 4;
            this.buttonxreport.Text = "Х-отчет";
            this.buttonxreport.UseVisualStyleBackColor = true;
            this.buttonxreport.Click += new System.EventHandler(this.buttonxreport_Click);
            // 
            // buttonzreport
            // 
            this.buttonzreport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonzreport.Location = new System.Drawing.Point(9, 141);
            this.buttonzreport.Name = "buttonzreport";
            this.buttonzreport.Size = new System.Drawing.Size(133, 60);
            this.buttonzreport.TabIndex = 5;
            this.buttonzreport.Text = "z-отчет";
            this.buttonzreport.UseVisualStyleBackColor = true;
            this.buttonzreport.Click += new System.EventHandler(this.buttonzreport_Click);
            // 
            // Mercury
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 215);
            this.Controls.Add(this.buttonzreport);
            this.Controls.Add(this.buttonxreport);
            this.Controls.Add(this.frstatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonConnect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Mercury";
            this.ShowInTaskbar = false;
            this.Text = "Сервер Меркурия 119-Ф";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mercury_FormClosing);
            this.Load += new System.EventHandler(this.Mercury_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox frstatus;
        private System.Windows.Forms.Button buttonxreport;
        private System.Windows.Forms.Button buttonzreport;
    }
}

