using System.Text;
using System.Timers;

namespace RandomTimer
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer;
        private Random rand;
        private DateTime timerStarted;
        private Boolean isDouble = false;

        public Form1()
        {
            InitializeComponent();
            this.timer = new System.Timers.Timer(1000);
            this.timer.AutoReset = true;
            this.timer.Elapsed += OnTimedEvent;
            this.rand = new Random();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.timer.Enabled)
            {
                this.timer.Enabled = true;
                this.timerStarted = DateTime.Now;
                this.button1.Text = "Stop Timer";
            }
            else
            {
                this.timer.Enabled = false;
                this.button1.Text = "Start Timer";
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TimeSpan elapsedTime = e.SignalTime - this.timerStarted;
            Invoke(UpdateUI, elapsedTime);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.isDouble = ((CheckBox)sender).Checked;
        }

        private void UpdateUI(TimeSpan elapsedTime)
        {
            if (elapsedTime.Seconds % 2 == 0)
            {
                this.label1.Text = "The random number is: " + (this.isDouble ? rand.NextDouble().ToString() : rand.Next().ToString());
            }
            this.label2.Text = "Time Elapsed: " + elapsedTime.ToString(@"hh\:mm\:ss");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Credits().Show();
        }
    }
}