using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
namespace Browser
{
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
}
