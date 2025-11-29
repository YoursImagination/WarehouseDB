using System;
using System.Windows.Forms;

namespace WarehouseBD
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label label = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 80 };

            textBox.Text = defaultValue;

            confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; };

            prompt.Controls.AddRange(new Control[] { textBox, label, confirmation });
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}