using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace ArcoBot.Utility
{
    public class RichTextBoxOutputter : TextWriter
    {
        RichTextBox textBox = null;

        public RichTextBoxOutputter(RichTextBox output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.AppendText(value.ToString());
            }));
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
