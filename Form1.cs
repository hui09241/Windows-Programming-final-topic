using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private PictureBox[] picture = new PictureBox[10];      //遊戲區PictureBox
        private PictureBox left_pic;                            //顯示答案之PictureBox
        private Label sec;                                      //秒數標籤
        private Label word;                                     //文字標籤
        private Label steps;                                    //步數標籤
        private int[] randnum = new int[10];                    //亂數陣列(將儲存1至9的不重複亂數)
        private static Bitmap OriginBMP = new Bitmap(Properties.Resources.FotoJet); //遊戲圖片
        private int width_of_pic =  OriginBMP.Height / 3;                           //定義單格圖片寬度
        private int length_of_pic = OriginBMP.Width / 3;                            //定義單格圖片長度
        System.Media.SoundPlayer sp = new System.Media.SoundPlayer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            randpicnum();           //將亂數陣列填入1~9亂數

        #region 定義初始屬性

            /*Form屬性*/
            AutoSize = false;
            Size = new Size(OriginBMP.Width * 2 + 200 + 20 + 17, OriginBMP.Height + 10 + 38);
            MaximizeBox = false;
            Text = "五月天ʕ•ᴥ•ʔ...";
            sp.SoundLocation = @"MAYDAY.WAV";
            sp.Play();

            /*定義秒數標籤之屬性*/
            sec = new Label();
            {
                sec.Location = new Point(OriginBMP.Width + 3 + 7, 3);         //位置
                sec.Size = new Size(200, width_of_pic);                       //大小
                sec.TextAlign = ContentAlignment.MiddleCenter;                //文字對齊方式
                sec.Font = new Font("微軟正黑體", 22, FontStyle.Bold);        //字型
                sec.ForeColor = Color.FromKnownColor(KnownColor.RoyalBlue);   //顏色
                sec.Text = "00 : 00 : 00";                                    //預設文字
                sec.BorderStyle = BorderStyle.Fixed3D;                        //邊框樣式
            }
            this.Controls.Add(sec);         //將秒數標籤加入控制項(以顯示在表單上)
            sec.Click += give_up;           //指定give_up sub處理秒數標籤之click事件

            /*定義文字標籤之屬性*/
            word = new Label();
            {
               word.Location = new Point(OriginBMP.Width + 3 + 7, width_of_pic + 3);
               word.Size = new Size(200, width_of_pic);
               word.TextAlign = ContentAlignment.MiddleCenter;
               word.Font = new Font("微軟正黑體", 22, FontStyle.Bold);
               word.ForeColor = Color.FromKnownColor(KnownColor.LightSeaGreen);
               word.Text = "移動次數";
            }
            this.Controls.Add(word);


            /*定義步數標籤之屬性*/
            steps = new Label();
            {
               steps.Location = new Point(OriginBMP.Width + 3 + 7, width_of_pic * 2 + 3);
               steps.Size = new Size(200, width_of_pic);
               steps.TextAlign = ContentAlignment.MiddleCenter;
               steps.Font = new Font("微軟正黑體", 26, FontStyle.Bold);
               steps.ForeColor = Color.FromKnownColor(KnownColor.CornflowerBlue);
               steps.Text = "00";
               steps.BorderStyle = BorderStyle.Fixed3D;
            }
            this.Controls.Add(steps);


            /*定義左邊答案PictureBox之屬性*/
            left_pic = new PictureBox();
            {
               left_pic.SizeMode = PictureBoxSizeMode.StretchImage;         //圖片填滿方式
               left_pic.Visible = true;
               left_pic.Padding = new System.Windows.Forms.Padding(0);      //內距
               left_pic.Margin = new System.Windows.Forms.Padding(0);       //外距
               left_pic.Location = new Point(3, 3);
               left_pic.Size = new Size(OriginBMP.Width, OriginBMP.Height);
               left_pic.Image = OriginBMP;                                  //圖片
            }
            this.Controls.Add(left_pic);

            /*定義右邊九格PictureBox遊戲格之屬性*/
            Image BMP;
            for (int i = 1; i <= 9; i += 1)
            {
                picture[i] = new PictureBox();
                Point point = new Point(length_of_pic * ((randnum[i] - 1) % 3), width_of_pic * (Convert.ToInt16(Math.Floor((randnum[i] - 1) / (double)3))));
                Size size = new Size(length_of_pic, width_of_pic);
                BMP = OriginBMP.Clone(new Rectangle(point, size), OriginBMP.PixelFormat);       //透過座標複製原始圖片之一部分至BMP(切成九格依序複製)

                {
                    picture[i].SizeMode = PictureBoxSizeMode.StretchImage;          //圖片填滿方式
                    picture[i].Visible = true;                                      //圖片可見
                    picture[i].Padding = new System.Windows.Forms.Padding(0);       //內距
                    picture[i].Margin = new System.Windows.Forms.Padding(0);        //外距
                    picture[i].Location = new Point((((i - 1) % 3) * length_of_pic) + (OriginBMP.Width + 200 + 17), ((i - 1) / 3) * width_of_pic + 3);  //位置
                    picture[i].Size = new Size(length_of_pic, width_of_pic);        //大小
                    picture[i].Image = BMP;                                         //將BMP儲存之圖片填入PictureBox
                    picture[i].Tag = randnum[i];   // 儲存圖片編號                  //儲存圖片之編號(1~9)
                    picture[i].Click += new EventHandler(main);                     //新增事件處理器
                }
                picture[i].Click += main;                                           //指定main sub處理PictureBox之Click事件
                this.Controls.Add(picture[i]);                                      //將PictureBox加入控制項
            }
            picture[9].Visible = false;                                             //PictureBox皆設置完成後，將圖片九設為步可見
            
            /*定義計時器屬性*/
            Timer1.Interval = 1000;                                                 //設定頻率
            Timer1.Enabled = true;                                                  //計時器開始運作
        #endregion
        }

        /*放棄函數*/
        private void give_up(object sender, EventArgs e)  //若放棄則停止遊戲並顯示答案
        {
            Timer1.Enabled = false;     //停止計時器
            left_pic.Visible = true;    //顯示答案於左側
            get_win();                  //將遊戲設為獲勝
        }

        /*遊戲主函數*/
        private int press = 0;      //步數
        private void main(object sender, EventArgs e)
        {
            int index = Array.IndexOf(picture, sender);     //取得點選PictureBox之陣列索引值
            int unvisible = get_unvisible(index);           //取得周圍空白格之索引值(若無空白格，則為點選格)

            if (unvisible != index)                                     //若周圍有空白格
            {
                unvisible = switch_pic(index, unvisible);               //將兩者交換並回傳空白索引值
                press = press + 1;                                      //步數+1
                steps.Text = String.Format(press.ToString(), "00");     //更新步數
            }
            if (press >= 99)                //超過99步，停止遊戲並顯示答案
            {
                Timer1.Enabled = false;
                left_pic.Visible = true;
                get_win();
            }
            if (If_win())                   //獲勝，顯示空白格並停止遊戲
            {
                picture[unvisible].Visible = true;
                Timer1.Enabled = false;
                left_pic.Visible = true;
            }
        }

        /*初始化亂數陣列*/
        Random a = new Random(Guid.NewGuid().GetHashCode());
        public bool randpicnum()        //亂數陣列儲存的是PictureBox應該放入的照片編號，"陣列[1]=8"代表第一格的位置要放入第八格之照片
        {
            //將陣列依序填入1到9
            for (int i = 1; i <= 9; i += 1)
                randnum[i] = i;
            
            //將陣列[1]至陣列[8]打亂
            int @switch, rndi;
            for (int i = 1; i <= 8; i += 1)
            {
                rndi = a.Next(1, 9);
                @switch = randnum[i];
                randnum[i] = randnum[rndi];
                randnum[rndi] = @switch;
            }

            //陣列[9]維持9
            randnum[9] = 9;
            return true;
        }

        public int get_unvisible(int index)
        {
            //index為點選之位置
            var m3 = 0;     //m3為點選格之上方格
            var p3 = 0;     //p3為點選格之下方格

            /*判斷上/下格是否超出範圍，
             * 若超出：設上/下格為點選格自己
             * 否則：設為上/下格之位置
             * 例如：index=2，則上方格m3=2，下方格p3=5*/
            if (index - 3 > 0)
                m3 = index - 3;
            else
                m3 = index;
            if (index + 3 < 10)
                p3 = index + 3;
            else
                p3 = index;

            /*最左欄，判斷上/下/右格*/
            if (index == 1 | index == 4 | index == 7)
            {
                if (picture[index + 1].Visible == false)    //若右格看不見(即為空白格)，回傳右格*/
                    return index + 1;
                if (picture[m3].Visible == false)           //若上欄看不見，回傳上欄
                    return m3;
                if (picture[p3].Visible == false)           //若下欄看不見，回傳上欄
                    return p3;
            }
            /*中欄，判斷上/下/左/右格*/
            else if (index == 2 | index == 5 | index == 8)
            {
                if (picture[index + 1].Visible == false)
                    return index + 1;
                if (picture[index - 1].Visible == false)
                    return index - 1;
                if (picture[m3].Visible == false)
                    return m3;
                if (picture[p3].Visible == false)
                    return p3;
            }
            /*最右欄，判斷上/下/左格*/
            else if (index == 3 | index == 6 | index == 9)
            {
                if (picture[index - 1].Visible == false)
                    return index - 1;
                if (picture[m3].Visible == false)
                    return m3;
                if (picture[p3].Visible == false)
                    return p3;
            }
            /*皆不符合，回傳點選格，代表未找到空白格*/
            return index;
        }

        public int switch_pic(int visible, int unvisible)
        {
            if (visible == unvisible)
                return 0;
            Image tmp = picture[visible].Image;
            picture[visible].Image = picture[unvisible].Image;
            picture[unvisible].Image = tmp;
            int tmp_int = (int)picture[visible].Tag;
            picture[visible].Tag = picture[unvisible].Tag;
            picture[unvisible].Tag = tmp_int;

            picture[unvisible].Visible = true; // 交換可見和不可見之牌
            picture[visible].Visible = false;
            return visible; // 回傳不可見之牌
        }

        public bool If_win()
        {
            /*若PictureBox之照片編號不符合其所在位置編號，回傳否*/
            for (int i = 1; i <= 9; i += 1)
            {
                if (i != (int)picture[i].Tag)
                    return false;
            }
            /*否則回傳獲勝*/
            return true;
        }

        public void get_win()//取得當獲勝時的狀態，也就是將格子設為獲勝時的樣子
        {
            /*將所有格子顯示正確答案)*/
            Image tmp_pic;
            int tmp_tag;
            for (var i = 1; i <= 9; i++)
            {
                for (var j = 1; j <= 9; j++)
                {
                    if (i == (int)picture[j].Tag)
                    {
                        tmp_pic = picture[i].Image;
                        picture[i].Image = picture[j].Image;
                        picture[j].Image = tmp_pic;
                        tmp_tag = (int)picture[i].Tag;
                        picture[i].Tag = picture[j].Tag;
                        picture[j].Tag = tmp_tag;
                    }
                }
                picture[i].Visible = true;
                picture[i].Enabled = false;
            }
        }


        private int pass_time = 0;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            pass_time = pass_time + 1;
            var ss = pass_time % 60;                                                //秒數
            var mm = Convert.ToInt16(Math.Floor((pass_time / (double)60) % 60));    //分數
            var hh = mm / (double)60;                                               //時數
            //更新秒數標籤
            sec.Text = String.Format("{0:00}", hh) + " : " + String.Format("{0:00}",mm) + " : " + String.Format("{0:00}", ss);
            //開始後10秒以內，偶數秒顯示，奇數秒隱藏，(閃爍)*/
            if (pass_time % 2 == 0 & ss <= 10)
                left_pic.Visible = true;
            else
                left_pic.Visible = false;
        }

    }

}
