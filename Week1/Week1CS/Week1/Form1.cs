using System.ComponentModel;
using System.Globalization;

namespace Week1{
    public partial class Form1 : Form{

        private BackgroundWorker asyncWorker;

        public Form1(){
            InitializeComponent();
            asyncWorker = new BackgroundWorker();
            asyncWorker.DoWork += (s, e) => {
                try
                {
                    if (InvokeRequired) {
                        Invoke((MethodInvoker)(() => textBox1.Enabled = false));
                    }
                    int number = int.Parse(textBox1.Text);
                    e.Result = recursiveFactorial(number);
                }
                catch (Exception ex)
                {
                    e.Result = -1;
                }

            };

            asyncWorker.RunWorkerCompleted += (s, e) => {
                //textBox1.Enabled = true;
                string res;
                long r = (long)e.Result;
                if (r == -1)
                {
                    res = "Number cannot be less than 0";
                }
                else if (r <= 0)
                {
                    res = "Overflow!";
                }
                else
                {
                    res = r.ToString();
                }
                label1.Text = "The factorial is: " + res;
                Invoke((MethodInvoker)(() => textBox1.Enabled = true));
            };
        }

        private void button2_Click(object sender, EventArgs e){
            new Credits().Show();
        }

        
        private void button1_Click(object sender, EventArgs e){
            asyncWorker.RunWorkerAsync();
        }

        private long recursiveFactorial(long value){
            if(value < 0){
                return -1;
            }else if (value <= 1){
                return 1;
            }
            else{
                return value * recursiveFactorial(value - 1);
            }
        }
    }
}