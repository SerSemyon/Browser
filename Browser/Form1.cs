using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Browser
{
    public partial class browserForm : Form
    {
        int i = 0; //индекс вкладки
        bool isPageCompleted = false;
        List<bookmark> bookmarks;
        List<history> histories;
        FileStream streamHtml;
        string lastOpenUrl;
        void ReadBookmarks()
        {
            StreamReader reader = new StreamReader("Bookmarks.txt");
            string fileString = reader.ReadToEnd();
            string[] fileData = fileString.Split('\n');
            bookmarks = new List<bookmark>(fileData.Length/2);
            for (int i = 0; i < fileData.Length - 2; i += 2)
            {
                bookmarks.Add(new bookmark(fileData[i], fileData[i + 1]));
            }
            reader.Close();
        }
        void ReadHistory()
        {
            StreamReader reader = new StreamReader("History.txt");
            string fileString = reader.ReadToEnd();
            string[] fileData = fileString.Split('\n');
            histories = new List<history>(fileData.Length / 3);
            for (int i = 0; i < fileData.Length - 3; i += 3)
            {
                DateTime timeBrowse = Convert.ToDateTime(fileData[i]);
                if (timeBrowse.AddDays(7)>=DateTime.Now)
                histories.Add(new history(timeBrowse, fileData[i+1], fileData[i + 2]));
            }
            reader.Close();
        }
        void SaveBookmarks()
        {
            StreamWriter writer = new StreamWriter("Bookmarks.txt");
            foreach (bookmark book in bookmarks)
            {
                writer.Write(book.name+ '\n'+book.url+'\n');
            }
            writer.Close();
        }
        void SaveHistory()
        {
            StreamWriter writer = new StreamWriter("History.txt");
            foreach (history book in histories)
            {
                writer.Write(book.time.ToString() + '\n' + book.name + '\n'+ book.url + '\n');
            }
            writer.Close();
        }
        private void HtmlDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            streamHtml.Close();
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;
            if (((WebBrowser)sender).Url.ToString()!= "about:blank")
            {
                ((WebBrowser)sender).DocumentCompleted -= HtmlDocumentCompleted;
                ((WebBrowser)sender).DocumentCompleted += DocumentCompleted;
                SaveInHistory();
            }
        }
        void CreateHtmlBookmarks()
        {
            StreamWriter streamwriter = new StreamWriter("lastBookmarks.html");
            streamwriter.WriteLine("<html>");
            streamwriter.WriteLine("<head>");
            streamwriter.WriteLine("  <title>Закладки</title>");
            streamwriter.WriteLine("  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            streamwriter.WriteLine("</head>");
            streamwriter.WriteLine("<body>");
            foreach (bookmark mark in bookmarks)
            {
                streamwriter.WriteLine("<p><a href="+ mark.url+">"+mark.name+"</a></p>");
            }
            streamwriter.WriteLine("</body>");
            streamwriter.WriteLine("</html>");
            streamwriter.Close();
        }
        void CreateHtmlHistory()
        {
            StreamWriter streamwriter = new StreamWriter("lastHistory.html");
            streamwriter.WriteLine("<html>");
            streamwriter.WriteLine("<head>");
            streamwriter.WriteLine("  <title>История</title>");
            streamwriter.WriteLine("  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            streamwriter.WriteLine("</head>");
            streamwriter.WriteLine("<body>");
            foreach (history story in histories)
            {
                streamwriter.WriteLine("<p><a>" + story.time.ToString()+"</a> <a href=" + story.url + ">" + story.name + " "+ story.url+"</a></p>");
            }
            streamwriter.WriteLine("</body>");
            streamwriter.WriteLine("</html>");
            streamwriter.Close();
        }
        public browserForm()
        {
            InitializeComponent();
            AddTab();
            ReadBookmarks();
            ReadHistory();
        }
        void AddTab()
        {
            AddTab("Yandex.ru");
        }

        void OpenHtml(string nameFile)
        {
            streamHtml = new FileStream(nameFile, FileMode.Open);
            ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentStream = streamHtml;
        }
        void AddHtmlTab(string nameFile)
        {
            WebBrowser web = new WebBrowser();
            web.Visible = true; 
            web.ScriptErrorsSuppressed = true; 
            web.Dock = DockStyle.Fill; 
            web.NewWindow += NewWindow;
            tabControl1.TabPages.Add("New Pages"); 
            tabControl1.SelectTab(i); 
            tabControl1.SelectedTab.Controls.Add(web); 
            i += 1;
            web.DocumentCompleted += HtmlDocumentCompleted;
            streamHtml = new FileStream(nameFile, FileMode.Open);
            web.DocumentStream = streamHtml;
        }
        void AddTab(string url)
        {
            WebBrowser web = new WebBrowser();
            web.Visible = true; 
            web.ScriptErrorsSuppressed = true; 
            web.Dock = DockStyle.Fill;
            web.DocumentCompleted += DocumentCompleted;
            web.NewWindow += NewWindow;
            tabControl1.TabPages.Add("New Pages"); 
            tabControl1.SelectTab(i); 
            tabControl1.SelectedTab.Controls.Add(web);
            i += 1;
            OpenUrl(url);
        }
        private void NewWindow(object sender, CancelEventArgs e)
        {
            HtmlElement link = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Document.ActiveElement;
            string url = link.GetAttribute("href");
            ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Navigate(url);
            e.Cancel = true;
        }
        void OpenUrl(string url)
        {
            isPageCompleted = false;
            refreshButton.BackgroundImage = Properties.Resources.stop;
            ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Navigate(url);

        }
        void SaveInHistory()
        {
            if (((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString() != "about:blank")
            {
                history nowOpen = new history(DateTime.Now, tabControl1.SelectedTab.Text, ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString());
                try
                {
                    if (nowOpen.url != lastOpenUrl)
                    {
                        histories.Add(nowOpen);
                    }
                }
                catch
                {
                    histories.Add(nowOpen);
                }
                lastOpenUrl = nowOpen.url;
            }
        }
        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isPageCompleted = true;
            refreshButton.BackgroundImage = Properties.Resources.refresh; 
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;
            SaveInHistory();
        }
        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString() != "about:blank")
            {
                if (isPageCompleted)
                {
                    string newUrl = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
                    OpenUrl(newUrl);
                }
                else
                {
                    ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Stop();
                    isPageCompleted = true;
                    refreshButton.BackgroundImage = Properties.Resources.refresh;
                }
            }
            else
            {
                if (tabControl1.SelectedTab.Text == "Закладки")
                {
                    CreateHtmlBookmarks();
                    OpenHtml("lastBookmarks.html");
                }
                else if (tabControl1.SelectedTab.Text == "История")
                {
                    CreateHtmlHistory();
                    OpenHtml("lastHistory.html");
                }
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            ((WebBrowser)tabControl1.SelectedTab.Controls[0]).GoBack();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            ((WebBrowser)tabControl1.SelectedTab.Controls[0]).GoForward();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddTab();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 1)
            {
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
                i -= 1;
            }
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenUrl(richTextBox1.Text);
            }
        }

        private void browserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBookmarks();
            SaveHistory();
        }

        private void addBookmarkButton_Click(object sender, EventArgs e)
        {
            string newUrl = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
            if (tabControl1.SelectedTab.Text != "" && tabControl1.SelectedTab.Text !="Закладки" && tabControl1.SelectedTab.Text != "История")
            {
                bool haveBookmark = false;
                foreach (bookmark mark in bookmarks)
                {
                    if (mark.url.Split('/')[2] == newUrl.Split('/')[2])
                    {
                        haveBookmark = true;
                        break;
                    }
                }
                if (!haveBookmark)
                {
                    bookmarks.Add(new bookmark(tabControl1.SelectedTab.Text, newUrl));
                }
            }
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            richTextBox1.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void bookmarksButton_Click(object sender, EventArgs e)
        {
            CreateHtmlBookmarks();
            AddHtmlTab("lastBookmarks.html");
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            CreateHtmlHistory();
            AddHtmlTab("lastHistory.html");
        }
    }

    class bookmark
    {
        public string name;
        public string url;
        public bookmark(string newName, string newUrl)
        {
            name = newName;
            url = newUrl;
        }
    }
    class history
    {
        public DateTime time;
        public string name;
        public string url;
        public history(DateTime newTime, string newName, string newUrl)
        {
            time = newTime;
            name = newName;
            url = newUrl;
        }
    }
}