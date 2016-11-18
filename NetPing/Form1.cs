using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.NetworkInformation;
using System.Threading;

namespace NetPing
{
    public partial class frmNetPing : Form
    {
        public string pingHost;
        public int pingTTL = 64;
        public int pingTimeout = 1000;
        public int display = 120;

        bool criticalErrorShown = false;

        public frmNetPing()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                stopPing();
            }
            else
            {
                startPing(txtPingTarget.Text);
            }
        }

        public void startPing(string who, int TTL = 64, int timeout = 1000)
        {
            criticalErrorShown = false;
            if (Uri.CheckHostName(who) == UriHostNameType.Unknown)
            {
                MessageBox.Show("This is not a valid host: " + who, "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (who.Length == 0)
            {
                MessageBox.Show("This is not a valid host!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pingHost = who;
            this.Text = "NetPing - " + who;
            pingTTL = Convert.ToInt32(txtTTL.Text);
            pingTimeout = Convert.ToInt32(txtTimeout.Text);
            display = Convert.ToInt32(txtDisplay.Text);
            timer1.Interval = Convert.ToInt32(txtInterval.Text);
            timer1.Enabled = true;
            btnActivate.Text = "Stop";
            txtPingTarget.Enabled = false;
            txtTTL.Enabled = false;
            txtInterval.Enabled = false;
            txtTimeout.Enabled = false;
            txtDisplay.Enabled = false;
        }

        public void stopPing()
        {
            this.Text = "NetPing";
            timer1.Enabled = false;
            btnActivate.Text = "Start";
            txtPingTarget.Enabled = true;
            txtTTL.Enabled = true;
            txtInterval.Enabled = true;
            txtTimeout.Enabled = true;
            txtDisplay.Enabled = true;
        }

        public void addItem(double ping, bool timeout = false)
        {

            Series s = pingChart.Series["srPing"];
            double nextX = 1;

            if (s.Points.Count > 0)
            {
                nextX = s.Points.Last().XValue + 1;
            }

            DataPoint x = new DataPoint();
            x.XValue = nextX;
            x.YValues[0] = ping;
            if (timeout)
            {
                x.IsEmpty = true;
            }

            s.Points.Add(x);

            if (s.Points.Count > display)
            {
                s.Points.Remove(s.Points[0]);
                pingChart.ResetAutoValues();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sendPing(pingHost, pingTTL, pingTimeout);
        }

        public void sendPing(string who, int TTL = 64, int timeout = 1000)
        {
            AutoResetEvent waiter = new AutoResetEvent(false);
            Ping pingSender = new Ping();
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            PingOptions options = new PingOptions(TTL, true);

            pingSender.SendAsync(who, timeout, buffer, options, waiter);
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                stopPing();
                if (!criticalErrorShown)
                {
                    MessageBox.Show("Ping canceled");
                }
                criticalErrorShown = true;
            }

            if (e.Error != null)
            {
                stopPing();
                //this makes sure that critical errors only get shown once, to avoid getting an error of every outstanding call when a connection drops
                if (!criticalErrorShown)
                {
                    MessageBox.Show("Ping failed: " + Environment.NewLine + e.Error.ToString(), "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                criticalErrorShown = true;
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);
        }

        public void DisplayReply(PingReply reply)
        {
            if (reply == null)
            {
                addItem(0, true);
                return;
            }

            switch (reply.Status)
            {
                case IPStatus.Success:
                    addItem(reply.RoundtripTime);
                    break;

                case IPStatus.BadDestination:
                    stopPing();
                    if (!criticalErrorShown)
                    {
                        MessageBox.Show("This is not a valid host!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    criticalErrorShown = true;
                    break;
                case IPStatus.BadRoute:
                    stopPing();
                    if (!criticalErrorShown)
                    {
                        MessageBox.Show("No route to host was found!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    criticalErrorShown = true;
                    break;

                default: //assume timeout
                    addItem(0, true);
                    break;

            }



            /*
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
            */
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                txtTTL.Text = "64";
                txtPingTarget.Text = "";
                txtInterval.Text = "1000";
                txtTimeout.Text = "1000";
            }
            else
            {
                stopPing();
            }

            Series s = pingChart.Series["srPing"];
            s.Points.Clear();
        }

        private void txtTTL_TextChanged(object sender, EventArgs e)
        {
            Int32 a;
            if (!Int32.TryParse(txtTTL.Text, out a))
            {
                txtTTL.Text = "64";
            }
        }

        private void txtTimeout_TextChanged(object sender, EventArgs e)
        {
            Int32 a;
            if (!Int32.TryParse(txtTimeout.Text, out a))
            {
                txtTimeout.Text = "1000";
            }
        }

        private void txtInterval_TextChanged(object sender, EventArgs e)
        {
            Int32 a;
            if (!Int32.TryParse(txtInterval.Text, out a))
            {
                txtInterval.Text = "1000";
            }
        }

        private void txtDisplay_TextChanged(object sender, EventArgs e)
        {
            Int32 a;
            if (!Int32.TryParse(txtDisplay.Text, out a))
            {
                txtDisplay.Text = "120";
            }
        }
    }
}
