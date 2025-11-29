using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarehouseBD
{
    public partial class MainForm : Form
    {
        private DataGridView dgvStock;
        private Button btnProducts, btnIncome, btnExpense, btnRefresh;
        private TabControl tabControl;
        private TabPage tabPageStock, tabPageIncome, tabPageExpense;
        private Panel panelTop, panelBottom;

        public MainForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Склад — Наличие";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightSteelBlue
            };

            btnProducts = CreateButton("Товары", Color.SteelBlue);
            btnIncome = CreateButton("Приход", Color.SteelBlue);
            btnExpense = CreateButton("Расход", Color.SteelBlue);
            btnRefresh = CreateButton("Обновить", Color.Green);

            btnProducts.Click += (_, e) => ShowProductForm();
            btnIncome.Click += (_, e) => ShowIncomeForm();
            btnExpense.Click += (_, e) => ShowExpenseForm();
            btnRefresh.Click += (_, e) => LoadData();

            panelTop.Controls.AddRange(new Control[] { btnProducts, btnIncome, btnExpense, btnRefresh });

            dgvStock = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Lavender },
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            // Вкладки (для Прихода и Расхода)
            tabControl = new TabControl { Dock = DockStyle.Fill };
            tabPageStock = new TabPage("Наличие");
            tabPageIncome = new TabPage("Приход");
            tabPageExpense = new TabPage("Расход");

            tabPageStock.Controls.Add(dgvStock);
            tabPageIncome.Controls.Add(CreatePlaceholder("Приход"));
            tabPageExpense.Controls.Add(CreatePlaceholder("Расход"));

            tabControl.TabPages.AddRange(new TabPage[] { tabPageStock, tabPageIncome, tabPageExpense });
            panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                BackColor = Color.Gainsboro
            };

            var lblStatus = new Label
            {
                Text = "Готово",
                Dock = DockStyle.Left,
                Margin = new Padding(10, 0, 0, 0),
                ForeColor = Color.Gray
            };

            panelBottom.Controls.Add(lblStatus);

            Controls.AddRange(new Control[] { panelTop, tabControl, panelBottom });
        }

        private Button CreateButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Width = 90,
                Height = 30,
                Margin = new Padding(5, 10, 5, 10),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private Panel CreatePlaceholder(string title)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var label = new Label
            {
                Text = $"Данные {title} будут здесь.\nДля редактирования нажмите кнопку «{title}» вверху.",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray
            };
            panel.Controls.Add(label);
            return panel;
        }

        private void LoadData()
        {
            try
            {
                dgvStock.DataSource = DatabaseHelper.GetStock();
                dgvStock.Columns["Наименование"].HeaderText = "Товар";
                dgvStock.Columns["Наличие"].HeaderText = "Наличие";
                dgvStock.AutoResizeColumns();
                dgvStock.Columns["Наименование"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvStock.Columns["Наличие"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowProductForm()
        {
            var form = new ProductForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void ShowIncomeForm()
        {
            var form = new IncomeForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void ShowExpenseForm()
        {
            var form = new ExpenseForm();
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
        }
    }
}