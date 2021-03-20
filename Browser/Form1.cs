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
        FileStream streamHtml;
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
        void SaveBookmarks()
        {
            StreamWriter writer = new StreamWriter("Bookmarks.txt");
            foreach (bookmark book in bookmarks)
            {
                writer.Write(book.name+ '\n'+book.url+'\n');
            }
            writer.Close();
        }

        private void HtmlDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            streamHtml.Close();
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;
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
        public browserForm()
        {
            InitializeComponent();
            AddTab();
            ReadBookmarks();
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
            web.Navigated += SaveInHistory;
            tabControl1.TabPages.Add("New Pages"); 
            tabControl1.SelectTab(i); 
            tabControl1.SelectedTab.Controls.Add(web); 
            i += 1; // вкладка с индексом один
            web.DocumentCompleted += HtmlDocumentCompleted;
            streamHtml = new FileStream(nameFile, FileMode.Open);
            web.DocumentStream = streamHtml;
        }
        void AddTab(string url)
        {
            WebBrowser web = new WebBrowser(); //экземпляр класса web браузер 
            web.Visible = true; // сделаем его видимым
            web.ScriptErrorsSuppressed = true; // отображает все ошибки
            web.Dock = DockStyle.Fill; // сделаем его на полный экран
            web.DocumentCompleted += DocumentCompleted;
            web.NewWindow += NewWindow;
            web.Navigated += SaveInHistory;
            tabControl1.TabPages.Add("New Pages"); // добавление влкадок
            tabControl1.SelectTab(i); // выделяет поределнную вкладку
            tabControl1.SelectedTab.Controls.Add(web); //то, что данной вкладой управляет
            i += 1; // вкладка с индексом один
            OpenUrl(url);
        }

        void SaveInHistory(object sender, WebBrowserNavigatedEventArgs e)
        {

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
        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isPageCompleted = true;
            refreshButton.BackgroundImage = Properties.Resources.refresh; 
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;//чтобы показывалось имя веб страницы
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
        }

        private void addBookmarkButton_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text != "" && tabControl1.SelectedTab.Text !="Закладки")
            bookmarks.Add(new bookmark(tabControl1.SelectedTab.Text, ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString()));
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
}