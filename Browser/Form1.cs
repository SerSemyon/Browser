using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
    public partial class browserForm : Form
    {
        int i = 0; //индекс вкладки
        bool isPageCompleted = false;
        public browserForm()
        {
            InitializeComponent();
            AddTab();
        }
        void AddTab()
        {
            AddTab("Yandex.ru");
        }
        void AddTab(string url)
        {
            WebBrowser web = new WebBrowser(); //экземпляр класса web браузер 
            web.Visible = true; // сделаем его видимым
            web.ScriptErrorsSuppressed = true; // отображает все ошибки
            web.Dock = DockStyle.Fill; // сделаем его на полный экран
            web.DocumentCompleted += DocumentCompleted;
            web.NewWindow += NewWindow;
            
            tabControl1.TabPages.Add("New Pages"); // добавление влкадок
            tabControl1.SelectTab(i); // выделяет поределнную вкладку
            tabControl1.SelectedTab.Controls.Add(web); //то, что данной вкладой управляет
            i += 1; // вкладка с индексом один
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
        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isPageCompleted = true;
            refreshButton.BackgroundImage = Properties.Resources.refresh; 
            tabControl1.SelectedTab.Text = ((WebBrowser)tabControl1.SelectedTab.Controls[0]).DocumentTitle;//чтобы показывалось имя веб страницы
        }
        private void refreshButton_Click(object sender, EventArgs e)
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
    }
}