using System;
using System.Data;
using System.Windows.Forms;

namespace WarehouseBD
{
    public partial class RecordForm : Form
    {
        private ComboBox cbProduct;
        private DateTimePicker dtpDate;
        private NumericUpDown nudQuantity;
        private Button btnSave, btnCancel;
        private string recordType;
        private int? recordId;

        public RecordForm(string type, int? id = null, string date = null, string product = null, int? quantity = null)
        {
            recordType = type;
            recordId = id;

            InitializeComponent();
            LoadProducts();

            if (id.HasValue)
            {
                dtpDate.Value = DateTime.Parse(date);
                cbProduct.Text = product;
                nudQuantity.Value = quantity ?? 0;
            }
        }

        private void InitializeComponent()
        {
            this.Text = recordType;
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            var labelProduct = new Label { Text = "Товар:", Left = 20, Top = 20 };
            cbProduct = new ComboBox { Left = 100, Top = 20, Width = 250 };

            var labelDate = new Label { Text = "Дата:", Left = 20, Top = 60 };
            dtpDate = new DateTimePicker { Left = 100, Top = 60, Width = 250 };

            var labelQuantity = new Label { Text = "Количество:", Left = 20, Top = 100 };
            nudQuantity = new NumericUpDown { Left = 100, Top = 100, Width = 250, Minimum = 1, Maximum = 999999 };

            btnSave = new Button { Text = "Сохранить", Left = 250, Top = 200, Width = 100 };
            btnCancel = new Button { Text = "Отмена", Left = 140, Top = 200, Width = 100 };

            btnSave.Click += SaveRecord;
            btnCancel.Click += (_, e) => this.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { labelProduct, cbProduct, labelDate, dtpDate, labelQuantity, nudQuantity, btnSave, btnCancel });
        }

        private void LoadProducts()
        {
            var products = DatabaseHelper.GetProducts();
            cbProduct.DataSource = products;
            cbProduct.DisplayMember = "Наименование";
            cbProduct.ValueMember = "ID";
        }

        private void SaveRecord(object sender, EventArgs e)
        {
            if (cbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int productId = (int)cbProduct.SelectedValue;
            DateTime date = dtpDate.Value;
            int quantity = (int)nudQuantity.Value;

            DatabaseHelper.AddOrUpdateRecord(recordType, recordId, date, productId, quantity);
            this.DialogResult = DialogResult.OK;
        }
    }
}