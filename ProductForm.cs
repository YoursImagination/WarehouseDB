using System;
using System.Data;
using System.Windows.Forms;

namespace WarehouseBD
{
    public partial class ProductForm : Form
    {
        private DataGridView dgvProducts;
        private Button btnAdd, btnEdit, btnDelete;

        public ProductForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Товары";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false
            };

            btnAdd = new Button { Text = "Добавить", Dock = DockStyle.Bottom };
            btnEdit = new Button { Text = "Изменить", Dock = DockStyle.Bottom };
            btnDelete = new Button { Text = "Удалить", Dock = DockStyle.Bottom };

            btnAdd.Click += AddProduct;
            btnEdit.Click += EditProduct;
            btnDelete.Click += DeleteProduct;

            Controls.AddRange(new Control[] { dgvProducts, btnAdd, btnEdit, btnDelete });
        }

        private void LoadData()
        {
            try
            {
                dgvProducts.DataSource = DatabaseHelper.GetProducts();
                dgvProducts.Columns["ID"].Visible = false; // Скрываем ID
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddProduct(object sender, EventArgs e)
        {
            string name = Prompt.ShowDialog("Наименование товара:", "Добавить товар");
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (DatabaseHelper.IsProductNameExists(name))
                {
                    MessageBox.Show("Товар с таким наименованием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseHelper.AddProduct(name);
                LoadData();
            }
        }

        private void EditProduct(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            int id = (int)dgvProducts.CurrentRow.Cells["ID"].Value;
            string oldName = dgvProducts.CurrentRow.Cells["Наименование"].Value.ToString();

            string newName = Prompt.ShowDialog("Наименование:", "Изменить товар", oldName);
            if (!string.IsNullOrWhiteSpace(newName) && newName != oldName)
            {
                if (DatabaseHelper.IsProductNameExists(newName, id))
                {
                    MessageBox.Show("Товар с таким наименованием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DatabaseHelper.UpdateProduct(id, newName);
                LoadData();
            }
        }

        private void DeleteProduct(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            int id = (int)dgvProducts.CurrentRow.Cells["ID"].Value;
            string name = dgvProducts.CurrentRow.Cells["Наименование"].Value.ToString();

            if (MessageBox.Show($"Удалить товар '{name}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseHelper.DeleteProduct(id);
                LoadData();
            }
        }
    }
}