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
        int indexTabBookmark = -1;
        int indexTabHistory = -1;
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
                if (tabControl1.SelectedIndex == indexTabHistory)
                    indexTabHistory = -1;
            }
        }
        void CreateHtmlHistory()
        {
            try
            {
                StreamWriter streamwriter = new StreamWriter("lastHistory.html");
                streamwriter.WriteLine("<html>");
                streamwriter.WriteLine("<head>");
                streamwriter.WriteLine("  <title>История</title>");
                streamwriter.WriteLine("  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                streamwriter.WriteLine("</head>");
                streamwriter.WriteLine("<body>");
                if (histories.Count == 0)
                {
                    streamwriter.WriteLine("<p><img src =\"https://cdn.iconscout.com/icon/premium/png-512-thumb/janitor-1631013-1380608.png\" alt=\"Clear\"></p>");
                    streamwriter.WriteLine("<p><a> Тут чисто </a></p>");
                }
                else
                    streamwriter.WriteLine("<p><img src =\"http://cdn.onlinewebfonts.com/svg/download_359214.png\" height =50 width = 50 align=\"middle\"></p>");
                foreach (history story in histories)
                {
                    streamwriter.WriteLine("<p><a>" + story.time.ToString() + "</a> <a href=" + story.url + ">" + story.name + " " + story.url + "</a></p>");
                }
                streamwriter.WriteLine("</body>");
                streamwriter.WriteLine("</html>");
                streamwriter.Close();
            }
            catch
            {

            }
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
            ((WebBrowser)sender).Navigate(url);
            e.Cancel = true;
        }
        void OpenUrl(string url)
        {
            isPageCompleted = false;
            refreshButton.BackgroundImage = Properties.Resources.stop;
            try
            {
                ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Navigate(url);
            }
            catch
            {
                AddTab(url);
            }
        }
        void SaveInHistory()
        {
            try
            {
                string thisUrl = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
                if (thisUrl != "about:blank")
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
            catch
            {

            }
        }
        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isPageCompleted = true;
            string namePage = ((WebBrowser)sender).DocumentTitle;
            if (namePage != "Не удается открыть эту страницу")
            {
                tabControl1.SelectedTab.Text = namePage;
            }
            else
            {
                OpenUrl("?" + ((WebBrowser)sender).Url.ToString().Split('/')[2]);
            }
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;//чтобы показывалось имя веб страницы
            SaveInHistory();
        }
        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshPage();
        }

        void RefreshPage()
        {
            try
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
                    CreateHtmlHistory();
                    OpenHtml("lastHistory.html");
                }
            }
            catch
            {
                RefreshBookmarksTab();
                tabControl1.SelectedTab.Text = "Закладки";
            }
        }
        private void backButton_Click(object sender, EventArgs e)
        {
            try
            {
                ((WebBrowser)tabControl1.SelectedTab.Controls[0]).GoBack();
            }
            catch
            {

            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            try
            {
                ((WebBrowser)tabControl1.SelectedTab.Controls[0]).GoForward();
            }
            catch
            {

            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (isPageCompleted)
                AddTab();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 1)
            {
                int number = tabControl1.SelectedIndex;
                if (number == indexTabBookmark)
                {
                    indexTabBookmark = -1;
                }
                if (number == indexTabHistory)
                {
                    indexTabHistory = -1;
                }
                int indexSelext = tabControl1.SelectedIndex;
                if (indexTabHistory > indexSelext)
                    indexTabHistory--;
                if (indexTabBookmark > indexSelext)
                    indexTabBookmark--;
                tabControl1.TabPages.RemoveAt(number);
                try
                {
                    tabControl1.SelectTab(number - 1);
                }
                catch
                {
                    tabControl1.SelectTab(0);
                }
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
            try
            {
                string newUrl = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
                if (tabControl1.SelectedTab.Text != "" && tabControl1.SelectedTab.Text != "Закладки" && tabControl1.SelectedTab.Text != "История")
                {
                    bool haveBookmark = false;
                    foreach (bookmark mark in bookmarks)
                    {
                        if (mark.url.Split('/')[2] == "google.com")
                        {
                            if (mark.url.Split('/')[2] == newUrl.Split('/')[2])
                            {
                                haveBookmark = true;
                                break;
                            }
                        }
                        else
                        {
                            if (mark.url == newUrl)
                            {
                                haveBookmark = true;
                                break;
                            }
                        }
                    }
                    if (!haveBookmark)
                    {
                        bookmarks.Add(new bookmark(tabControl1.SelectedTab.Text, newUrl));
                    }
                }
            }
            catch
            {

            }
        }
        private void OpenBookmark(object sender, EventArgs e)
        {
            AddTab(((urlButton)sender).url);
        }
        public void DeleteBookmark(object sender, EventArgs e)
        {
            foreach (bookmark mark in bookmarks)
            {
                if (mark.url == ((urlButton)sender).url)
                {
                    bookmarks.Remove(mark);
                    break;
                }
            }
            RefreshBookmarksTab();
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Url.ToString();
                if (richTextBox1.Text == "about:blank")
                    richTextBox1.Text = "История";
            }
            catch
            {
                richTextBox1.Text = "Закладки";
            }
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void bookmarksButton_Click(object sender, EventArgs e)
        {
            AddBookmarksTab();
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            if (indexTabHistory == -1)
            {
                CreateHtmlHistory();
                indexTabHistory = i;
                AddHtmlTab("lastHistory.html");
            }
            else
            {
                tabControl1.SelectedIndex = indexTabHistory;
                RefreshPage();
            }
        }

        private void clearHistoryButton_Click(object sender, EventArgs e)
        {
            DialogResult mes = MessageBox.Show("Очистить историю?", "История просмотров будет удалена", MessageBoxButtons.OKCancel);
            if (mes == DialogResult.OK)
            {
                histories.Clear(); 
                if (tabControl1.SelectedTab.Text == "История")
                {
                    CreateHtmlHistory();
                    OpenHtml("lastHistory.html");
                } 
            }
        }

        void AddBookmarksTab()
        {
            if (indexTabBookmark ==-1)
            {
                tabControl1.TabPages.Add("Закладки");
                tabControl1.SelectTab(i);
                CreateBookmarksTab();
                indexTabBookmark = i;
                i += 1;
            }
            else
            {
                tabControl1.SelectedIndex = indexTabBookmark;
                RefreshPage();
            }
        }
        void CreateBookmarksTab()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ColumnCount = 0;
            mainPanel.RowCount = 0;
            int column;
            int row;
            if (bookmarks.Count <= 4)
            {
                column = bookmarks.Count;
                row = 1;
            }
            else if (bookmarks.Count<=8)
            {
                column = 4;
                row = 2;
            }
            else if (bookmarks.Count <= 16)
            {
                column = 4;
                row = 4;
            }
            else
            {
                column = 8;
                row = bookmarks.Count / 8;
                if (bookmarks.Count % 8 != 0)
                    row++;
            }
            int number = 0;
            foreach (bookmark mark in bookmarks)
            {
                bookmarkButton newButton = new bookmarkButton(mark);
                newButton.openUrlButton.Click += OpenBookmark;
                newButton.deleteButton.Click += DeleteBookmark;
                if (mainPanel.ColumnCount < column)
                {
                    mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
                }

                else if (mainPanel.RowCount < row)
                    mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
                mainPanel.Controls.Add(newButton, number % column, number / column);
                number++;
            }
            foreach (ColumnStyle style in mainPanel.ColumnStyles)
            {
                style.Width = 100;
            }
            foreach (RowStyle style in mainPanel.RowStyles)
            {
                style.Height = 100;
            }
            mainPanel.Visible = true;
            if (bookmarks.Count > 0)
                mainPanel.BackColor = Color.DarkCyan;
            else 
                mainPanel.BackgroundImage = Properties.Resources.bookmarks;
            mainPanel.BackgroundImageLayout = ImageLayout.Zoom;
            tabControl1.SelectedTab.Controls.Add(mainPanel);
        }
        void RefreshBookmarksTab()
        {
            tabControl1.SelectedTab.Controls.RemoveAt(0);
            CreateBookmarksTab();

        }
        private void printButton_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((WebBrowser)tabControl1.SelectedTab.Controls[0]).Print();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка параметров печати.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString(((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentText, richTextBox1.Font, Brushes.Black, 0, 0); //Класс Graphics предоставляет методы рисования на устройстве отображения
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab.Text != "История")
                {
                    ((WebBrowser)tabControl1.SelectedTab.Controls[0]).ShowSaveAsDialog();
                }
            }
            catch
            {

            }
        }
    }
    class bookmarkButton : TableLayoutPanel
    {
        public urlButton openUrlButton;
        public urlButton deleteButton;
        bookmark mark;
        public bookmarkButton(bookmark mark) : base()
        {
            this.mark = mark;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.AliceBlue;
            openUrlButton = new urlButton(mark.url);
            openUrlButton.Visible = true;
            openUrlButton.Text = mark.name;
            openUrlButton.BackColor = Color.White;
            openUrlButton.Dock = DockStyle.Fill;
            openUrlButton.BackgroundImageLayout = ImageLayout.Zoom;
            openUrlButton.BackgroundImage = Properties.Resources.tableBookmark;
            deleteButton = new urlButton(mark.url);
            deleteButton.Visible = true;
            deleteButton.Text = "X";
            deleteButton.BackColor = Color.Red;
            deleteButton.Dock = DockStyle.Fill;
            this.Controls.Add(deleteButton);
            this.Controls.Add(openUrlButton);
        }
    }
    class urlButton : Button
    {
        public string url;
        public urlButton(string Url) : base()
        {
            url = Url;
        }

    }
    public class bookmark
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