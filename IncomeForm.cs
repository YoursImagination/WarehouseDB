using System;
using System.Drawing;
using System.Windows.Forms;

namespace WarehouseBD
{
    public partial class IncomeForm : Form
    {
        private DataGridView dgvIncome;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnFilter, btnAdd, btnEdit, btnDelete;

        public IncomeForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Приход товаров";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15)
            };

            btnAdd = CreateStyledButton("Добавить", Color.Green);
            btnEdit = CreateStyledButton("Изменить", Color.Orange);
            btnDelete = CreateStyledButton("Удалить", Color.Red);

            var lblFrom = new Label { Text = "С:", AutoSize = true, Margin = new Padding(10, 0, 5, 0) };
            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100 };
            var lblTo = new Label { Text = "По:", AutoSize = true, Margin = new Padding(10, 0, 5, 0) };
            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100 };
            btnFilter = CreateStyledButton("Применить фильтр", Color.SteelBlue);
            var leftPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(0, 0, 15, 0)
            };
            leftPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            var rightPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Right,
                AutoSize = true,
                WrapContents = false
            };
            rightPanel.Controls.AddRange(new Control[] { lblFrom, dtpFrom, lblTo, dtpTo, btnFilter });

            panelTop.Controls.Add(leftPanel);
            panelTop.Controls.Add(rightPanel);
            dgvIncome = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 248, 255) },
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.LightGray
            };
            btnFilter.Click += FilterData;
            btnAdd.Click += AddRecord;
            btnEdit.Click += EditRecord;
            btnDelete.Click += DeleteRecord;
            this.Controls.Add(dgvIncome);
            this.Controls.Add(panelTop);
        }

        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(0, 0, 8, 0),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true
            };
        }

        private void LoadData(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                dgvIncome.DataSource = DatabaseHelper.GetRecords("Приход", from, to);
                dgvIncome.Columns["ID"].Visible = false;
                dgvIncome.Columns["Дата"].HeaderText = "Дата";
                dgvIncome.Columns["Товар"].HeaderText = "Товар";
                dgvIncome.Columns["Количество"].HeaderText = "Количество";
                dgvIncome.Columns["Дата"].Width = 100;
                dgvIncome.Columns["Количество"].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки прихода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            LoadData(dtpFrom.Value, dtpTo.Value);
        }

        private void AddRecord(object sender, EventArgs e)
        {
            var form = new RecordForm("Приход");
            form.Owner = this;
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void EditRecord(object sender, EventArgs e)
        {
            if (dgvIncome.CurrentRow == null) return;
            int id = (int)dgvIncome.CurrentRow.Cells["ID"].Value;
            string date = dgvIncome.CurrentRow.Cells["Дата"].Value.ToString();
            string product = dgvIncome.CurrentRow.Cells["Товар"].Value.ToString();
            int quantity = (int)dgvIncome.CurrentRow.Cells["Количество"].Value;

            var form = new RecordForm("Приход", id, date, product, quantity);
            form.Owner = this;
            if (form.ShowDialog() == DialogResult.OK)
                LoadData();
        }

        private void DeleteRecord(object sender, EventArgs e)
        {
            if (dgvIncome.CurrentRow == null) return;
            int id = (int)dgvIncome.CurrentRow.Cells["ID"].Value;

            if (MessageBox.Show("Удалить запись прихода?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteRecord("Приход", id);
                LoadData();
            }
        }
    }
}