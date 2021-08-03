﻿using System;

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
            DuanKou.isop = false;
            new Thread(() =>
            {
                new TcpClient(textBox1.Text == "0.0.0.0" ? "127.0.0.1" : textBox1.Text, int.Parse(textBox3.Text));
            }).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DuanKou.isop = true;

            dk.Run();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(dk.getmyThreadOn().ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dk = new DuanKou(textBox1.Text, int.Parse(textBox3.Text), textBox2.Text, int.Parse(textBox4.Text));
            button2.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            //button4.Enabled = false;
        }
    }

    
}