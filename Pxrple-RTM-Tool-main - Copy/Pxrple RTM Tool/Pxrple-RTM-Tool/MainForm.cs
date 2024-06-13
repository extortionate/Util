using PS3Lib;
using PS3Lib.NET;
using PS3ManagerAPI;
using static PS3ManagerAPI.PS3MAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Media;
using System.Net;
using System.Windows.Input;
using Pxrple_RTM_Tool;
using System.Threading;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Web;
using System.Web.Script.Serialization;
using System.Net.WebSockets;

namespace Tool_Base
{
    public partial class MainForm : Form
    {
        #region -> Things <-
        private bool isOpenedByButton = false;
        private bool isConversionInProgress = false;
        private const string ftpRootDirectory = "/dev_hdd0/game/";
        public static PS3API PS3 = new PS3API();
        public static CCAPI CCAPIX = new CCAPI();
        public static PS3ManagerAPI.PS3MAPI PS3M_API = new PS3ManagerAPI.PS3MAPI();
        private BackgroundWorker uploadWorker = new BackgroundWorker();
        private const int DefaultTimeoutMilliseconds = Timeout.Infinite;
        private Color progressBarColor = Color.Blue;
        private TemperatureMonitor temperatureMonitor;
        private string filePath1;
        private string filePath2;
        private string API = "";
        private bool isHovered = false;
        private bool isDragging = false;
        private Point lastCursorPos;
        private Point lastFormPos;
        private string ps3IpAddress;
        private uint[] procs;
        private string[] f;
        private string[] p;
        private int start_offset = 268697600;
        private bool isPictureBoxVisible = true;
        private bool isConnectedOrAttached = false;
        private bool isDisconnected = false;
        private bool isConnected = false;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            PopulateGamesComboBox();
            PopulateSymbolsComboBox();
            PopulateComboBox();
           // PopulateComboBoxWithLedOptions();

            #region Styles-Other
            pictureBox1.AllowDrop = true; // Enable drag and drop
            pictureBox1.DragEnter += pictureBox1_DragEnter; // Subscribe to DragEnter event
            pictureBox1.DragDrop += pictureBox1_DragDrop; // Subscribe to DragDrop event

            comboBox9.SelectedIndexChanged += comboBox9_SelectedIndexChanged;

            string[] selectedAccount = { "00000001", "00000002", "00000003", "00000004", "00000005", "00000006", "00000007", "00000008", "00000009", "00000010" };
            comboBox1.Items.AddRange(selectedAccount);

            string[] selectedAccounts = { "00000001", "00000002", "00000003", "00000004", "00000005", "00000006", "00000007", "00000008", "00000009", "00000010" };
            comboBox7.Items.AddRange(selectedAccounts);

            label1.Cursor = Cursors.Hand;
            label1.Click += label1_Click;
            label1.MouseEnter += label1_MouseEnter;
            label1.MouseLeave += label1_MouseLeave;
            label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            label1.BackColor = Color.Transparent;
            label1.Paint += label1_Paint;
            label1.Click += label1_Click;

            label32.Cursor = Cursors.Hand;
            label32.Click += label32_Click;
            label32.MouseEnter += label32_MouseEnter;
            label32.MouseLeave += label32_MouseLeave;
            label32.BorderStyle = System.Windows.Forms.BorderStyle.None;
            label32.BackColor = Color.Transparent;
            label32.Paint += label32_Paint;
            label32.Click += label32_Click;

            panel3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            panel3.Visible = false;
            panel3.BackColor = Color.Transparent;

            panel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            panel2.Visible = false;
            panel2.BackColor = Color.Transparent;

            buttonctest.FlatStyle = FlatStyle.Flat;
            buttonctest.FlatAppearance.BorderSize = 0;
            buttonctest.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttonctest.Width, buttonctest.Height, 15, 15));

            buttonsend.FlatStyle = FlatStyle.Flat;
            buttonsend.FlatAppearance.BorderSize = 0;
            buttonsend.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttonsend.Width, buttonsend.Height, 15, 15));

            buttondir.FlatStyle = FlatStyle.Flat;
            buttondir.FlatAppearance.BorderSize = 0;
            buttondir.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttondir.Width, buttondir.Height, 15, 15));

            buttonsd.FlatStyle = FlatStyle.Flat;
            buttonsd.FlatAppearance.BorderSize = 0;
            buttonsd.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttonsd.Width, buttonsd.Height, 15, 15));

            button5.FlatStyle = FlatStyle.Flat;
            button5.FlatAppearance.BorderSize = 0;
            button5.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button5.Width, button5.Height, 15, 15));

            button4.FlatStyle = FlatStyle.Flat;
            button4.FlatAppearance.BorderSize = 0;
            button4.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button4.Width, button4.Height, 15, 15));

            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;
            button3.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button3.Width, button3.Height, 15, 15));

            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button1.Width, button1.Height, 15, 15));

            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0; // Optional: remove border
            button2.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button2.Width, button2.Height, 15, 15));

            button6.FlatStyle = FlatStyle.Flat;
            button6.FlatAppearance.BorderSize = 0; // Optional: remove border
            button6.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button6.Width, button6.Height, 15, 15));

            button7.FlatStyle = FlatStyle.Flat;
            button7.FlatAppearance.BorderSize = 0; // Optional: remove border
            button7.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button7.Width, button7.Height, 15, 15));

            button9.FlatStyle = FlatStyle.Flat;
            button9.FlatAppearance.BorderSize = 0; // Optional: remove border
            button9.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button9.Width, button9.Height, 15, 15));

            button10.FlatStyle = FlatStyle.Flat;
            button10.FlatAppearance.BorderSize = 0; // Optional: remove border
            button10.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button10.Width, button10.Height, 15, 15));

            button11.FlatStyle = FlatStyle.Flat;
            button11.FlatAppearance.BorderSize = 0; // Optional: remove border
            button11.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button11.Width, button11.Height, 15, 15));

            button12.FlatStyle = FlatStyle.Flat;
            button12.FlatAppearance.BorderSize = 0; // Optional: remove border
            button12.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button12.Width, button12.Height, 15, 15));

            ConnectBtn.FlatStyle = FlatStyle.Flat;
            ConnectBtn.FlatAppearance.BorderSize = 0; // Optional: remove border
            ConnectBtn.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, ConnectBtn.Width, ConnectBtn.Height, 15, 15));

            button8.FlatStyle = FlatStyle.Flat;
            button8.FlatAppearance.BorderSize = 0; // Optional: remove border
            button8.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button8.Width, button8.Height, 15, 15));

            button13.FlatStyle = FlatStyle.Flat;
            button13.FlatAppearance.BorderSize = 0; // Optional: remove border
            button13.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button13.Width, button13.Height, 15, 15));

            pictureBox6.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, pictureBox6.Width, pictureBox6.Height, 15, 15));

            button18.FlatStyle = FlatStyle.Flat;
            button18.FlatAppearance.BorderSize = 0; // Optional: remove border
            button18.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button18.Width, button18.Height, 15, 15));

            button17.FlatStyle = FlatStyle.Flat;
            button17.FlatAppearance.BorderSize = 0; // Optional: remove border
            button17.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button17.Width, button17.Height, 15, 15));

            button15.FlatStyle = FlatStyle.Flat;
            button15.FlatAppearance.BorderSize = 0; // Optional: remove border
            button15.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, button15.Width, button15.Height, 15, 15));

            buttongs.FlatStyle = FlatStyle.Flat;
            buttongs.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttongs.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttongs.Width, buttongs.Height, 15, 15));


            buttongs2.FlatStyle = FlatStyle.Flat;
            buttongs2.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttongs2.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttongs2.Width, buttongs2.Height, 15, 15));


            buttongs3.FlatStyle = FlatStyle.Flat;
            buttongs3.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttongs3.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttongs3.Width, buttongs3.Height, 15, 15));

            buttonpcm.FlatStyle = FlatStyle.Flat;
            buttonpcm.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttonpcm.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttonpcm.Width, buttonpcm.Height, 15, 15));

            browseButton.FlatStyle = FlatStyle.Flat;
            browseButton.FlatAppearance.BorderSize = 0; // Optional: remove border
            browseButton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, browseButton.Width, browseButton.Height, 15, 15));

            checkButton.FlatStyle = FlatStyle.Flat;
            checkButton.FlatAppearance.BorderSize = 0; // Optional: remove border
            checkButton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, checkButton.Width, checkButton.Height, 15, 15));

            buttonfxc.FlatStyle = FlatStyle.Flat;
            buttonfxc.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttonfxc.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttonfxc.Width, buttonfxc.Height, 15, 15));

            buttoncmp.FlatStyle = FlatStyle.Flat;
            buttoncmp.FlatAppearance.BorderSize = 0; // Optional: remove border
            buttoncmp.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(1, 1, buttoncmp.Width, buttoncmp.Height, 15, 15));

            #endregion
        }

        #region Drawing
        // Helper function to create a rounded rectangle region
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void DrawCircularOutline(Control control, Color outlineColor, int outlineWidth)
        {
            using (Graphics g = control.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw the circular outline around the border of the panel
                using (Pen pen = new Pen(outlineColor, outlineWidth))
                {
                    g.DrawEllipse(pen, 0, 0, control.Width - 1, control.Height - 1);
                }
            }
        }
        #endregion

        #region Dat-Stuff
        private void ConvertDatToPng(string datFilePath)
        {
            try
            {
                byte[] data = File.ReadAllBytes(datFilePath);

                // Find the start position of PNG signature
                int pngStartIndex = FindBytes(data, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

                if (pngStartIndex != -1)
                {
                    // Extract the PNG content
                    byte[] pngContent = data.Skip(pngStartIndex).ToArray();

                    // Convert the PNG content directly to an Image
                    using (MemoryStream stream = new MemoryStream(pngContent))
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox1.Image = Image.FromStream(stream);
                    }

                    // Ask the user if they want to save the image only if opened by the button
                    if (isOpenedByButton)
                    {
                        DialogResult result = MessageBox.Show("Do you want to save the image?", "Save Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Prompt user for save location
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "PNG Files|*.png";
                            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                // Save the PNG content to the specified file
                                File.WriteAllBytes(saveFileDialog.FileName, pngContent);
                            }
                        }
                    }
                }
                else
                {
                    // If PNG signature is not found, show an error message
                    MessageBox.Show("The provided file is not a valid PNG image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to convert .dat to .png: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int FindBytes(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool IsPngFile(byte[] data)
        {
            // PNG signature: 89 50 4E 47 0D 0A 1A 0A
            byte[] pngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

            return data.Take(pngSignature.Length).SequenceEqual(pngSignature);
        }

        private void SaveBitmapAsPng(Bitmap bitmap, string filePath)
        {
            System.Drawing.Imaging.EncoderParameters encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100); // Set quality to 100

            ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Png);

            bitmap.Save(filePath, codecInfo, encoderParameters);
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        private void DisplayImage(string imagePath)
        {
            try
            {
                using (FileStream stream = new FileStream(imagePath, FileMode.Open))
                {
                    // Set the SizeMode to AutoSize to handle images of different dimensions
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                    // Assign the original image directly without resizing
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadAndDisplayImage(string ps3IpAddress, string directory, string fileName)
        {
            try
            {
                // Create an FTP request to download the image file
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ps3IpAddress}/{directory}/{fileName}");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream ftpStream = response.GetResponseStream())
                {
                    // Read the image file into a MemoryStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        ftpStream.CopyTo(memoryStream);

                        // Display the image on pictureBox4
                        pictureBox4.Image = Image.FromStream(memoryStream);

                        // Provide an option to save the image
                        DialogResult result = MessageBox.Show("Do you want to save the image?", "Save Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.Filter = "PNG Files|*.png";
                            saveFileDialog.Title = "Save Image";

                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                // Save the image to the selected location
                                pictureBox4.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                                MessageBox.Show("Image saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region FTP-Stuff
        private bool IsDirectory(string entry)
        {
            return entry.EndsWith("/");
        }

        private void ConnectToFtp(string ipAddress, string directory)
        {
            try
            {
                // Create an FTP request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ipAddress}/{directory}");
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                // Set credentials if needed
                // request.Credentials = new NetworkCredential("username", "password");

                // Establish the connection
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                // Display a success message
                MessageBox.Show($"Connected to FTP at {ipAddress}/{directory}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Close the response
                response.Close();
            }
            catch (WebException ex)
            {
                // Display an error message
                MessageBox.Show($"Error connecting to FTP: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool EnsurePsnAvatarFolder(string ftpIp, string selectedAccounts, string ftpDirectory)
        {
            try
            {
                // Construct the full FTP path
                string ftpPath = $"ftp://{ftpIp}/dev_hdd0/home/{selectedAccounts}/{ftpDirectory}";

                // Check if the psn_avatar folder exists
                FtpWebRequest checkRequest = (FtpWebRequest)WebRequest.Create(ftpPath);
                checkRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                checkRequest.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access

                using (FtpWebResponse checkResponse = (FtpWebResponse)checkRequest.GetResponse())
                {
                    // Folder exists
                    Console.WriteLine($"Directory {ftpDirectory} already exists, status {checkResponse.StatusDescription}");
                    return true;
                }
            }
            catch (WebException webEx)
            {
                FtpWebResponse response = webEx.Response as FtpWebResponse;

                if (response != null && response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    // Folder doesn't exist
                    Console.WriteLine($"Directory {ftpDirectory} does not exist, status {response.StatusDescription}");
                    return false;
                }
                else
                {
                    // Other web exception, handle as needed
                    MessageBox.Show($"Error checking psn_avatar folder: {webEx.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                MessageBox.Show($"Error checking psn_avatar folder: {ex.Message}");
                return false;
            }
        }

        private void CreateFtpDirectory(string ftpIp, string selectedAccounts, string ftpDirectory)
        {
            try
            {
                // Construct the full FTP path with the directory
                string ftpPath = $"ftp://{ftpIp}/dev_hdd0/home/{selectedAccounts}/{ftpDirectory}";

                // Create the FTP request to make the directory
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access

                // Get the response (attempt to create the directory)
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Directory {ftpDirectory} created, status {response.StatusDescription}");
                }
            }
            catch (WebException dirEx)
            {
                if (dirEx.Response != null)
                {
                    using (var responseStream = dirEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
                throw; // Rethrow the exception to propagate it to the calling method
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating directory: {ex.Message}");
                throw; // Rethrow the exception to propagate it to the calling method
            }
        }

        private void UploadFileToFTP(string ftpIp, string selectedAccounts, string ftpDirectory, string filePath)
        {
            try
            {
                string ftpPath = $"ftp://{ftpIp}/dev_hdd0/home/{selectedAccounts}/{ftpDirectory}{Path.GetFileName(filePath)}";

                // Create the FTP request to upload the file
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access
                request.UsePassive = true;
                request.KeepAlive = true;

                // Read the file and write it to the request stream with an increased buffer size
                using (Stream fileStream = File.OpenRead(filePath))
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[65536]; // Adjust the buffer size as needed
                    int bytesRead;
                    long fileSize = fileStream.Length;
                    long totalBytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        // Calculate progress percentage
                        int progress = (int)((totalBytesRead * 100) / fileSize);

                        // Update progress label (invoke on UI thread)
                        labelProgress2.Invoke((MethodInvoker)(() =>
                        {
                            labelProgress2.Text = $"Uploading: {progress}%";
                        }));
                    }
                }

                // Hide progress label after upload
                labelProgress.Invoke((MethodInvoker)(() =>
                {
                    labelProgress2.Text = string.Empty;
                }));

                // Get the response (attempt to upload the file)
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"File {Path.GetFileName(filePath)} uploaded, status {response.StatusDescription}");
                }
            }
            catch (WebException uploadEx)
            {
                if (uploadEx.Response != null)
                {
                    using (var responseStream = uploadEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
                throw; // Rethrow the exception to propagate it to the calling method
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading file: {ex.Message}");
                throw; // Rethrow the exception to propagate it to the calling method
            }
        }
       
        private void UpdateProgressLabel(string text)
        {
            if (labelProgress2.InvokeRequired)
            {
                labelProgress2.Invoke((MethodInvoker)(() =>
                {
                    labelProgress2.Text = text;
                }));
            }
            else
            {
                labelProgress2.Text = text;
            }
        }

        private void PopulateGamesComboBox()
        {
            string ipAddress = textBox4.Text.Trim();
            Task.Run(async () =>
            {
                try
                {
                    string[] games = await GetGamesListAsync(ipAddress);

                    BeginInvoke((Action)(() =>
                    {
                        comboBox8.Items.Clear();
                        comboBox8.Items.AddRange(games);

                        if (games.Length > 0)
                        {
                            comboBox8.SelectedIndex = 0;
                        }

                        //MessageBox.Show("Connected successfully and fetched games list.");
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error populating games combo box: {ex.Message}");
                }
            });
        }

        private async Task<string[]> GetGamesListAsync(string ipAddress)
        {
            if (!Uri.IsWellFormedUriString($"ftp://{ipAddress}", UriKind.RelativeOrAbsolute))
            {
                return new string[0];
            }

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ipAddress}{ftpRootDirectory}");
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string directories = await reader.ReadToEndAsync();
                    string[] games = directories
                        .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(game => !game.Equals(".") && !game.Equals(".."))
                        .ToArray();

                    return games;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching list: {ex.Message}");
                return new string[0];
            }
        }

        private void PopulateGamesComboBox(string ipAddress)
        {
            PopulateGamesComboBox();
        }

        private void DownloadFile(string sourceFile, string destinationPath)
        {
            try
            {
                Uri ftpUri = new Uri($"ftp://{sourceFile}");

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access
                request.UsePassive = true;
                request.KeepAlive = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception instead of throwing it
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        private string[] GetFilesInDirectory(string directory)
        {
            List<string> files = new List<string>();

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{directory}");
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access
                request.Timeout = DefaultTimeoutMilliseconds; // Set the timeout value

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    // Read and split the list of files, excluding special entries
                    string fileList = reader.ReadToEnd();
                    files = fileList
                        .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(entry => !entry.Equals(".") && !entry.Equals(".."))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception instead of throwing it
                Console.WriteLine($"Error fetching file list: {ex.Message}");
            }

            return files.ToArray();
        }

        private void DownloadGameFolder(string gameName)
        {
            string ipAddress = textBox4.Text.Trim();
            string ftpGameDirectory = $"{ftpRootDirectory}{gameName}/";

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select Destination Folder";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string destinationFolder = folderBrowserDialog.SelectedPath;
                Task.Run(() => DownloadDirectory($"{ipAddress}{ftpGameDirectory}", destinationFolder))
                    .ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            Console.WriteLine($"Error downloading game folder: {task.Exception.InnerException.Message}");
                        }
                        else
                        {
                            MessageBox.Show($"{gameName} has been downloaded to the selected directory.");
                        }
                    });
            }
            else
            {
                MessageBox.Show("Download canceled by user.");
            }
        }

        private void DownloadDirectory(string sourceDirectory, string destinationDirectory)
        {
            try
            {
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory); // Create destination directory if it doesn't exist
                }

                // Get files and directories in the source directory
                string[] entries = GetFilesInDirectory(sourceDirectory);

                foreach (string entry in entries)
                {
                    string sourceEntry = $"{sourceDirectory}/{entry}";
                    string destinationEntry = Path.Combine(destinationDirectory, entry);

                    // Check if the entry is one of the special folders
                    if (entry.Equals("DRMDIR") || entry.Equals("TROPDIR") || entry.Equals("USRDIR"))
                    {
                        // Recursively download contents inside the special folder
                        DownloadDirectory(sourceEntry, destinationEntry);
                    }
                    else if (!IsDirectory(entry))
                    {
                        // Download file
                        DownloadFile(sourceEntry, destinationEntry);
                    }
                    else
                    {
                        // Recursively download contents inside the directory
                        DownloadDirectory(sourceEntry, destinationDirectory); // Pass sourceDirectory instead of sourceEntry
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception instead of showing it to the user
                Console.WriteLine($"Error downloading directory: {ex.Message}");
            }
        }

        private bool DirectoryExists(string directory)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{directory}");
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential("anonymous", ""); // Assuming anonymous access

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetWebExceptionMessage(WebException ex)
        {
            if (ex.Response != null)
            {
                using (Stream responseStream = ex.Response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            return ex.Message;
        }

        #endregion

        #region Buttons
        private void buttonhe_Click(object sender, EventArgs e)
        {
            string aboutMessage = "Select Art From The Combo\nPaste It In The Custom Notify Box Above!\nYou Can Use The Custom Notify As Normal Notify As Well\n::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n";
            MessageBox.Show(aboutMessage, "XZ UTIL", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttondir_Click(object sender, EventArgs e)
        {
            // Get values from the form controls
            string ps3Ip = ipbox.Text;
            string selectedAccount = comboBoxAcc.SelectedItem?.ToString();

            // Construct the FTP URI for the directory
            string ftpDirectoryUri = $"ftp://{ps3Ip}/dev_hdd0/home/{selectedAccount}/friendim/";

            try
            {
                // Create FtpWebRequest
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpDirectoryUri);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                // Set the FTP request details
                request.Credentials = new NetworkCredential("anonymous", "anonymous@example.com"); // No authentication

                // Get the response
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string directoryContents = reader.ReadToEnd();
                    MessageBox.Show($"Connected to directory!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonsend_Click(object sender, EventArgs e)
        {
            // Get values from the form controls
            string ps3Ip = ipbox.Text;
            string selectedAccount = comboBoxAcc.SelectedItem?.ToString();
            string customComment = commentbox.Text;

            // Construct the FTP URI
            string ftpUri = $"ftp://{ps3Ip}/dev_hdd0/home/{selectedAccount}/friendim/mecomment.dat";

            try
            {
                // Create FtpWebRequest
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Set the FTP request details
                request.UseBinary = true;
                request.Credentials = new NetworkCredential("anonymous", "anonymous@example.com"); // No authentication
                request.ContentLength = customComment.Length;

                // Get the request stream and write the custom comment
                using (Stream requestStream = request.GetRequestStream())
                using (StreamWriter writer = new StreamWriter(requestStream))
                {
                    writer.Write(customComment);
                }

                // Get the response
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    MessageBox.Show($"Upload File Complete, status {response.StatusDescription}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonctest_Click(object sender, EventArgs e)
        {
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = true;
            comboBox9.Visible = true;
            labelcm.Visible = true;
            comboBoxAcc.Visible = true;
            buttonsend.Visible = true;
            buttondir.Visible = true;
            ipbox.Visible = true;
            commentbox.Visible = true;
            groupBoxAP2.Visible = false;
            groupBoxspd.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP.Visible = false;
            groupBoxAPI.Visible = false;
            buttoncmp.Visible = false;
            labelProgress2.Visible = false;
            labelgp.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            labelst.Visible = false;
            browseButton.Visible = false;
            txtBox_history.Visible = false;
            groupPC2.Visible = false;
            groupPC.Visible = false;
            buttonfxc.Visible = false;
            labelm5.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpc.Visible = false;
            labelpl.Visible = false;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = false;
            label31.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            label4.Visible = false;
            button6.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;
        }

        private void buttoncmp_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath1 = filePathTextBox.Text;  // Get the file path from the TextBox

                if (File.Exists(filePath1))  // Check if the file exists
                {
                    // Calculate MD5 and SHA-256 hashes for the first file
                    byte[] md5Hash1 = CalculateMD5(filePath1);
                    byte[] sha256Hash1 = CalculateSHA256(filePath1);

                    // Prompt user to select the second file
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        string filePath2 = openFileDialog.FileName;

                        if (File.Exists(filePath2))
                        {
                            // Calculate MD5 and SHA-256 hashes for the second file
                            byte[] md5Hash2 = CalculateMD5(filePath2);
                            byte[] sha256Hash2 = CalculateSHA256(filePath2);

                            // Compare MD5 hashes
                            bool md5Match = StructuralComparisons.StructuralEqualityComparer.Equals(md5Hash1, md5Hash2);

                            // Compare SHA-256 hashes
                            bool sha256Match = StructuralComparisons.StructuralEqualityComparer.Equals(sha256Hash1, sha256Hash2);

                            // Display the comparison result
                            string comparisonResult = $"MD5 Match: {md5Match}\nSHA-256 Match: {sha256Match}";
                            MessageBox.Show(comparisonResult, "File Comparison Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("File not found: " + filePath2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("File not found: " + filePath1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // If there is an error, generate an error log and show a message box.
                DateTime today = DateTime.Today;
                File.AppendAllText(@"errorLog.txt", today.ToString("d") + " ");
                File.AppendAllText(@"errorLog.txt", DateTime.Now.ToShortTimeString() + ": ");
                File.AppendAllText(@"errorLog.txt", ex.ToString());
                File.AppendAllText(@"errorLog.txt", Environment.NewLine);
                MessageBox.Show("Error comparing files\nCheck errorLog.txt for more details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonfxc_Click(object sender, EventArgs e)
        {
            string aboutMessage = "Browse File Then Check Hashes\nYou Can Also Compare Files!\n\nVerify the integrity of files by comparing the generated hash value with the original hash value.\nMD5 & SHA-256\n::::::::::::::::::::::::::::::::::::::::::::::::::::::::::";
            MessageBox.Show(aboutMessage, "XZ UTIL", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                filePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = filePathTextBox.Text;  // Get the file path from the TextBox

                if (File.Exists(filePath))  // Check if the file exists
                {
                    // Calculate MD5 hash
                    byte[] md5Hash = MD5.Create().ComputeHash(File.ReadAllBytes(filePath));

                    // Calculate SHA-256 hash
                    byte[] sha256Hash = SHA256.Create().ComputeHash(File.ReadAllBytes(filePath));

                    // Display only MD5 and SHA-256 hashes
                    txtBox_history.AppendText("MD5: " + BitConverter.ToString(md5Hash).Replace("-", ""));
                    txtBox_history.AppendText(Environment.NewLine);
                    txtBox_history.AppendText("SHA-256: " + BitConverter.ToString(sha256Hash).Replace("-", ""));
                    txtBox_history.AppendText(Environment.NewLine);
                    txtBox_history.AppendText(Environment.NewLine);
                }
                else
                {
                    MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // If there is an error, generate an error log and show a message box.
                DateTime today = DateTime.Today;
                File.AppendAllText(@"errorLog.txt", today.ToString("d") + " ");
                File.AppendAllText(@"errorLog.txt", DateTime.Now.ToShortTimeString() + ": ");
                File.AppendAllText(@"errorLog.txt", ex.ToString());
                File.AppendAllText(@"errorLog.txt", Environment.NewLine);
                MessageBox.Show("Error processing " + filePathTextBox.Text + "\nCheck errorLog.txt for more details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonpcm_Click(object sender, EventArgs e)
        {
            DisplaySystemInformation();
            UpdateControlsVisibility();
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = false;
            comboBox9.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxspd.Visible = true;
            labelGpu.Visible = true;
            groupBoxAP2.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxAPI.Visible = false;
            groupBoxAP.Visible = false;
            buttoncmp.Visible = true;
            labelip.Visible = true;
            labelipp.Visible = true;
            labelgp.Visible = true;
            labelst.Visible = true;
            groupPC2.Visible = true;
            groupPC.Visible = true;
            txtBox_history.Visible = true;
            buttonfxc.Visible = true;
            labelm5.Visible = true;
            browseButton.Visible = true;
            filePathTextBox.Visible = true;
            md5Label.Visible = true;
            checkButton.Visible = true;
            labelpt3.Visible = true;
            labelpt5.Visible = true;
            labelpt5.Visible = true;
            labelpt7.Visible = true;
            labelpt7.Visible = true;
            labelpt1.Visible = true;
            labelpc.Visible = true;
            labelpt.Visible = true;
            labelpo.Visible = true;
            labelpl.Visible = true;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            label31.Visible = false;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            label4.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;
            pictureBox5.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = false;
            comboBox9.Visible = false;
            comboBoxAcc.Visible = false;
            labelcm.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxspd.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP2.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxAPI.Visible = false;
            groupBoxAP.Visible = false;
            buttoncmp.Visible = false;
            labelProgress2.Visible = false;
            labelm5.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            labelst.Visible = false;
            labelgp.Visible = false;
            groupPC2.Visible = false;
            groupPC.Visible = false;
            txtBox_history.Visible = false;
            buttonfxc.Visible = false;
            browseButton.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpl.Visible = false;
            labelpc.Visible = false;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            label31.Visible = false;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            label4.Visible = false;
            label3.Visible = true;
            button1.Visible = true;
            pictureBox1.Visible = true;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = false;
            comboBox9.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP2.Visible = true;
            groupBoxspd.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP.Visible = false;
            groupBoxAPI.Visible = false;
            buttoncmp.Visible = false;
            labelProgress2.Visible = false;
            labelgp.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            labelst.Visible = false;
            browseButton.Visible = false;
            txtBox_history.Visible = false;
            groupPC2.Visible = false;
            groupPC.Visible = false;
            buttonfxc.Visible = false;
            labelm5.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpc.Visible = false;
            labelpl.Visible = false;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = false;
            label31.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = true;
            comboBox1.Visible = true;
            label9.Visible = true;
            textBox1.Visible = true;
            pictureBox4.Visible = true;
            button9.Visible = true;
            label7.Visible = false;
            label4.Visible = false;
            button6.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            comboBox9.Visible = false;
            labelExplanation.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP2.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxspd.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP.Visible = false;
            groupBoxAPI.Visible = false;
            labelProgress2.Visible = false;
            buttoncmp.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            browseButton.Visible = false;
            labelm5.Visible = false;
            labelst.Visible = false;
            labelgp.Visible = false;
            groupPC2.Visible = false;
            groupPC.Visible = false;
            txtBox_history.Visible = false;
            buttonfxc.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpt.Visible = false;
            labelpc.Visible = false;
            labelpo.Visible = false;
            labelpl.Visible = false;
            IP_Reader.Visible = true;
            textBox5.Visible = true;
            label8.Visible = false;
            label31.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            label4.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;

            if (isConnected)
            {
                // Show connected elements directly
                UpdateControlsVisibility();
            }
            else
            {
                // Show connection-related controls
                UpdateControlsVisibility();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            buttonsd.Visible = false;
            comboBox9.Visible = false;
            labelExplanation.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP2.Visible = false;
            groupBoxspd.Visible = false;
            groupBoxAP1.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP.Visible = true;
            groupBoxAPI.Visible = true;
            buttoncmp.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            labelm5.Visible = false;
            groupPC2.Visible = false;
            labelgp.Visible = false;
            labelst.Visible = false;
            groupPC.Visible = false;
            txtBox_history.Visible = false;
            buttonfxc.Visible = false;
            browseButton.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpc.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpl.Visible = false;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = false;
            label31.Visible = false;
            textBox4.Visible = false;
            comboBox8.Visible = false;
            buttongs2.Visible = false;
            buttongs3.Visible = false;
            ConnectionBox.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = true;
            ConnectBtn.Visible = true;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            radioButton1.Visible = true;
            label4.Visible = true;
            button9.Visible = false;
            textBoxIPAddress.Visible = true;
            button7.Visible = true;
            label5.Visible = true;
            button6.Visible = true;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            buttoncc.Visible = false;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.ControlConsole);
            this.SelectedAPI_Properties();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.TargetManager);
            this.SelectedAPI_Properties();
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.PS3Manager);
            this.SelectedAPI_Properties();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
                {
                    try
                    {
                        PS3.ConnectTarget();
                        this.ConnectionStatusSuccess();

                        this.StatusLabel.Text = "Connected";
                        this.StatusLabel.ForeColor = Color.Green;
                    }
                    catch
                    {
                        this.StatusLabel.Text = "Not connected";
                        this.StatusLabel.ForeColor = Color.Red;
                    }
                }
                else if (PS3.ConnectTarget(this.textBoxIPAddress.Text))
                {
                    PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.PROGRESS, "Connected Seccessfully");
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);
                    this.ConnectionStatusSuccess();
                    this.API = this.textBoxIPAddress.Text;
                }
                else
                    this.ConnectionStatusFailed();
            }
            else
            {
                PS3.ChangeAPI(SelectAPI.PS3Manager);
                PS3M_API.ConnectTarget(this.textBoxIPAddress.Text, Convert.ToInt32(7887));
                try
                {
                    if (!PS3M_API.IsConnected)
                        return;
                    this.AttachMethod.Items.Clear();
                    foreach (uint pidProcess in PS3M_API.Process.GetPidProcesses())
                    {
                        if (pidProcess != 0U)
                        {
                            this.AttachMethod.Items.Add((object)PS3M_API.Process.GetName(pidProcess));
                            PS3M_API.PS3.Notify("Connected Seccessfully");
                            PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                            this.ConnectionStatusSuccess();
                        }
                        else
                            break;
                    }
                    this.AttachMethod.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    int num = (int)MessageBox.Show((IWin32Window)this, ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string ps3IpAddress = textBoxIPAddress.Text;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PKG Files|*.pkg";
                openFileDialog.Title = "Select a PKG File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pkgFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(pkgFilePath);

                    // Show progress label
                    labelProgress.Text = "Uploading...";
                    labelProgress.Visible = true;

                    await Task.Run(() =>
                    {
                        using (FileStream fileStream = File.OpenRead(pkgFilePath))
                        {
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ps3IpAddress}/dev_hdd0/packages/{fileName}");
                            request.Method = WebRequestMethods.Ftp.UploadFile;

                            using (Stream ftpStream = request.GetRequestStream())
                            {
                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                long fileSize = fileStream.Length;
                                long totalBytesRead = 0;

                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ftpStream.Write(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    // Calculate progress percentage
                                    int progress = (int)((totalBytesRead * 100) / fileSize);

                                    // Update progress label (invoke on UI thread)
                                    labelProgress.Invoke((MethodInvoker)(() =>
                                    {
                                        labelProgress.Text = $"Uploading: {progress}%";
                                    }));
                                }
                            }
                        }
                    });

                    // Hide progress label after upload
                    labelProgress.Visible = false;

                    MessageBox.Show($"File {fileName} uploaded successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string ps3IpAddress = textBox1.Text;

            // Get the selected account number from the ComboBox
            string selectedAccount = comboBox1.SelectedItem as string;

            if (selectedAccount != null)
            {
                // Specify the directory and file name based on the selected account number
                string directory = $"/dev_hdd0/home/{selectedAccount}/friendim/avatar";
                string fileName = "me.png";

                // Call the method to download and display the image
                DownloadAndDisplayImage(ps3IpAddress, directory, fileName);
            }
            else
            {
                MessageBox.Show("Please select an account number from the ComboBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DAT Files|*.dat";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string datFilePath = openFileDialog.FileName;
                isOpenedByButton = true;
                ConvertDatToPng(datFilePath);
                isOpenedByButton = false;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                string selectedOption = comboBox3.SelectedItem?.ToString();

                if (selectedOption == "Shut Down")
                    PS3.CCAPI.ShutDown(PS3Lib.CCAPI.RebootFlags.ShutDown);
                else if (selectedOption == "Soft Reboot")
                    PS3.CCAPI.ShutDown(PS3Lib.CCAPI.RebootFlags.SoftReboot);
                else if (selectedOption == "Hard Reboot")
                    PS3.CCAPI.ShutDown(PS3Lib.CCAPI.RebootFlags.HardReboot);
            }
            else if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
            {
                PS3.TMAPI.PowerOff(true);
            }
            else
            {
                if (PS3.GetCurrentAPI() != SelectAPI.PS3Manager)
                    return;

                string selectedOption = comboBox3.SelectedItem?.ToString();

                if (selectedOption == "Shut Down")
                    PS3.PS3MAPI.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.ShutDown);
                else if (selectedOption == "Soft Reboot")
                    PS3.PS3MAPI.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.SoftReboot);
                else if (selectedOption == "Hard Reboot")
                    PS3.PS3MAPI.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.HardReboot);
                else if (selectedOption == "Quick Reboot")
                    PS3.PS3MAPI.Power(PS3ManagerAPI.PS3MAPI.PS3_CMD.PowerFlags.QuickReboot);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                if (this.GreenOff.Checked)
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Green, PS3Lib.CCAPI.LedMode.Off);
                if (this.GreenOn.Checked)
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Green, PS3Lib.CCAPI.LedMode.On);
                if (this.RedOff.Checked)
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Red, PS3Lib.CCAPI.LedMode.Off);
                if (this.RedOn.Checked)
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Red, PS3Lib.CCAPI.LedMode.On);
                    return;

            }
            else
            {
                if (PS3.GetCurrentAPI() != SelectAPI.PS3Manager)
                    return;
                if (this.GreenOff.Checked)
                    PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                if (this.GreenOn.Checked)
                    PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                if (this.RedOff.Checked)
                    PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                if (this.RedOn.Checked)
                    PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    return;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string title = "| XZ UTIL |";
            string message = richTextBox1.Text;

            NotifyWithRichTextBox(title, message, PS3Lib.CCAPI.NotifyIcon.CAUTION);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string selectedBuzzerOption = comboBox5.SelectedItem?.ToString();

            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                if (selectedBuzzerOption == "Single")
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);
                else if (selectedBuzzerOption == "Double")
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Double);
                else if (selectedBuzzerOption == "Triple")
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Triple);
                else if (selectedBuzzerOption == "Continuous")
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Continuous);
            }
            else
            {
                if (PS3.GetCurrentAPI() != SelectAPI.PS3Manager)
                    return;

                if (selectedBuzzerOption == "Single")
                    PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                else if (selectedBuzzerOption == "Double")
                    PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Double);
                else if (selectedBuzzerOption == "Triple")
                    PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Triple);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ExecuteCustomNotify();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.procs = new uint[64];
            PS3.CCAPI.GetProcessList(out this.procs);
            this.comboBox6.Items.Clear();
            for (int index = 0; index < this.procs.Length; ++index)
            {
                string name = string.Empty;
                PS3.CCAPI.GetProcessName(this.procs[index], out name);
                comboBox6.Items.Add((object)name);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            progressBar1.Visible = false;
            labelProgress.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = false;
            comboBox9.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP2.Visible = false;
            label8.Visible = false;
            groupBoxspd.Visible = false;
            buttongs3.Visible = false;
            labelGpu.Visible = false;
            buttongs2.Visible = false;
            groupBoxAP2.Visible = false;
            groupBoxAP1.Visible = false;
            groupBoxAP0.Visible = true;
            groupBoxAP.Visible = false;
            groupBoxAPI.Visible = true;
            buttoncmp.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            labelm5.Visible = false;
            labelgp.Visible = false;
            labelst.Visible = false;
            groupPC2.Visible = false;
            groupPC.Visible = false;
            browseButton.Visible = false;
            txtBox_history.Visible = false;
            buttonfxc.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpc.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpl.Visible = false;
            label30.Visible = true;
            label29.Visible = true;
            label28.Visible = true;
            textBox3.Visible = true;
            button17.Visible = true;
            button18.Visible = true;
            comboBox7.Visible = true;
            radioButton4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label19.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            ConnectBtn.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            radioButton1.Visible = false;
            label4.Visible = false;
            button9.Visible = false;
            textBoxIPAddress.Visible = false;
            button7.Visible = false;
            label5.Visible = false;
            button6.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.ControlConsole);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.PS3Manager);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (this.radioButton4.Checked)
            {
                if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
                {
                    try
                    {
                        PS3.ConnectTarget();
                        this.ConnectionStatusSuccess();

                        this.StatusLabel.Text = "Connected";
                        this.StatusLabel.ForeColor = Color.Green;
                    }
                    catch
                    {
                        this.StatusLabel.Text = "Not connected";
                        this.StatusLabel.ForeColor = Color.Red;
                    }
                }
                else if (PS3.ConnectTarget(this.textBox3.Text))
                {
                    PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.PROGRESS, "Connected Seccessfully");
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);
                    this.ConnectionStatusSuccess();
                    this.API = this.textBoxIPAddress.Text;
                }
                else
                    this.ConnectionStatusFailed();
            }
            else
            {
                PS3.ChangeAPI(SelectAPI.PS3Manager);
                PS3M_API.ConnectTarget(this.textBox3.Text, Convert.ToInt32(7887));
                try
                {
                    if (!PS3M_API.IsConnected)
                        return;
                    this.AttachMethod.Items.Clear();
                    foreach (uint pidProcess in PS3M_API.Process.GetPidProcesses())
                    {
                        if (pidProcess != 0U)
                        {
                            this.AttachMethod.Items.Add((object)PS3M_API.Process.GetName(pidProcess));
                            PS3M_API.PS3.Notify("Connected Seccessfully");
                            PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                            this.ConnectionStatusSuccess();

                        }
                        else
                            break;
                    }
                    this.AttachMethod.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    int num = (int)MessageBox.Show((IWin32Window)this, ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            string ftpIp = textBox3.Text;
            string selectedAccounts = comboBox7.SelectedItem?.ToString();
            string ftpDirectory = "psn_avatar/";

            try
            {
                if (!EnsurePsnAvatarFolder(ftpIp, selectedAccounts, ftpDirectory))
                {
                    // Folder doesn't exist, attempt to create it
                    CreateFtpDirectory(ftpIp, selectedAccounts, ftpDirectory);
                    Console.WriteLine($"Directory {ftpDirectory} created.");
                }

                MessageBox.Show("psn_avatar folder checked/created successfully.");

                // Now, show the file dialog for uploading files
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select .edat files";
                openFileDialog.Filter = "EDAT Files|*.edat";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        await Task.Run(() => UploadFileToFTP(ftpIp, selectedAccounts, ftpDirectory, filePath));
                    }

                    MessageBox.Show("File(s) uploaded successfully.");
                }
            }
            catch (WebException dirEx)
            {
                if (dirEx.Response != null)
                {
                    using (var responseStream = dirEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }

                MessageBox.Show("Error checking/creating psn_avatar folder.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking/creating psn_avatar folder: {ex.Message}");
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void AttachMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.AttachMethod.SelectedIndex == 0)
            {
                PS3.ChangeAPI(SelectAPI.ControlConsole);
            }
            else if (this.AttachMethod.SelectedIndex == 1)
            {
                PS3.ChangeAPI(SelectAPI.TargetManager);
            }
            else
            {
                if (this.AttachMethod.SelectedIndex != 2)
                    return;
                PS3.ChangeAPI(SelectAPI.PS3Manager);
            }
        }

        private void buttoncc_Click(object sender, EventArgs e)
        {
            if (this.CCAPI.Checked)
            {
                if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
                {
                    try
                    {
                        PS3.ConnectTarget(this.textBox5.Text);
                        PS3.AttachProcess();
                        this.StatusLabel.Text = "Connected";
                        this.StatusLabel.ForeColor = Color.Green;

                        PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);

                        string[] firstMessage = {
    "+=========================+",
    "|            Welcome to XZ UTIL          |",
    "+=========================+"
};

                        string combinedFirstMessage = string.Join("\n", firstMessage);
                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.CAUTION, combinedFirstMessage);

                        string[] secondMessage = {
    "+=========================+",
    "|              Created by: WMP            |",
    "+=========================+"
};

                        string combinedSecondMessage = string.Join("\n", secondMessage);

                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.INFO, combinedSecondMessage);

                        string[] thirdMessage = {
    "+=========================+",
    "|               WWW.XREZ.IO               |",
    "+=========================+"
};

                        string combinedThirdMessage = string.Join("\n", thirdMessage);

                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.ARROWRIGHT, combinedThirdMessage);

                        // Show connected elements

                        PS3.CCAPI.ClearTargetInfo();

                        this.CELL_Temperature.Text = PS3.CCAPI.GetTemperatureCELL();
                        this.RSX_Temperature.Text = PS3.CCAPI.GetTemperatureRSX();
                        this.Firmware.Text = PS3.CCAPI.GetFirmwareVersion();
                        this.Console_type.Text = PS3.CCAPI.GetFirmwareType();
                        this.label23.Text = PS3.CCAPI.GetDllVersion().ToString();

                        groupBox12.Visible = true;
                        groupBox13.Visible = true;
                        buttonsd.Visible = true;
                        label14.Visible = true;
                        button15.Visible = true;
                        comboBox6.Visible = true;
                        label24.Visible = true;
                        label23.Visible = true;
                        label21.Visible = true;
                        richTextBox1.Visible = true;
                        label20.Visible = true;
                        RSX_Temperature.Visible = true;
                        CELL_Temperature.Visible = true;
                        Console_type.Visible = true;
                        label7.Visible = true;
                        label15.Visible = true;
                        label16.Visible = true;
                        button10.Visible = true;
                        button8.Visible = true;
                        button11.Visible = true;
                        button12.Visible = true;
                        comboBox5.Visible = true;
                        comboBox3.Visible = true;
                        label13.Visible = true;
                        label12.Visible = true;
                        label11.Visible = true;
                        Firmware.Visible = true;

                        // Hide connection-related elements
                        buttoncc.Visible = false;
                        label19.Visible = false;
                        label17.Visible = false;
                        ConnectionBox.Visible = false;
                        CCAPI.Visible = false;
                        IPtextBox.Visible = false;
                        IP_Reader.Visible = false;
                        ConnectBtn.Visible = false;
                    }
                    catch
                    {
                        this.ConnectionStatusFailed();
                    }
                }
            }
            else
            {
                PS3.ChangeAPI(SelectAPI.PS3Manager);
                try
                {
                    if (PS3M_API.ConnectTarget(this.textBox5.Text, 7887))
                    {
                        if (PS3M_API.IsConnected)
                        {
                            this.AttachMethod.Items.Clear();

                            foreach (uint pidProcess in PS3M_API.Process.GetPidProcesses())
                            {
                                if (pidProcess != 0U)
                                {
                                    this.AttachMethod.Items.Add(PS3M_API.Process.GetName(pidProcess));
                                }
                                else
                                {
                                    break;
                                }
                            }

                            MainForm.PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                            this.ConnectionStatusSuccess();

                            groupBox12.Visible = true;
                            groupBox13.Visible = true;
                            buttonsd.Visible = true;
                            label14.Visible = true;
                            button15.Visible = true;
                            comboBox6.Visible = true;
                            label24.Visible = true;
                            label23.Visible = true;
                            label21.Visible = true;
                            richTextBox1.Visible = true;
                            label20.Visible = true;
                            RSX_Temperature.Visible = true;
                            CELL_Temperature.Visible = true;
                            Console_type.Visible = true;
                            label7.Visible = true;
                            label15.Visible = true;
                            label16.Visible = true;
                            button10.Visible = true;
                            button8.Visible = true;
                            button11.Visible = true;
                            button12.Visible = true;
                            comboBox5.Visible = true;
                            comboBox3.Visible = true;
                            label13.Visible = true;
                            label12.Visible = true;
                            label11.Visible = true;
                            Firmware.Visible = true;

                            // Hide connection-related elements
                            buttoncc.Visible = false;
                            label19.Visible = false;
                            label17.Visible = false;
                            ConnectionBox.Visible = false;
                            CCAPI.Visible = false;
                            IPtextBox.Visible = false;
                            IP_Reader.Visible = false;
                            ConnectBtn.Visible = false;

                        }
                        else
                        {
                            this.ConnectionStatusFailed();
                        }
                    }
                    else
                    {
                        this.ConnectionStatusFailed();
                    }
                }
                catch (Exception ex)
                {
                    this.ConnectionStatusFailed();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConnectionStatusSuccess()
        {
            
        }

        private void ConnectionStatusFailed()
        {
            int num = (int)MessageBox.Show((IWin32Window)this, "Make sure you choose your right ip, or check your Internet connection", "Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void buttongs_Click(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
            groupBox12.Visible = false;
            groupBox13.Visible = false;
            buttonsd.Visible = false;
            labelExplanation.Visible = false;
            comboBox9.Visible = false;
            labelcm.Visible = false;
            comboBoxAcc.Visible = false;
            buttonsend.Visible = false;
            buttondir.Visible = false;
            ipbox.Visible = false;
            commentbox.Visible = false;
            groupBoxAP2.Visible = false;
            groupBoxspd.Visible = false;
            labelGpu.Visible = false;
            groupBoxAP0.Visible = false;
            groupBoxAP1.Visible = true;
            groupBoxAPI.Visible = false;
            groupBoxAP.Visible = false;
            labelProgress2.Visible = false;
            labelm5.Visible = false;
            buttoncmp.Visible = false;
            labelst.Visible = false;
            labelip.Visible = false;
            labelipp.Visible = false;
            groupPC2.Visible = false;
            labelgp.Visible = false;
            groupPC.Visible = false;
            txtBox_history.Visible = false;
            buttonfxc.Visible = false;
            browseButton.Visible = false;
            filePathTextBox.Visible = false;
            md5Label.Visible = false;
            checkButton.Visible = false;
            labelpt3.Visible = false;
            labelpt5.Visible = false;
            labelpt5.Visible = false;
            labelpt7.Visible = false;
            labelpt7.Visible = false;
            labelpt1.Visible = false;
            labelpc.Visible = false;
            labelpt.Visible = false;
            labelpo.Visible = false;
            labelpl.Visible = false;
            IP_Reader.Visible = false;
            textBox5.Visible = false;
            label8.Visible = true;
            label31.Visible = true;
            textBox4.Visible = true;
            comboBox8.Visible = true;
            buttongs2.Visible = true;
            buttongs3.Visible = true;
            ConnectionBox.Visible = false;
            buttoncc.Visible = false;
            label30.Visible = false;
            label29.Visible = false;
            label28.Visible = false;
            textBox3.Visible = false;
            button17.Visible = false;
            button18.Visible = false;
            label28.Visible = false;
            comboBox7.Visible = false;
            radioButton4.Visible = false;
            pictureBox5.Visible = true;
            pictureBox6.Visible = false;
            labelProgress.Visible = false;
            label14.Visible = false;
            button15.Visible = false;
            comboBox6.Visible = false;
            label24.Visible = false;
            label23.Visible = false;
            label21.Visible = false;
            richTextBox1.Visible = false;
            label20.Visible = false;
            button8.Visible = false;
            comboBox5.Visible = false;
            label17.Visible = false;
            label19.Visible = false;
            ConnectionBox.Visible = false;
            CCAPI.Visible = false;
            IPtextBox.Visible = false;
            IP_Reader.Visible = false;
            ConnectBtn.Visible = false;
            label17.Visible = false;
            ConnectionBox.Visible = false;
            label16.Visible = false;
            button12.Visible = false;
            comboBox3.Visible = false;
            label15.Visible = false;
            button11.Visible = false;
            button10.Visible = false;
            Console_type.Visible = false;
            CELL_Temperature.Visible = false;
            RSX_Temperature.Visible = false;
            label13.Visible = false;
            label12.Visible = false;
            label11.Visible = false;
            Firmware.Visible = false;
            label7.Visible = false;
            label10.Visible = false;
            comboBox1.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            pictureBox4.Visible = false;
            button9.Visible = false;
            label7.Visible = false;
            label4.Visible = false;
            button6.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            radioButton1.Visible = false;
            textBoxIPAddress.Visible = false;
            label5.Visible = false;
        }

        private void buttongs2_Click(object sender, EventArgs e)
        {
            string ipAddress = textBox4.Text.Trim();

            // Validate the IP address or hostname
            if (!Uri.IsWellFormedUriString($"ftp://{ipAddress}", UriKind.RelativeOrAbsolute))
            {

                return;
            }

            // Connect to FTP and fetch the list of games
            PopulateGamesComboBox(ipAddress);
        }

        private void buttongs3_Click(object sender, EventArgs e)
        {
            string selectedGame = comboBox8.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedGame))
            {
                MessageBox.Show("Please select a game.");
                return;
            }

            // Connect to FTP and download the game folder
            DownloadGameFolder(selectedGame);
        }
        
        private void buttonsd_Click(object sender, EventArgs e)
        {
            if (MainForm.PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                MainForm.PS3.CCAPI.ClearTargetInfo();
                this.CELL_Temperature.Text = MainForm.PS3.CCAPI.GetTemperatureCELL();
                this.RSX_Temperature.Text = MainForm.PS3.CCAPI.GetTemperatureRSX();
                this.Firmware.Text = MainForm.PS3.CCAPI.GetFirmwareVersion();
                this.Console_type.Text = MainForm.PS3.CCAPI.GetFirmwareType();
            }
            else
            {
                if (MainForm.PS3.GetCurrentAPI() != SelectAPI.PS3Manager || !MainForm.PS3M_API.IsConnected)
                    return;
                uint cpu;
                uint rsx;
                MainForm.PS3M_API.PS3.GetTemperature(out cpu, out rsx);
                this.CELL_Temperature.Text = Convert.ToString(cpu);
                this.RSX_Temperature.Text = Convert.ToString(rsx);
                this.Firmware.Text = Convert.ToString(MainForm.PS3M_API.PS3.GetFirmwareVersion_Str().ToString());
                this.Console_type.Text = MainForm.PS3M_API.PS3.GetFirmwareType();
            }
        }

        private void buttongd_Click(object sender, EventArgs e)
        {
            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole || PS3.GetCurrentAPI() == SelectAPI.PS3Manager)
            {
                try
                {
                    PS3.AttachProcess();
                    this.StatusLabel.Text = "Connected + Attached";
                    this.StatusLabel.ForeColor = Color.Green;
                }
                catch
                {
                    this.StatusLabel.Text = "Not attached";
                    this.StatusLabel.ForeColor = Color.Red;
                }
            }
            else if (PS3.GetCurrentAPI() == SelectAPI.PS3Manager)
            {
                try
                {
                    MainForm.PS3M_API.AttachProcess(MainForm.PS3M_API.Process.Processes_Pid[0]);
                    if (MainForm.PS3M_API.IsAttached)
                    {
                        //mods
                        this.StatusLabel.Text = "Connected + Attached";
                        this.StatusLabel.ForeColor = Color.Green;
                    }
                }
                catch
                {
                    this.StatusLabel.Text = "Not attached";
                    this.StatusLabel.ForeColor = Color.Red;
                }
            }
        }
        #endregion

        #region Other-Stuff
        private List<ArtOption> asciiArtOptions = new List<ArtOption>
        {
            new ArtOption("Dead Face", @" _______
/          \
|  () ()   |
\  ~~~  /
 \_____/
            "),
            new ArtOption("Spooderman", @"______
/         \
|   \  /   |
\   \/    /
\_____/
            "),
            new ArtOption("1337", @"_   _____  _____   _____
/ | |___  /  |___ /  |___  |
| |    |_  \    |_  \      / /
| |  ___) |  ___) |     / /
|_| |____/  |____/   /_/
            "),
            new ArtOption("XZ Util", @"
__  __ ____    _   _   | |_    (_) | |
\ \/ /  |_  /   | | | |   | |__|  | | | |
 >  <   / /    | |_| |   | |_    | | | |
/_/\_\ /___|   \__,_|   \__|   |_| |_|
            "),
            new ArtOption("Bunny", @"
   (\__/)
  (='.'=)
  (.)__(.)
            "),
            new ArtOption("Hax", @" _   _       _       __  __
| | | |      / \      \ \ / /
| |_| |    / _ \      \   / 
|  _  |   / ___ \    /   \ 
|_| |_| /_/    \_\ /_/ \_\
            "),
            new ArtOption("Bong", @"   ___     (
  |   |   ( )
  |   |__))
  |    __|
  |___|
            "),
            new ArtOption("Dog #1", @"   / \__
  (    @\___
  /         O
 /   (_____/
/_____/   U
            "),
            new ArtOption("Dog #2", @"  __
 /''|\_____/)
 \_/|_)     )
    \  __  /
    (_/ (_/
            "),
            // Add more ASCII art options here...
        };

        public class ArtOption
        {
            public string DisplayText { get; set; }
            public string AsciiArt { get; set; }

            public ArtOption(string displayText, string asciiArt)
            {
                DisplayText = displayText;
                AsciiArt = asciiArt;
            }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private void PopulateComboBox()
        {
            
        }

        private void comboBox9_Click(object sender, EventArgs e)
        {
            string selectedSymbolName = comboBox9.SelectedItem?.ToString();

            // Find the corresponding symbol using the dictionary
            if (symbolDictionary.ContainsValue(selectedSymbolName))
            {
                string selectedSymbol = symbolDictionary.First(pair => pair.Value == selectedSymbolName).Key;

                // Append the selected symbol to the existing text in the comment box
                commentbox.Text += selectedSymbol;
            }
        }

        private void comboBox9_DropDown(object sender, EventArgs e)
        {
            PopulateSymbolsComboBox();
        }

        private void PopulateSymbolsComboBox()
        {
            // Clear the combo box items
            comboBox9.Items.Clear();

            // Add symbols to the combo box
            foreach (var symbolPair in symbolDictionary)
            {
                comboBox9.Items.Add(symbolPair.Value);
            }
        }

        private Dictionary<string, string> symbolDictionary = new Dictionary<string, string>
        {
        { "","RAF Logo (NOT FOR HEN)" },
        { "", "CFW Logo (NOT FOR HEN)" },
        { "", "HEN Logo (NOT FOR HEN)" },
        { "", "SPRX Logo (NOT FOR HEN)" },
        { "", "ALL Logo (NOT FOR HEN)" },
        { "", "PSVITA Logo" },
        { "", "PS3 Logo" },
        { "", "PS4 Logo" },
        { "", "Flashing Triangle" },
        { "", "DPAD Right" },
        { "", "L1" },
        { "", "L2" },
        { "", "L3" },
        { "", "R1" },
        { "", "R2" },
        { "", "R3" },
        { "", "Select" },
        { "", "Start" },
        { "", "PSButton" },
        { "", "L2/DPAD Left" },
        { "", "R2/DPAD Right" },
        { "", "Speaker0" },
        { "", "Speaker1" },
        { "", "Speaker2" },
        { "", "Speaker3" },
        { "", "Mic Muted" },
        { "", "HQ" },
        { "", "Arrow Right" },
        { "", "0/3 Battery" },
        { "", "1/3 Battery" },
        { "", "2/3 Battery" },
        { "", "3/3 Battery" },
        { "", "Loading Battery (Moving)" },
        { "", "Bronze Trophy" },
        { "", "Silver Trophy" },
        { "", "Gold Trophy" },
        { "", "Platin Trophy" },
        { "", "Caps" },
        { "", "Keyboard" },
        { "", "Finger/Mouse" },
        { "", "Circle in Circle" },
        { "", "Circle" },
        { "", "PSPlus" },
        { "", "Headset" },
        { "", "Headset in Circle" },
        { "", "2 Hearts (Moving)" },
        { "", "Broken Heart (Moving)" },
        { "", "Heart with Arrow (Moving)" },
        { "", "Heart" },
        { "", "Spades" },
        { "", "Clubs" },
        { "", "Diamond" },
        { "", "!" },
        { "", "!!" },
        { "", "?" },
        { "", "?! " },
        { "", "Light Bulb" },
        { "", "Explosion (Moving)" },
        { "", "3 Drops (Moving)" },
        { "", "Drop (Moving)" },
        { "", "Blast" },
        { "", "Flower" },
        { "", "Skull" },
        { "", "Stars (Moving)" },
        { "", "Fire (Moving)" },
        { "", "Note" },
        { "", "Note1" },
        { "", "Note2" },
        { "", "Note3" },
        { "", "2Notes" },
        { "", "2Notes1" },
        { "", "Lips" },
        { "", "Lips with Tongue" },
        { "", "Kissing Lips" },
        { "", "Nose" },
        { "", "Ear" },
        { "", "Feet" },
        { "", "Feet1" },
        { "", "Sun (Moving)" },
        { "", "Flowers (Moving)" },
        { "", "Umbrella" },
        { "", "Rainbow" },
        { "", "Umbreall Raining (Moving)" },
        { "", "Snowman (Moving)" },
        { "", "Storm (Moving)" },
        { "", "Tornado" },
        { "", "Closed Umbrella" },
        { "", "Snowflake" },
        { "", "Bouquet" },
        { "", "Ring" },
        { "", "Loop" },
        { "", "Present" },
        { "", "Cake" },
        { "", "Money" },
        { "", "¥" },
        { "", "$" }
        };

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Unsubscribe from the event temporarily
            comboBox9.SelectedIndexChanged -= comboBox9_SelectedIndexChanged;

            string selectedSymbolName = comboBox9.SelectedItem?.ToString();

            // Find the corresponding symbol using the dictionary
            if (symbolDictionary.ContainsValue(selectedSymbolName))
            {
                string selectedSymbol = symbolDictionary.First(pair => pair.Value == selectedSymbolName).Key;

                // Append the selected symbol to the existing text in the comment box
                commentbox.Text += selectedSymbol;
            }

            // Subscribe to the event again
            comboBox9.SelectedIndexChanged += comboBox9_SelectedIndexChanged;
        }

        private void ToggleVisibility(bool isVisible, params Control[] controls)
        {
            foreach (var control in controls)
            {
                control.Visible = isVisible;
            }
        }

        private void DisplaySystemInformation()
        {
            try
            {
                // Create a ManagementObjectSearcher to query system information
                ManagementObjectSearcher computerSystemSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

                // Get the first result (assuming there is only one result)
                ManagementObjectCollection computerSystemCollection = computerSystemSearcher.Get();
                ManagementObject computerSystemInfo = computerSystemCollection.OfType<ManagementObject>().FirstOrDefault();

                // Display information on labels or other UI elements
                if (computerSystemInfo != null)
                {
                    labelpt3.Text = $"{Environment.UserName}";
                    labelpt3.ForeColor = Color.LightBlue;

                    // Additional information
                    labelpt5.Text = $"{GetProcessorInformation()}";
                    labelpt5.ForeColor = Color.LightBlue;
                    labelpt7.Text = $"{GetOperatingSystem()}";
                    labelpt7.ForeColor = Color.LightBlue;
                    labelpt1.Text = $"{GetSystemType()}";
                    labelpt1.ForeColor = Color.LightBlue;

                    // Display GPU information
                    labelgp.Text = $"{GetGPUInformation()}";
                    labelgp.ForeColor = Color.LightBlue;

                    // Display IP address information
                    labelipp.Text = GetSystemUptime();
                    labelipp.ForeColor = Color.LightBlue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSystemUptime()
        {
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);

            return $"{uptime.Days} days, {uptime.Hours} hours, \n{uptime.Minutes} minutes, {uptime.Seconds} seconds";
        }

        private string GetGPUInformation()
        {
            try
            {
                ManagementObjectSearcher gpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

                foreach (ManagementObject gpuInfo in gpuSearcher.Get())
                {
                    string gpuName = $"{gpuInfo["Description"]}";
                    return $"{gpuName}";
                }
            }
            catch (Exception ex)
            {
                return $"Error fetching GPU information: {ex.Message}";
            }

            return "GPU Information not available";
        }

        private string GetProcessorInformation()
        {
            try
            {
                // Create a ManagementObjectSearcher to query processor information
                ManagementObjectSearcher processorSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

                // Get the first result (assuming there is only one result)
                ManagementObjectCollection processorCollection = processorSearcher.Get();
                ManagementObject processorInfo = processorCollection.OfType<ManagementObject>().FirstOrDefault();

                // Display processor name, breaking into lines if it's too long
                if (processorInfo != null)
                {
                    string processorName = $"{processorInfo["Name"]}";

                    // Set a maximum length for the displayed processor name
                    int maxLength = 40;

                    // Check if the processor name exceeds the maximum length
                    if (processorName.Length > maxLength)
                    {
                        // Break the line after maxLength characters
                        processorName = processorName.Substring(0, maxLength) + "\n" + processorName.Substring(maxLength);
                    }

                    return processorName;
                }
                else
                {
                    return "N/A";
                }
            }
            catch (Exception ex)
            {
                return $"Error fetching processor information: {ex.Message}";
            }
        }

        private byte[] CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(File.ReadAllBytes(filePath));
            }
        }

        private byte[] CalculateSHA256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(File.ReadAllBytes(filePath));
            }
        }

        private string GetOperatingSystem()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectCollection collection = searcher.Get();
            ManagementObject osInfo = collection.OfType<ManagementObject>().FirstOrDefault();

            return osInfo != null ? $"{osInfo["Caption"]} {osInfo["Version"]}" : "N/A";
        }

        private string GetSystemType()
        {
            return Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
        }

        private string FormatBytes(ulong bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RSX_Temperature_Click(object sender, EventArgs e)
        {
            RSX_Temperature.Text = PS3.CCAPI.GetTemperatureRSX();
        }

        private void CELL_Temperature_Click(object sender, EventArgs e)
        {
            CELL_Temperature.Text = PS3.CCAPI.GetTemperatureCELL();
        }

        private void Console_type_Click(object sender, EventArgs e)
        {
            Console_type.Text = PS3.CCAPI.GetFirmwareVersion();
        }

        private void Firmware_Click(object sender, EventArgs e)
        {
            Firmware.Text = PS3.CCAPI.GetFirmwareType();
        }
        #endregion

        #region PS3 Connection
        private void SelectedAPI_Properties()
        {
            if (isConnectedOrAttached)
                if (CCAPI.Checked || AttachMethod.SelectedIndex == 0)
                {

                }
        }

        private void CCAPI_CheckedChanged(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.ControlConsole);
            this.SelectedAPI_Properties();
        }

        private void TMAPI_CheckedChanged(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.TargetManager);
            this.SelectedAPI_Properties();
        }

        private void PS3MAPI_CheckedChanged(object sender, EventArgs e)
        {
            PS3.ChangeAPI(SelectAPI.PS3Manager);
            this.SelectedAPI_Properties();
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (this.CCAPI.Checked)
            {
                if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
                {
                    try
                    {
                        PS3.ConnectTarget();
                        this.ConnectionStatusSuccess();
                        string[] firstMessage = {
    "+=========================+",
    "|            Welcome to XZ UTIL          |",
    "+=========================+"
};

                        string combinedFirstMessage = string.Join("\n", firstMessage);
                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.CAUTION, combinedFirstMessage);

                        string[] secondMessage = {
    "+=========================+",
    "|              Created by: WMP            |",
    "+=========================+"
};

                        string combinedSecondMessage = string.Join("\n", secondMessage);

                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.INFO, combinedSecondMessage);


                        string[] thirdMessage = {
    "+=========================+",
    "|               WWW.XREZ.IO               |",
    "+=========================+"
};

                        string combinedThirdMessage = string.Join("\n", thirdMessage);

                        PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.ARROWRIGHT, combinedThirdMessage);

                        // Show connected elements
                        PS3.CCAPI.ClearTargetInfo();

                        this.CELL_Temperature.Text = PS3.CCAPI.GetTemperatureCELL();
                        this.RSX_Temperature.Text = PS3.CCAPI.GetTemperatureRSX();
                        this.Firmware.Text = PS3.CCAPI.GetFirmwareVersion();
                        this.Console_type.Text = PS3.CCAPI.GetFirmwareType();
                        this.label23.Text = PS3.CCAPI.GetDllVersion().ToString();

                        groupBox12.Visible = true;
                        groupBox13.Visible = true;
                        button15.Visible = true;
                        comboBox6.Visible = true;
                        label14.Visible = true;
                        label24.Visible = true;
                        label23.Visible = true;
                        label21.Visible = true;
                        richTextBox1.Visible = true;
                        label20.Visible = true;
                        RSX_Temperature.Visible = true;
                        CELL_Temperature.Visible = true;
                        Console_type.Visible = true;
                        label7.Visible = true;
                        label15.Visible = true;
                        label16.Visible = true;
                        button10.Visible = true;
                        button11.Visible = true;
                        button12.Visible = true;
                        comboBox5.Visible = true;
                        comboBox3.Visible = true;
                        button8.Visible = true;
                        label13.Visible = true;
                        label12.Visible = true;
                        label11.Visible = true;
                        Firmware.Visible = true;

                        // Hide connection-related elements
                        label19.Visible = false;
                        label17.Visible = false;
                        ConnectionBox.Visible = false;
                        CCAPI.Visible = false;
                        IPtextBox.Visible = false;
                        IP_Reader.Visible = false;
                        ConnectBtn.Visible = false;

                        this.StatusLabel.Text = "Connected";
                        this.StatusLabel.ForeColor = Color.Green;
                    }
                    catch
                    {
                        this.StatusLabel.Text = "Not connected";
                        this.StatusLabel.ForeColor = Color.Red;
                    }
                }
                else if (PS3.ConnectTarget(this.IPtextBox.Text))
                {
                    PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.PROGRESS, "Connected Seccessfully");
                    PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);
                    this.ConnectionStatusSuccess();
                    this.API = this.IPtextBox.Text;

                    // Show connected elements
                    PS3.CCAPI.ClearTargetInfo();

                    this.CELL_Temperature.Text = PS3.CCAPI.GetTemperatureCELL();
                    this.RSX_Temperature.Text = PS3.CCAPI.GetTemperatureRSX();
                    this.Firmware.Text = PS3.CCAPI.GetFirmwareVersion();
                    this.Console_type.Text = PS3.CCAPI.GetFirmwareType();
                    this.label23.Text = PS3.CCAPI.GetDllVersion().ToString();

                    groupBox12.Visible = true;
                    groupBox13.Visible = true;
                    button15.Visible = true;
                    comboBox6.Visible = true;
                    label14.Visible = true;
                    label24.Visible = true;
                    label23.Visible = true;
                    label21.Visible = true;
                    richTextBox1.Visible = true;
                    label20.Visible = true;
                    RSX_Temperature.Visible = true;
                    CELL_Temperature.Visible = true;
                    Console_type.Visible = true;
                    label7.Visible = true;
                    label15.Visible = true;
                    label16.Visible = true;
                    button10.Visible = true;
                    button11.Visible = true;
                    button12.Visible = true;
                    comboBox5.Visible = true;
                    comboBox3.Visible = true;
                    button8.Visible = true;
                    label13.Visible = true;
                    label12.Visible = true;
                    label11.Visible = true;
                    Firmware.Visible = true;

                    // Hide connection-related elements
                    label19.Visible = false;
                    label17.Visible = false;
                    ConnectionBox.Visible = false;
                    CCAPI.Visible = false;
                    IPtextBox.Visible = false;
                    IP_Reader.Visible = false;
                    ConnectBtn.Visible = false;
                }
                else
                    this.ConnectionStatusFailed();
            }
            else
            {
                PS3.ChangeAPI(SelectAPI.PS3Manager);
                PS3M_API.ConnectTarget(this.IPtextBox.Text, Convert.ToInt32(7887));
                try
                {
                    if (!PS3M_API.IsConnected)
                        return;
                    this.AttachMethod.Items.Clear();
                    foreach (uint pidProcess in PS3M_API.Process.GetPidProcesses())
                    {
                        if (pidProcess != 0U)
                        {
                            this.AttachMethod.Items.Add((object)PS3M_API.Process.GetName(pidProcess));
                            PS3M_API.PS3.Notify("Connected Seccessfully");
                            PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                            this.ConnectionStatusSuccess();

                            // Notify messages
                            string[] firstMessage = {
                "+=========================+",
                "|            Welcome to XZ UTIL          |",
                "+=========================+"
            };
                            string combinedFirstMessage = string.Join("\n", firstMessage);
                            PS3M_API.PS3.Notify(combinedFirstMessage);

                            string[] secondMessage = {
                "+=========================+",
                "|              Created by: WMP            |",
                "+=========================+"
            };
                            string combinedSecondMessage = string.Join("\n", secondMessage);
                            PS3M_API.PS3.Notify(combinedSecondMessage);

                            string[] thirdMessage = {
                "+=========================+",
                "|               WWW.XREZ.IO               |",
                "+=========================+"
            };
                            string combinedThirdMessage = string.Join("\n", thirdMessage);
                            PS3M_API.PS3.Notify(combinedThirdMessage);

                            // Show connected elements
                            PS3.CCAPI.ClearTargetInfo();

                            this.CELL_Temperature.Text = PS3.CCAPI.GetTemperatureCELL();
                            this.RSX_Temperature.Text = PS3.CCAPI.GetTemperatureRSX();
                            this.Firmware.Text = PS3.CCAPI.GetFirmwareVersion();
                            this.Console_type.Text = PS3.CCAPI.GetFirmwareType();
                            this.label23.Text = PS3.CCAPI.GetDllVersion().ToString();

                            groupBox12.Visible = true;
                            groupBox13.Visible = true;
                            buttonsd.Visible = true;
                            button15.Visible = true;
                            comboBox6.Visible = true;
                            label14.Visible = true;
                            label24.Visible = true;
                            label23.Visible = true;
                            label21.Visible = true;
                            richTextBox1.Visible = true;
                            label20.Visible = true;
                            RSX_Temperature.Visible = true;
                            CELL_Temperature.Visible = true;
                            Console_type.Visible = true;
                            label7.Visible = true;
                            label15.Visible = true;
                            label16.Visible = true;
                            button10.Visible = true;
                            button11.Visible = true;
                            button12.Visible = true;
                            comboBox5.Visible = true;
                            comboBox3.Visible = true;
                            button8.Visible = true;
                            label13.Visible = true;
                            label12.Visible = true;
                            label11.Visible = true;
                            Firmware.Visible = true;

                            // Hide connection-related elements
                            label19.Visible = false;
                            label17.Visible = false;
                            ConnectionBox.Visible = false;
                            CCAPI.Visible = false;
                            IPtextBox.Visible = false;
                            IP_Reader.Visible = false;
                            ConnectBtn.Visible = false;
                        }
                        else
                            break;
                    }
                    this.AttachMethod.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    int num = (int)MessageBox.Show((IWin32Window)this, ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            this.ShowPS3Info();
        }

        private void ShowPS3Info()
        {
            this.API = PS3.GetCurrentAPIName();
        }

        private void AttachBtn_Click(object sender, EventArgs e)
        {
            SelectAPI currentAPI = PS3.GetCurrentAPI();

            try
            {
                switch (currentAPI)
                {
                    case SelectAPI.ControlConsole:
                    case SelectAPI.TargetManager:
                    case SelectAPI.PS3Manager:
                        AttachProcess(currentAPI);
                        break;

                    default:
                        this.StatusLabel.Text = "Invalid API selected";
                        this.StatusLabel.ForeColor = Color.Red;
                        break;
                }
            }
            catch
            {
                this.StatusLabel.Text = "Error attaching process";
                this.StatusLabel.ForeColor = Color.Red;
            }
        }

        private void AttachProcess(SelectAPI currentAPI)
        {
            switch (currentAPI)
            {
                case SelectAPI.ControlConsole:
                    PS3.AttachProcess();
                    break;

                case SelectAPI.TargetManager:
                    TMAPI tmapiInstance = new TMAPI();
                    bool attachResult = tmapiInstance.AttachProcess();
                    if (attachResult)
                    {
                        // Process has been attached successfully
                        this.StatusLabel.Text = "Connected + Attached";
                        this.StatusLabel.ForeColor = Color.Green;
                    }
                    else
                    {
                        // Process attachment failed
                        this.StatusLabel.Text = "Not attached";
                        this.StatusLabel.ForeColor = Color.Red;
                    }
                    break;

                case SelectAPI.PS3Manager:
                    try
                    {
                        PS3M_API.AttachProcess(PS3M_API.Process.Processes_Pid[this.AttachMethod.SelectedIndex]);
                        PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Double);
                        if (MainForm.PS3M_API.IsAttached)
                        {
                            this.StatusLabel.Text = "Connected + Attached";
                            this.StatusLabel.ForeColor = Color.Green;
                        }
                    }
                    catch
                    {
                        this.StatusLabel.Text = "Not attached";
                        this.StatusLabel.ForeColor = Color.Red;
                    }
                    break;
            }
        }

        #endregion

        #region Lables+Other
        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".dat", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Effect = DragDropEffects.Copy;

                        // Set a flag indicating that the file is dragged and dropped
                        isOpenedByButton = false;

                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (!isConversionInProgress)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".dat", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertDatToPng(file);
                        return;
                    }
                }
            }
        }

        private void IP_Reader_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void writeIP_Btn_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            isHovered = true;
            label1.Invalidate();

            // Draw the circular outline around the panel
            DrawCircularOutline(panel2, Color.FromArgb(153, 204, 255), 2);
            panel2.Visible = true;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            isHovered = false;
            label1.Invalidate();
            panel2.Visible = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastCursorPos = Cursor.Position;
            lastFormPos = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = Cursor.Position.X - lastCursorPos.X;
                int deltaY = Cursor.Position.Y - lastCursorPos.Y;

                this.Location = new Point(lastFormPos.X + deltaX, lastFormPos.Y + deltaY);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void UpdateControlsVisibility()
        {
            // Always show these controls
            buttoncc.Visible = true;
            label19.Visible = true;
            label17.Visible = true;
            ConnectionBox.Visible = true;
            CCAPI.Visible = true;
            IPtextBox.Visible = true;
            IP_Reader.Visible = true;
            ConnectBtn.Visible = true;

            if (isConnected)
            {
                // Show connected elements
                groupBox12.Visible = false;
                groupBox13.Visible = false;
                label14.Visible = true;
                button15.Visible = true;
                comboBox6.Visible = true;
                label24.Visible = true;
                label23.Visible = true;
                label21.Visible = true;
                richTextBox1.Visible = true;
                label20.Visible = true;
                RSX_Temperature.Visible = true;
                CELL_Temperature.Visible = true;
                Console_type.Visible = true;
                label7.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                button10.Visible = true;
                button8.Visible = true;
                button11.Visible = true;
                button12.Visible = true;
                comboBox5.Visible = true;
                comboBox3.Visible = true;
                label13.Visible = true;
                label12.Visible = true;
                label11.Visible = true;
                Firmware.Visible = true;
            }
        }

        private void UpdateLabel(int value)
        {
            // Update the label
            labelProgress.Text = $"Uploading: {value}%";

            // Check if the upload is complete (adjust the condition as needed)
            if (value == 100)
            {
                // If upload is complete, hide the label
                labelProgress.Visible = false;
            }
        }

        private void HandleCCAPILed(string selectedOption)
        {
            switch (selectedOption)
            {
                case "RedOff":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Red, PS3Lib.CCAPI.LedMode.Off);
                    break;
                case "RedOn":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Red, PS3Lib.CCAPI.LedMode.On);
                    break;
                case "RedBlink":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Red, PS3Lib.CCAPI.LedMode.Blink);
                    break;
                case "GreenOff":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Green, PS3Lib.CCAPI.LedMode.Off);
                    break;
                case "GreenOn":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Green, PS3Lib.CCAPI.LedMode.On);
                    break;
                case "GreenBlink":
                    PS3.CCAPI.SetConsoleLed(PS3Lib.CCAPI.LedColor.Green, PS3Lib.CCAPI.LedMode.Blink);
                    break;
                default:
                    // Handle unsupported option or provide a default behavior
                    break;
            }
        }

        private void HandlePS3MAPILed(string selectedOption)
        {
            switch (selectedOption)
            {
                case "RedOff":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                    break;
                case "RedOn":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    break;
                case "RedBlinkSlow":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Red, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkSlow);
                    break;
                case "GreenOff":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.Off);
                    break;
                case "GreenOn":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.On);
                    break;
                case "GreenBlinkSlow":
                    MainForm.PS3M_API.PS3.Led(PS3ManagerAPI.PS3MAPI.PS3_CMD.LedColor.Green, PS3ManagerAPI.PS3MAPI.PS3_CMD.LedMode.BlinkSlow);
                    break;
                default:
                    // Handle unsupported option or provide a default behavior
                    break;
            }
        }

        private void NotifyWithRichTextBox(string title, string message, PS3Lib.CCAPI.NotifyIcon icon)
        {
            // Set ReadOnly property to false to allow typing any character or symbol
            richTextBox1.ReadOnly = false;

            Font boldFont = new Font(richTextBox1.Font, FontStyle.Bold);
            richTextBox1.SelectionFont = boldFont;

            string formattedTime = DateTime.Now.ToString("hh:mmtt - yyyy");

            string composedMessage = $"{title} [{formattedTime}]\n{message}\n";
            richTextBox1.AppendText(composedMessage);

            // Optionally, remove the line below if you want to keep the text in richTextBox1
            richTextBox1.Clear();

            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                // Send the notification using CCAPI function
                SendCCAPINotification(composedMessage);
            }
            else if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
            {
                SendCCAPINotification(composedMessage);
            }
            else if (PS3.GetCurrentAPI() == SelectAPI.PS3Manager)
            {
                // Send the notification using PS3MAPI function
                SendPS3MAPINotification2(composedMessage);
            }
        }

        private void ExecuteCustomNotify()
        {

            string[] asciiArtOptions =
            {
        @"
      _______
     /          \
     |  () ()   |
     \  ~~~  /
      \_____/
        ",

        @"
          ______
         /         \
        |   \  /   |
        \   \/    /
         \____/
        ",

        @"
                            _       _   _ 
__  __ ____    _   _   | |_    (_) | |
\ \/ /  |_  /   | | | |   | |__|  | | | |
 >  <   / /    | |_| |   | |_    | | | |
/_/\_\ /___|   \__,_|   \__|  |_| |_|
        ",
        @"
 _   _____  _____   _____ 
/ | |___  /  |___ /  |___  |
| |    |_  \    |_  \      / / 
| |  ___) |  ___) |     / /  
|_| |____/  |____/   /_/   
",
        @"
 ____         _      _    _  _  __
|  _   \      / \     | \ |  | | |/ /
| |  |  |    / _ \    |  \|  | | ' / 
| |_ | |   / ___ \   | |\  |  | . \ 
|____/  /_/    \_\ |_| \_| |_ |\_\
",
        @"
 _   _       _       __  __
| | | |      / \      \ \ / /
| |_| |    / _ \      \   / 
|  _  |   / ___ \    /   \ 
|_| |_| /_/    \_\ /_/ \_\
",
@"
  ___     (
  |   |   ( )
  |   |__))
  |    __|
  |___|
",
@"
   /\_/\  
 ( o.o ) 
 > ^ <
",
@"
   (\__/)
  (='.'=)
  (.)__(.)
",
@"
   / \__
  (    @\___
  /         O
 /   (_____/
/_____/   U
",
@"
  __
 /''|\_____/)
 \_/|_)     )
    \  __  /
    (_/ (_/
"
    };

            // Get the selected index from the combo box
            int selectedIndex = comboBox6.SelectedIndex;

            // Ensure a valid index is selected
            if (selectedIndex >= 0 && selectedIndex < asciiArtOptions.Length)
            {
                // Get the selected ASCII art
                string selectedAsciiArt = asciiArtOptions[selectedIndex];

                // Check if PS3 object is initialized
                if (PS3 != null)
                {
                    // Send the notification based on the current API
                    switch (PS3.GetCurrentAPI())
                    {
                        case SelectAPI.ControlConsole:
                            SendCCAPINotification(selectedAsciiArt);
                            break;
                        case SelectAPI.TargetManager:
                            SendCCAPINotification(selectedAsciiArt);
                            break;
                        case SelectAPI.PS3Manager:
                            SendPS3MAPINotification(selectedAsciiArt);
                            break;
                        default:
                            Console.WriteLine("Unsupported API: " + PS3.GetCurrentAPI());
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("PS3 object is not initialized.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ASCII art selection.");
            }
        }

        private void SendCCAPINotification(int selectedIndex, string message)
        {
            PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.DIALOG, message);
        }

        private void SendCCAPINotification(string message)
        {
            PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.DIALOG, message);
        }

        private void SendPS3MAPINotification(string msg)
        {
            MainForm.PS3M_API.PS3.Notify(msg);
        }

        private void SendPS3MAPINotification2(string msg)
        {
            PS3M_API.PS3.Notify(msg);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            string webLink = "https://xrez.io";
            DialogResult result = MessageBox.Show($"This link is directing you to ( https://www.xrez.io )\nDo you want to open it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Process.Start(webLink);
            }
        }

        private void notificationTimer_Tick(object sender, EventArgs e)
        {

        }

        private void Disconnect()
        {
            if (PS3.GetCurrentAPI() == SelectAPI.ControlConsole)
            {
                PS3.CCAPI.Notify(PS3Lib.CCAPI.NotifyIcon.INFO, "Target Disconnected Successfully");
                PS3.CCAPI.RingBuzzer(PS3Lib.CCAPI.BuzzerMode.Single);
                PS3.CCAPI.DisconnectTarget();
            }
            else if (PS3.GetCurrentAPI() == SelectAPI.TargetManager)
            {
                PS3.TMAPI.DisconnectTarget();
            }
            else
            {
                if (PS3.GetCurrentAPI() == SelectAPI.PS3Manager)
                    return;
                if (!PS3M_API.IsConnected)
                {
                    int num = (int)MessageBox.Show((IWin32Window)this, "Connect first!");
                }
                else
                {
                    PS3M_API.PS3.Notify("Target Disconnected Successfully");
                    PS3M_API.PS3.RingBuzzer(PS3ManagerAPI.PS3MAPI.PS3_CMD.BuzzerMode.Single);
                    PS3M_API.DisconnectTarget();
                }
            }
        }

        private void ConnectionAndAttachedStatusFailed()
        {
            isConnectedOrAttached = false;
        }

        private void label32_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label32_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label32_MouseEnter(object sender, EventArgs e)
        {
            label32.BackColor = Color.Transparent;
            isHovered = true;
            label32.Invalidate();

            // Draw the circular outline around the panel
            DrawCircularOutline(panel3, Color.FromArgb(153, 204, 255), 2);
            panel3.Visible = true;
        }

        private void label32_MouseLeave(object sender, EventArgs e)
        {
            label32.BackColor = Color.Transparent;
            isHovered = false;
            label32.Invalidate();
            panel3.Visible = false;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            string webLink = "https://xrez.io";
            DialogResult result = MessageBox.Show($"This link is directing you to ( https://www.xrez.io )\nDo you want to open it?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Process.Start(webLink);
            }
        }
        #endregion
    }
}