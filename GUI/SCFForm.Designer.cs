namespace GUI
{
    partial class SCFForm
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
            this.GamesListLabel = new System.Windows.Forms.Label();
            this.gamesComboBox = new System.Windows.Forms.ComboBox();
            this.MaxPriceLabel = new System.Windows.Forms.Label();
            this.PriceDial = new System.Windows.Forms.NumericUpDown();
            this.FetchGamesButton = new System.Windows.Forms.Button();
            this.CalculateChance = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PriceDial)).BeginInit();
            this.SuspendLayout();
            // 
            // GamesListLabel
            // 
            this.GamesListLabel.AutoSize = true;
            this.GamesListLabel.Location = new System.Drawing.Point(13, 65);
            this.GamesListLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GamesListLabel.Name = "GamesListLabel";
            this.GamesListLabel.Size = new System.Drawing.Size(99, 20);
            this.GamesListLabel.TabIndex = 0;
            this.GamesListLabel.Text = "Список игр";
            // 
            // gamesComboBox
            // 
            this.gamesComboBox.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gamesComboBox.FormattingEnabled = true;
            this.gamesComboBox.Location = new System.Drawing.Point(13, 89);
            this.gamesComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.gamesComboBox.Name = "gamesComboBox";
            this.gamesComboBox.Size = new System.Drawing.Size(348, 25);
            this.gamesComboBox.Sorted = true;
            this.gamesComboBox.TabIndex = 1;
            // 
            // MaxPriceLabel
            // 
            this.MaxPriceLabel.AutoSize = true;
            this.MaxPriceLabel.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MaxPriceLabel.Location = new System.Drawing.Point(13, 9);
            this.MaxPriceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MaxPriceLabel.Name = "MaxPriceLabel";
            this.MaxPriceLabel.Size = new System.Drawing.Size(212, 20);
            this.MaxPriceLabel.TabIndex = 2;
            this.MaxPriceLabel.Text = "Максимальная цена игры";
            // 
            // PriceDial
            // 
            this.PriceDial.DecimalPlaces = 2;
            this.PriceDial.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PriceDial.Location = new System.Drawing.Point(13, 33);
            this.PriceDial.Margin = new System.Windows.Forms.Padding(4);
            this.PriceDial.Maximum = new decimal(new int[] {
            75000,
            0,
            0,
            0});
            this.PriceDial.Name = "PriceDial";
            this.PriceDial.Size = new System.Drawing.Size(201, 28);
            this.PriceDial.TabIndex = 3;
            // 
            // FetchGamesButton
            // 
            this.FetchGamesButton.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FetchGamesButton.Location = new System.Drawing.Point(221, 32);
            this.FetchGamesButton.Name = "FetchGamesButton";
            this.FetchGamesButton.Size = new System.Drawing.Size(140, 29);
            this.FetchGamesButton.TabIndex = 4;
            this.FetchGamesButton.Text = "Выбрать";
            this.FetchGamesButton.UseVisualStyleBackColor = true;
            this.FetchGamesButton.Click += new System.EventHandler(this.FetchGamesButton_Click);
            // 
            // CalculateChance
            // 
            this.CalculateChance.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CalculateChance.Location = new System.Drawing.Point(12, 124);
            this.CalculateChance.Name = "CalculateChance";
            this.CalculateChance.Size = new System.Drawing.Size(349, 28);
            this.CalculateChance.TabIndex = 5;
            this.CalculateChance.Text = "Рассчитать";
            this.CalculateChance.UseVisualStyleBackColor = true;
            // 
            // SCFForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 245);
            this.Controls.Add(this.CalculateChance);
            this.Controls.Add(this.FetchGamesButton);
            this.Controls.Add(this.PriceDial);
            this.Controls.Add(this.MaxPriceLabel);
            this.Controls.Add(this.gamesComboBox);
            this.Controls.Add(this.GamesListLabel);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SCFForm";
            this.Text = "Steam Cards Farmer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PriceDial)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label GamesListLabel;
        private System.Windows.Forms.ComboBox gamesComboBox;
        private System.Windows.Forms.Label MaxPriceLabel;
        private System.Windows.Forms.NumericUpDown PriceDial;
        private System.Windows.Forms.Button FetchGamesButton;
        private System.Windows.Forms.Button CalculateChance;
    }
}

