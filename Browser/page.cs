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
        public bool isPageCompleted = false;
        public page(int index) : base()
        {
            indexPage = index;
            this.Visible = true;
            this.ScriptErrorsSuppressed = true;
            this.Dock = DockStyle.Fill;
            this.DocumentCompleted += ThisDocumentCompleted;
        }
        private void ThisDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isPageCompleted = true;
        }
        public void OpenUrl(string url)
        {
            isPageCompleted = false;
            this.Navigate(url);
        }
    }
}
