using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;

namespace NetPing
{
    public partial class PingForm : Form
    {
        public string pingHost;
        public int pingTTL = 64;
        public int pingTimeout = 1000;
        public int display = 120;

        private BindingList<PingTarget> targets = new BindingList<PingTarget>();

        public PingForm()
        {
            InitializeComponent();

            listBox1.DataSource = targets;
            listBox1.DisplayMember = "Target";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Uri.CheckHostName(txtPingTarget.Text) == UriHostNameType.Unknown)
            {
                MessageBox.Show("This is not a valid host: " + txtPingTarget.Text, "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtPingTarget.Text.Length == 0)
            {
                MessageBox.Show("This is not a valid host!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pingTTL = Convert.ToInt32(txtTTL.Text);
            pingTimeout = Convert.ToInt32(txtTimeout.Text);
            display = Convert.ToInt32(txtDisplay.Text);

            targets.Add(new PingTarget(this, txtPingTarget.Text, pingTTL, pingTimeout, 1000, display));           
        }

       
        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (PingTarget _target in targets.ToList())
            {
                _target.Stop();
            }
            targets.Clear();
        }
        private void txtTTL_TextChanged(object sender, EventArgs e)
        {
            int a;
            if (!int.TryParse(txtTTL.Text, out a))
            {
                txtTTL.Text = "64";
            }
        }

        private void txtTimeout_TextChanged(object sender, EventArgs e)
        {
            int a;
            if (!int.TryParse(txtTimeout.Text, out a))
            {
                txtTimeout.Text = "1000";
            }
        }

        private void txtInterval_TextChanged(object sender, EventArgs e)
        {
            int a;
            if (!int.TryParse(txtInterval.Text, out a))
            {
                txtInterval.Text = "1000";
            }
        }

        private void txtDisplay_TextChanged(object sender, EventArgs e)
        {
            int a;
            if (!int.TryParse(txtDisplay.Text, out a))
            {
                txtDisplay.Text = "120";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) { return; }
            PingTarget _target = (PingTarget)listBox1.SelectedItem;
            _target.Stop();
            targets.Remove(_target);
        }

        public void Remove(PingTarget _t)
        {
            targets.Remove(_t);
        }

        public Chart Chart 
        {
            get
            {
                return pingChart;
            }
        }
    }
}
