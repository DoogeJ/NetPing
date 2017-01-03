using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace NetPing
{
    public class PingTarget
    {
        private PingForm _form;
        private Chart _chart;
        private string _target;
        private int _interval;
        private int _timeout;
        private int _ttl;
        private int _display;
        private Series _serie = new Series();
        private System.Timers.Timer _timer;
        private Guid _guid;

        public string Target
        {
            get
            {
                return _target;
            }
        }

        public string Guid
        {
            get
            {
                return _guid.ToString();
            }
        }

        public PingTarget(PingForm form, string target, int TTL = 64, int timeout = 1000, int interval = 1000, int display = 100)
        {
            _guid = System.Guid.NewGuid();
            _target = target;
            _chart = form.Chart;
            _form = form;
            _timeout = timeout;
            _interval = interval;
            _ttl = TTL;
            _display = display;

            _timer = new System.Timers.Timer();
            _timer.Interval = _interval;
            _timer.Enabled = true;
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            _serie.BorderWidth = 2;
            _serie.ChartArea = "ChartArea1";
            _serie.ChartType = SeriesChartType.Spline;
            _serie.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
            _serie.EmptyPointStyle.Color = System.Drawing.Color.Red;
            _serie.EmptyPointStyle.LegendText = "Timeout";
            _serie.Legend = "Legend1";
            _serie.LegendText = "Ping " + target;
            _serie.Name = "Ping" + _guid.ToString();

            _chart.Series.Add(_serie);
        }

        public void Stop()
        {
            _timer.Enabled = false;
            MethodInvoker mi = delegate
            {
                _chart.Series.Remove(_serie);
            };
            _chart.Invoke(mi);

            _form.Remove(this);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            sendPing(_target, _ttl, _timeout);
        }

        private void sendPing(string who, int TTL = 64, int timeout = 1000)
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
        public void addItem(double ping, bool timeout = false)
        {

            Series s = _serie;
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

            MethodInvoker mi = delegate
            {
                s.Points.Add(x);

                if (s.Points.Count > _display)
                {
                    s.Points.Remove(s.Points[0]);
                    _chart.ResetAutoValues();
                }
            };
            _chart.Invoke(mi);
        }
        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                Stop();

                if (e.Cancelled)
                {
                    MessageBox.Show("Ping canceled");
                }
                else if (e.Error != null)
                {
                    MessageBox.Show("Ping failed: " + Environment.NewLine + e.Error.ToString(), "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            PingReply reply = e.Reply;

            if (reply == null)
            {
                addItem(0, true);
                return;
            }

            Random rnd = new Random();

            switch (reply.Status)
            {
                case IPStatus.Success:
                  
                    addItem(reply.RoundtripTime);
 
                    break;

                case IPStatus.BadDestination:
                    Stop();
                    MessageBox.Show("This is not a valid host!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case IPStatus.BadRoute:
                    Stop();
                    MessageBox.Show("No route to host was found!", "NetPing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    addItem(0, true);
                    break;

            }

        }
    }
}
