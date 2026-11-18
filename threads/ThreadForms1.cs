/*
using System;
using System.Threading;
using System.Windows.Forms;

class MainForm : Form
{
    TextBox txtStart, txtEnd, txtCount, txtOut;
    Button btnStart;
    int remainingThreads;   // сколько потоков ещё работают

    public MainForm()
    {
        Text = "Потоки";
        Width = 500; Height = 450;
        StartPosition = FormStartPosition.CenterScreen;

        txtStart = new TextBox { Text = "0", Left = 60, Top = 22, Width = 60 };
        txtEnd = new TextBox { Text = "50", Left = 180, Top = 22, Width = 60 };
        txtCount = new TextBox { Text = "3", Left = 320, Top = 22, Width = 40 };

        Controls.Add(new Label { Text = "От:", Left = 25, Top = 25 });
        Controls.Add(new Label { Text = "До:", Left = 145, Top = 25 });
        Controls.Add(new Label { Text = "Потоков:", Left = 255, Top = 25 });

        btnStart = new Button { Text = "Старт", Left = 380, Top = 20, Width = 90 };
        btnStart.Click += BtnStart_Click;

        txtOut = new TextBox
        {
            Left = 20,
            Top = 60,
            Width = 450,
            Height = 310,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        Controls.Add(btnStart);
        Controls.Add(txtStart); Controls.Add(txtEnd); Controls.Add(txtCount);
        Controls.Add(txtOut);
    }

    void BtnStart_Click(object sender, EventArgs e)
    {
        int start = int.Parse(txtStart.Text);
        int end = int.Parse(txtEnd.Text);
        int count = int.Parse(txtCount.Text);

        txtOut.Clear();
        btnStart.Enabled = false;   // защита от повторного нажатия

        int total = end - start + 1;
        int perThread = total / count;
        int current = start;
        remainingThreads = count;

        for (int i = 0; i < count; i++)
        {
            int from = current;
            int to = (i == count - 1) ? end : current + perThread - 1;
            Thread t = new Thread(PrintPart);
            t.IsBackground = true;
            t.Start(new int[] { i + 1, from, to });
            current = to + 1;
        }
    }

    void PrintPart(object partObj)
    {
        int[] part = (int[])partObj;
        for (int i = part[1]; i <= part[2]; i++)
        {
            AppendLine($"Поток #{part[0]}: {i}");
            Thread.Sleep(5);
        }
        if (Interlocked.Decrement(ref remainingThreads) == 0)
            Invoke((Action)(() => btnStart.Enabled = true));
    }

    // Потокобезопасный вывод строки в TextBox из любого потока
    void AppendLine(string s)
    {
        if (txtOut.InvokeRequired)
            txtOut.Invoke((Action)(() => txtOut.AppendText(s + "\r\n")));
        else
            txtOut.AppendText(s + "\r\n");
    }
}

static class ThreadForms1
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}
*/