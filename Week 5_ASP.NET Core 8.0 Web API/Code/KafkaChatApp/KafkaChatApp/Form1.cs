using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Confluent.Kafka;

namespace KafkaChatApp
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragControlPoint;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartConsumer();

            txtChatLog.MouseDown += Control_MouseDown;
            txtChatLog.MouseMove += Control_MouseMove;
            txtChatLog.MouseUp += Control_MouseUp;

            txtUsername.MouseDown += Control_MouseDown;
            txtUsername.MouseMove += Control_MouseMove;
            txtUsername.MouseUp += Control_MouseUp;

            txtMessage.MouseDown += Control_MouseDown;
            txtMessage.MouseMove += Control_MouseMove;
            txtMessage.MouseUp += Control_MouseUp;

            btnSend.MouseDown += Control_MouseDown;
            btnSend.MouseMove += Control_MouseMove;
            btnSend.MouseUp += Control_MouseUp;
        }

        private async void SendMessage(string user, string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var fullMessage = $"{user}: {message}";
            await producer.ProduceAsync("chat-topic", new Message<Null, string> { Value = fullMessage });

            txtMessage.Clear();
        }

        private void StartConsumer()
        {
            var config = new ConsumerConfig
            {
                GroupId = "chat-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            Task.Run(() =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe("chat-topic");

                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume();
                        AppendChat(cr.Message.Value);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            });
        }

        private void AppendChat(string text)
        {
            if (txtChatLog.InvokeRequired)
            {
                txtChatLog.Invoke(new Action(() =>
                {
                    txtChatLog.AppendText(text + Environment.NewLine);
                }));
            }
            else
            {
                txtChatLog.AppendText(text + Environment.NewLine);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsername.Text) && !string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                SendMessage(txtUsername.Text, txtMessage.Text);
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragControlPoint = ((Control)sender).Location;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                ((Control)sender).Location = Point.Add(dragControlPoint, new Size(diff));
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
