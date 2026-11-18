using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

class MainForm : Form
{
    int[] numbers = new int[10000];
    int maxVal, minVal;
    double avgVal;
    int remaining;

    Button btnCalc, btnSave;
    Label lblResults;
    TextBox txtOut;

    public MainForm()
    {
        Text = "Потоки — задача 7";
        Width = 520; Height = 420;
        StartPosition = FormStartPosition.CenterScreen;

        btnCalc = new Button { Text = "Сгенерировать и вычислить", Left = 20, Top = 20, Width = 200 };
        btnSave = new Button { Text = "Сохранить в файл", Left = 230, Top = 20, Width = 140, Enabled = false };
        btnCalc.Click += BtnCalc_Click;
        btnSave.Click += BtnSave_Click;

        lblResults = new Label { Text = "Результаты: —", Left = 380, Top = 25, AutoSize = true };

        txtOut = new TextBox
        {
            Left = 20,
            Top = 60,
            Width = 470,
            Height = 280,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        Controls.Add(btnCalc); Controls.Add(btnSave);
        Controls.Add(lblResults); Controls.Add(txtOut);
    }

    void BtnCalc_Click(object sender, EventArgs e)
    {
        Random rnd = new Random();
        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = rnd.Next(0, 1001);

        txtOut.Clear();
        AppendLine("Сгенерировано 10000 чисел. Вычисляю в 3 потока...");
        btnCalc.Enabled = false;
        btnSave.Enabled = false;
        remaining = 3;

        new Thread(FindMax) { IsBackground = true }.Start();
        new Thread(FindMin) { IsBackground = true }.Start();
        new Thread(FindAvg) { IsBackground = true }.Start();
    }

    void FindMax() { int m = numbers[0]; foreach (int x in numbers) if (x > m) m = x; maxVal = m; Done(); }
    void FindMin() { int m = numbers[0]; foreach (int x in numbers) if (x < m) m = x; minVal = m; Done(); }
    void FindAvg()
    {
        long sum = 0;
        foreach (int x in numbers) sum += x;
        avgVal = (double)sum / numbers.Length;
        Done();
    }

    // Вызывается каждым вычислителем в конце; последний обновляет интерфейс
    void Done()
    {
        if (Interlocked.Decrement(ref remaining) == 0)
        {
            Invoke((Action)(() =>
            {
                lblResults.Text = $"Max={maxVal} Min={minVal} Ср={avgVal:F2}";
                AppendLine($"Максимум: {maxVal}");
                AppendLine($"Минимум:  {minVal}");
                AppendLine($"Среднее:  {avgVal:F2}");
                btnCalc.Enabled = true;
                btnSave.Enabled = true;
            }));
        }
    }

    void BtnSave_Click(object sender, EventArgs e)
    {
        btnSave.Enabled = false;
        new Thread(SaveToFile) { IsBackground = true }.Start();
    }

    void SaveToFile()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.txt");
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("Набор чисел:");
            for (int i = 0; i < numbers.Length; i++)
                sw.Write(numbers[i] + " ");
            sw.WriteLine();
            sw.WriteLine($"Максимум: {maxVal}");
            sw.WriteLine($"Минимум:  {minVal}");
            sw.WriteLine($"Среднее:  {avgVal:F2}");
        }
        Invoke((Action)(() =>
        {
            AppendLine($"Сохранено в файл: {path}");
            btnSave.Enabled = true;
        }));
    }

    void AppendLine(string s)
    {
        if (txtOut.InvokeRequired)
            txtOut.Invoke((Action)(() => txtOut.AppendText(s + "\r\n")));
        else
            txtOut.AppendText(s + "\r\n");
    }
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}