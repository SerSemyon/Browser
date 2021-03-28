using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Browser
{
    class page : WebBrowser
    {
        public int indexPage;
        Stack<string> pastUrl;
        Stack<string> futureUrl;
        string realUrl;
        public page(int index) : base()
        {
            indexPage = index;
            this.Visible = true;
            this.ScriptErrorsSuppressed = true;
            this.Dock = DockStyle.Fill;
            this.DocumentCompleted += ThisDocumentCompleted;
            pastUrl = new Stack<string>(4);
            futureUrl = new Stack<string>(1);
        }
        private void ThisDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (DocumentTitle != "Не удается открыть эту страницу")
            {
                realUrl = Url.ToString();
            }
        }
        public void OpenUrl(string url)
        {
            if (realUrl != "")
            {
                pastUrl.Push(realUrl);
                realUrl = "";
                futureUrl.Clear();
            }
            this.Navigate(url);
        }

        public new void GoBack()
        {
            if (realUrl != "") //если страница загружена
            {
                futureUrl.Push(realUrl);
                realUrl = "";
                this.Navigate(pastUrl.Pop());
            }
            else
            {
                Navigate(pastUrl.Pop());
            }
        }
        public new void GoForward()
        {

        }
    }
}
