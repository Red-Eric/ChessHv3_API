using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class Program
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
    [STAThread]
    static void Main()
    {
        string komodoPath = null;

        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Title = "Select Komodo Engine";
            ofd.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                komodoPath = ofd.FileName;
            }
            else
            {
                Console.WriteLine("No file selected. Exiting.");
                return;
            }
        }
        var engine = new Komodo(komodoPath, elo: 3500, depth: 10, multipv: 5);
        //AllocConsole();

        var form = new MainForm();

        form.applyButton.Click += (s, e) =>
        {
            engine.Elo = form.eloSlider.Value;
            engine.Depth = form.depthSlider.Value;
            engine.MultiPV = form.multipvSlider.Value;
            MessageBox.Show($"Params applied:\nElo={engine.Elo}\nDepth={engine.Depth}\nMultiPV={engine.MultiPV}");
        };

        Task.Run(() => StartHttpServer(engine));

        Application.Run(form);
    }

    static async Task StartHttpServer(Komodo engine)
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:8080/api/");
        httpListener.Start();
        Console.WriteLine("HTTP server started at http://localhost:8080/api/");

        while (true)
        {
            var context = await httpListener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                string fen;
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                    fen = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(fen) || fen.Split(' ').Length != 6)
                {
                    response.StatusCode = 400;
                    byte[] errorBytes = Encoding.UTF8.GetBytes("[]");
                    await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    response.Close();
                    continue;
                }

                try
                {
                    var bestMoves = engine.GetBestMoves(fen);
                    string json = System.Text.Json.JsonSerializer.Serialize(bestMoves);
                    byte[] respBytes = Encoding.UTF8.GetBytes(json);
                    response.ContentType = "application/json";
                    await response.OutputStream.WriteAsync(respBytes, 0, respBytes.Length);
                }
                catch
                {
                    byte[] errorBytes = Encoding.UTF8.GetBytes("[]");
                    response.ContentType = "application/json";
                    await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                }

                response.Close();
            }
            else
            {
                response.StatusCode = 405;
                response.Close();
            }
        }
    }
}
