using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace IP
{
    public partial class Form1 : Form
    {
        DuanKou dk = null;

        public Form1() { InitializeComponent(); }

        private void Form1_Load(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            dk.stop();
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try {
                dk = new DuanKou(textBox1.Text, int.Parse(textBox3.Text), textBox2.Text, int.Parse(textBox4.Text));
            }
            catch
            {
                return;
            }
            
            bool rt = dk.restart();
            button2.Enabled = !rt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(dk.getmyThreadOn().ToString());
        }
    }

    
}
