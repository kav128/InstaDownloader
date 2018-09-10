using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using InstaCore;
using System.IO;

namespace InstaDownloaderGUI
{
    struct DownloadingResult
    {
        public int DownloadedItems { get; set; }
        public int TotalItems { get; set; }
        public int Percentage { get { return (DownloadedItems * 100) / TotalItems; } }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            images = new List<Image>();
            destinationPathTextBox.Text = Environment.CurrentDirectory;
        }
        
        InstaPost post;
        List<Image> images;

        private void button1_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.Value = 0;
            images.Clear();
            imageList1.Images.Clear();
            listView1.Items.Clear();
            previewLoadButton.Enabled = false;
            toolStripStatusLabel1.Text = "Загрузка фотографий для предпросмотра";

            previewBackgroundDownloader.RunWorkerAsync();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            pictureBox1.Image = images[e.Item.ImageIndex];
            toolStripStatusLabel1.Text = string.Format("Выбрано {0} из {1}", listView1.SelectedItems.Count, listView1.Items.Count);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            post = new InstaPost(urlTextBox.Text);
            using (WebClient wc = new WebClient())
            {
                foreach (var item in post.SidecarData)
                {
                    Stream stream = wc.OpenRead(item.DisplayUrl);
                    Image img = Image.FromStream(stream);
                    imageList1.Images.Add(img);
                    images.Add(img);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.Value = 0;

            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                ListViewItem lvi = new ListViewItem
                {
                    ImageIndex = i//,
                    //Text = (i + 1).ToString()
                };
                listView1.Items.Add(lvi);
            }
            previewLoadButton.Enabled = true;
            toolStripStatusLabel1.Text = "";
            listView1.Items[0].Selected = true;
        }

        private void selectDestinationPathButton_Click(object sender, EventArgs e)
        {
            DialogResult res = folderBrowserDialog.ShowDialog();
            if (res != DialogResult.OK)
                return;

            destinationPathTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Text = string.Format("{0}x{1}", Width, Height);
        }

        int[] selected;

        private void downloadButton_Click(object sender, EventArgs e)
        {
            downloadButton.Enabled = false;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.Value = 0;
            selected = listView1.SelectedIndices.Cast<int>().ToArray();
            
            backgroundDownloader.RunWorkerAsync();
        }

        private void backgroundDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            DownloadingResult result = (DownloadingResult)e.UserState;
            toolStripStatusLabel1.Text = string.Format("Загружено {0} из {1}", result.DownloadedItems, result.TotalItems);
        }

        private void backgroundDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            string destination = destinationPathTextBox.Text;
            DownloadingResult result = new DownloadingResult
            {
                DownloadedItems = 0,
                TotalItems = selected.Length
            };
            backgroundDownloader.ReportProgress(result.Percentage, result);
            foreach (var index in selected)
            {
                InstaMedia curItem = post.SidecarData[index];
                string itemUrl = curItem is InstaPhoto ? (curItem as InstaPhoto).DisplayUrl : (curItem as InstaVideo).VideoUrl;
                FileDownloader.Download(itemUrl, destination);
                result.DownloadedItems++;
                backgroundDownloader.ReportProgress(result.Percentage, result);
            }
        }

        private void backgroundDownloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            downloadButton.Enabled = true;
        }
    }
}
