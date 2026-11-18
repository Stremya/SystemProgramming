using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;

/*
class MainForm : Form
{
    ComboBox cboNum, cboLet, cboSym;
    TextBox txtOut;
    Button btnStart;
    string[] priorities = { "Lowest", "BelowNormal", "Normal", "AboveNormal", "Highest" };

    public MainForm()
    {
        Text = "Задача 1 — потоки и приоритеты";
        Width = 560; Height = 460;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(new Label { Text = "Числа:", Left = 20, Top = 25 });
        cboNum = new ComboBox { Left = 80, Top = 22, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
        Controls.Add(new Label { Text = "Буквы:", Left = 195, Top = 25 });
        cboLet = new ComboBox { Left = 255, Top = 22, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
        Controls.Add(new Label { Text = "Символы:", Left = 370, Top = 25 });
        cboSym = new ComboBox { Left = 435, Top = 22, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
        foreach (ComboBox c in new[] { cboNum, cboLet, cboSym })
        {
            c.Items.AddRange(priorities);
            c.SelectedIndex = 2;   // Normal
        }

        btnStart = new Button { Text = "Старт", Left = 20, Top = 55, Width = 100 };
        btnStart.Click += BtnStart_Click;

        txtOut = new TextBox
        {
            Left = 20,
            Top = 90,
            Width = 510,
            Height = 290,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical
        };

        Controls.AddRange(new Control[] { cboNum, cboLet, cboSym, btnStart, txtOut });
    }

    void BtnStart_Click(object sender, EventArgs e)
    {
        txtOut.Clear();
        Thread tNum = new Thread(WorkNumbers); tNum.Priority = GetPriority(cboNum); tNum.IsBackground = true;
        Thread tLet = new Thread(WorkLetters); tLet.Priority = GetPriority(cboLet); tLet.IsBackground = true;
        Thread tSym = new Thread(WorkSymbols); tSym.Priority = GetPriority(cboSym); tSym.IsBackground = true;
        tNum.Start(); tLet.Start(); tSym.Start();
    }

    ThreadPriority GetPriority(ComboBox cbo)
    {
        switch (cbo.SelectedItem as string)
        {
            case "Lowest": return ThreadPriority.Lowest;
            case "BelowNormal": return ThreadPriority.BelowNormal;
            case "AboveNormal": return ThreadPriority.AboveNormal;
            case "Highest": return ThreadPriority.Highest;
            default: return ThreadPriority.Normal;
        }
    }

    void WorkNumbers() { for (int i = 1; i <= 20; i++) { AppendLine($"[Числа] {i}"); Thread.Sleep(100); } }
    void WorkLetters() { for (char c = 'A'; c <= 'T'; c++) { AppendLine($"[Буквы] {c}"); Thread.Sleep(100); } }
    void WorkSymbols()
    {
        foreach (char c in "!@#$%^&*()_+-=[]{}|;:,.<>?") { AppendLine($"[Символы] {c}"); Thread.Sleep(100); }
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
    static void Main() { Application.EnableVisualStyles(); Application.Run(new MainForm()); }
}
*/

/*
class Chunk { public long Offset; public long Length; }

class MainForm : Form
{
    TextBox txtSrc, txtDst;
    NumericUpDown numThreads;
    ProgressBar progress;
    Label lblBytes, lblStatus;
    Button btnStart, btnPause, btnStop;

    string srcPath, dstPath;
    long totalBytes, copiedBytes;
    volatile bool stopRequested;
    bool paused;
    ManualResetEvent pauseEvent = new ManualResetEvent(true);
    Thread[] workers;
    System.Windows.Forms.Timer uiTimer = new System.Windows.Forms.Timer { Interval = 100 };

    public MainForm()
    {
        Text = "Задачи 2–3 — многопоточное копирование файла";
        Width = 560; Height = 270;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(new Label { Text = "Источник:", Left = 20, Top = 25 });
        txtSrc = new TextBox { Left = 90, Top = 22, Width = 350 };
        Button b1 = new Button { Text = "...", Left = 450, Top = 21, Width = 40 };
        b1.Click += (s, e) => { using (OpenFileDialog d = new OpenFileDialog()) if (d.ShowDialog() == DialogResult.OK) txtSrc.Text = d.FileName; };

        Controls.Add(new Label { Text = "Куда:", Left = 20, Top = 55 });
        txtDst = new TextBox { Left = 90, Top = 52, Width = 350 };
        Button b2 = new Button { Text = "...", Left = 450, Top = 51, Width = 40 };
        b2.Click += (s, e) => { using (SaveFileDialog d = new SaveFileDialog()) if (d.ShowDialog() == DialogResult.OK) txtDst.Text = d.FileName; };

        Controls.Add(new Label { Text = "Потоков:", Left = 20, Top = 88 });
        numThreads = new NumericUpDown { Left = 90, Top = 85, Width = 50, Minimum = 1, Maximum = 16, Value = 4 };

        btnStart = new Button { Text = "Старт", Left = 160, Top = 84, Width = 90 };
        btnPause = new Button { Text = "Пауза", Left = 260, Top = 84, Width = 90, Enabled = false };
        btnStop = new Button { Text = "Стоп", Left = 360, Top = 84, Width = 90, Enabled = false };
        btnStart.Click += BtnStart_Click;
        btnPause.Click += BtnPause_Click;
        btnStop.Click += BtnStop_Click;

        progress = new ProgressBar { Left = 20, Top = 120, Width = 470, Height = 25 };
        lblBytes = new Label { Text = "", Left = 20, Top = 150, AutoSize = true };
        lblStatus = new Label { Text = "Выберите файл и нажмите «Старт»", Left = 20, Top = 170, AutoSize = true };

        Controls.AddRange(new Control[] { txtSrc, b1, txtDst, b2, numThreads,
                                          btnStart, btnPause, btnStop, progress, lblBytes, lblStatus });

        uiTimer.Tick += (s, e) =>
        {
            if (totalBytes > 0)
                progress.Value = (int)Math.Min(100, copiedBytes * 100 / totalBytes);
            lblBytes.Text = $"Скопировано: {copiedBytes} из {totalBytes} байт";
        };
        uiTimer.Start();
    }

    void BtnStart_Click(object sender, EventArgs e)
    {
        srcPath = txtSrc.Text; dstPath = txtDst.Text;
        if (!File.Exists(srcPath) || string.IsNullOrEmpty(dstPath))
        { lblStatus.Text = "Укажите источник и приёмник!"; return; }

        int count = (int)numThreads.Value;
        totalBytes = new FileInfo(srcPath).Length;
        copiedBytes = 0; stopRequested = false; paused = false;
        pauseEvent.Set(); btnPause.Text = "Пауза";

        using (FileStream fs = new FileStream(dstPath, FileMode.Create, FileAccess.Write))
            fs.SetLength(totalBytes);

        workers = new Thread[count];
        long chunkSize = totalBytes / count;
        for (int i = 0; i < count; i++)
        {
            long off = i * chunkSize;
            long len = (i == count - 1) ? totalBytes - off : chunkSize;
            workers[i] = new Thread(CopyChunk);
            workers[i].IsBackground = true;
            workers[i].Start(new Chunk { Offset = off, Length = len });
        }
        new Thread(Supervise) { IsBackground = true }.Start();

        btnStart.Enabled = false; btnPause.Enabled = true; btnStop.Enabled = true;
        lblStatus.Text = "Идёт копирование...";
    }

    void CopyChunk(object chunkObj)
    {
        Chunk chunk = (Chunk)chunkObj;
        byte[] buffer = new byte[64 * 1024];
        using (FileStream s = new FileStream(srcPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (FileStream d = new FileStream(dstPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            s.Seek(chunk.Offset, SeekOrigin.Begin);
            d.Seek(chunk.Offset, SeekOrigin.Begin);
            long remaining = chunk.Length;
            while (remaining > 0)
            {
                pauseEvent.WaitOne();        
                if (stopRequested) return;   
                int read = s.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));
                if (read <= 0) break;
                d.Write(buffer, 0, read);
                remaining -= read;
                Interlocked.Add(ref copiedBytes, read);
            }
        }
    }

    void Supervise()
    {
        foreach (Thread t in workers) t.Join();
        Invoke((Action)(() =>
        {
            btnStart.Enabled = true; btnPause.Enabled = false; btnStop.Enabled = false;
            if (stopRequested) { lblStatus.Text = "Копирование остановлено. Можно выбрать другой файл."; progress.Value = 0; }
            else { lblStatus.Text = "Копирование завершено!"; progress.Value = 100; }
        }));
    }

    void BtnPause_Click(object sender, EventArgs e)
    {
        paused = !paused;
        if (paused) { pauseEvent.Reset(); btnPause.Text = "Продолжить"; lblStatus.Text = "Пауза..."; }
        else { pauseEvent.Set(); btnPause.Text = "Пауза"; lblStatus.Text = "Идёт копирование..."; }
    }

    void BtnStop_Click(object sender, EventArgs e)
    {
        stopRequested = true;
        pauseEvent.Set();   
    }
}

static class Program
{
    [STAThread]
    static void Main() { Application.EnableVisualStyles(); Application.Run(new MainForm()); }
}
*/

class MainForm : Form
{
    TextBox txtSrc, txtDst;
    NumericUpDown numThreads;
    ProgressBar progress;
    Label lblBytes, lblStatus;
    Button btnStart, btnPause, btnStop;

    string srcDir, dstDir;
    string[] files;
    long totalBytes, copiedBytes;
    int nextIndex;
    volatile bool stopRequested;
    bool paused;
    ManualResetEvent pauseEvent = new ManualResetEvent(true);
    Thread[] workers;
    System.Windows.Forms.Timer uiTimer = new System.Windows.Forms.Timer { Interval = 100 };

    public MainForm()
    {
        Text = "Задачи 4–5 — копирование директории";
        Width = 560; Height = 270;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(new Label { Text = "Папка-источник:", Left = 20, Top = 25 });
        txtSrc = new TextBox { Left = 120, Top = 22, Width = 320 };
        Button b1 = new Button { Text = "...", Left = 450, Top = 21, Width = 40 };
        b1.Click += (s, e) => { using (FolderBrowserDialog d = new FolderBrowserDialog()) if (d.ShowDialog() == DialogResult.OK) txtSrc.Text = d.SelectedPath; };

        Controls.Add(new Label { Text = "Папка-приёмник:", Left = 20, Top = 55 });
        txtDst = new TextBox { Left = 120, Top = 52, Width = 320 };
        Button b2 = new Button { Text = "...", Left = 450, Top = 51, Width = 40 };
        b2.Click += (s, e) => { using (FolderBrowserDialog d = new FolderBrowserDialog()) if (d.ShowDialog() == DialogResult.OK) txtDst.Text = d.SelectedPath; };

        Controls.Add(new Label { Text = "Потоков:", Left = 20, Top = 88 });
        numThreads = new NumericUpDown { Left = 120, Top = 85, Width = 50, Minimum = 1, Maximum = 16, Value = 4 };

        btnStart = new Button { Text = "Старт", Left = 190, Top = 84, Width = 90 };
        btnPause = new Button { Text = "Пауза", Left = 290, Top = 84, Width = 90, Enabled = false };
        btnStop = new Button { Text = "Стоп", Left = 390, Top = 84, Width = 90, Enabled = false };
        btnStart.Click += BtnStart_Click;
        btnPause.Click += BtnPause_Click;
        btnStop.Click += BtnStop_Click;

        progress = new ProgressBar { Left = 20, Top = 120, Width = 470, Height = 25 };
        lblBytes = new Label { Text = "", Left = 20, Top = 150, AutoSize = true };
        lblStatus = new Label { Text = "Выберите папки и нажмите «Старт»", Left = 20, Top = 170, AutoSize = true };

        Controls.AddRange(new Control[] { txtSrc, b1, txtDst, b2, numThreads,
                                          btnStart, btnPause, btnStop, progress, lblBytes, lblStatus });

        uiTimer.Tick += (s, e) =>
        {
            if (totalBytes > 0)
                progress.Value = (int)Math.Min(100, copiedBytes * 100 / totalBytes);
            lblBytes.Text = $"Скопировано: {copiedBytes} из {totalBytes} байт";
        };
        uiTimer.Start();
    }

    void BtnStart_Click(object sender, EventArgs e)
    {
        srcDir = txtSrc.Text.TrimEnd('\\'); dstDir = txtDst.Text;
        if (!Directory.Exists(srcDir) || string.IsNullOrEmpty(dstDir))
        { lblStatus.Text = "Укажите обе папки!"; return; }

        files = Directory.GetFiles(srcDir, "*.*", SearchOption.AllDirectories);
        totalBytes = 0;
        foreach (string f in files) totalBytes += new FileInfo(f).Length;

        copiedBytes = 0; nextIndex = 0; stopRequested = false; paused = false;
        pauseEvent.Set(); btnPause.Text = "Пауза";
        Directory.CreateDirectory(dstDir);

        int count = (int)numThreads.Value;
        workers = new Thread[count];
        for (int i = 0; i < count; i++)
        {
            workers[i] = new Thread(CopyWorker);
            workers[i].IsBackground = true;
            workers[i].Start();
        }
        new Thread(Supervise) { IsBackground = true }.Start();

        btnStart.Enabled = false; btnPause.Enabled = true; btnStop.Enabled = true;
        lblStatus.Text = $"Идёт копирование {files.Length} файлов...";
    }

    void CopyWorker()
    {
        byte[] buffer = new byte[64 * 1024];
        while (true)
        {
            pauseEvent.WaitOne();
            if (stopRequested) return;

            int idx = Interlocked.Increment(ref nextIndex) - 1;  
            if (idx >= files.Length) return;                    

            string srcFile = files[idx];
            string rel = srcFile.Substring(srcDir.Length).TrimStart('\\');
            string dstFile = Path.Combine(dstDir, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dstFile));

            using (FileStream s = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
            using (FileStream d = new FileStream(dstFile, FileMode.Create, FileAccess.Write))
            {
                int read;
                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    pauseEvent.WaitOne();
                    if (stopRequested) return;
                    d.Write(buffer, 0, read);
                    Interlocked.Add(ref copiedBytes, read);
                }
            }
        }
    }

    void Supervise()
    {
        foreach (Thread t in workers) t.Join();
        Invoke((Action)(() =>
        {
            btnStart.Enabled = true; btnPause.Enabled = false; btnStop.Enabled = false;
            if (stopRequested) { lblStatus.Text = "Остановлено. Можно выбрать другую папку."; progress.Value = 0; }
            else { lblStatus.Text = "Копирование завершено!"; progress.Value = 100; }
        }));
    }

    void BtnPause_Click(object sender, EventArgs e)
    {
        paused = !paused;
        if (paused) { pauseEvent.Reset(); btnPause.Text = "Продолжить"; lblStatus.Text = "Пауза..."; }
        else { pauseEvent.Set(); btnPause.Text = "Пауза"; lblStatus.Text = "Идёт копирование..."; }
    }

    void BtnStop_Click(object sender, EventArgs e)
    {
        stopRequested = true;
        pauseEvent.Set();
    }
}

static class Program
{
    [STAThread]
    static void Main() { Application.EnableVisualStyles(); Application.Run(new MainForm()); }
}
