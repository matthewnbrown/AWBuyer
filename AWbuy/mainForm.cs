namespace AWbuy
{
    using Properties;
    using AWbuy.Res;
    using System;
    using System.ComponentModel;
    using System.Deployment.Application;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public class mainForm : Form
    {

        private string eventManagementList;

        private bool minimizeNotification;
        private CheckBox autoUpdate_chk;
        private ToolStripStatusLabel balance_txt;
        private CheckBox bankBuy_chk;
        private bool banking;
        private TextBox bankPct_txt;
        private StatusStrip bottomStrip;
        private ToolStripStatusLabel bottomStrip_txt;
        private Thread bThread;
        private Buyer buyer;
        private TabPage buyer_tab;
        private TextBox buyPct_txt;
        private Label captchaAttempts_txt;
        private Label captchaRate_txt;
        private Label captchaSuccess_txt;
        private int checkUpdates = 15;
        private CheckBox Click_chk;
        private Label Clicked_txt;
        private Clicker clicker;
        private TabPage clicker_tab;
        private GroupBox clicksettings_box;
        private GroupBox clickstats_box;
        private IContainer components;
        private Thread cThread;
        private TextBox email_txt;
        private TextBox endMins;
        private ToolStripStatusLabel gapfiller;
        private TabPage general_tab;
        private TextBox GoalBox;
        private Label goldBank_lbl;
        private GroupBox groupBox1;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label saveExit_lbl;
        private Label label14;
        private Label autoUpdate_lbl;
        private Label label17;
        private Label label18;
        private Label label19;
        private Label label2;
        private Label label20;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private NotifyIcon notifyIcon;
        private TextBox password_txt;
        private bool promptedUpdate;
        private TabPage moreSettings_tab;
        private CheckBox saveOnExit_chk;
        private CheckBox sendMorale_chk;
        private TextBox sendUserID_txt;
        private TabPage settings_tab;
        private TextBox startMins;
        private Button startStop_btn;
        private TabControl tabs;
        private Label time_lbl;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Label totalClicks_txt;
        private System.Windows.Forms.Timer updateCheckTimer = new System.Windows.Forms.Timer();
        private bool updating;
        private Label wepPurch_lbl;
        private Label darkTheme_lbl;
        private GroupBox buyerSettings_gb;
        private CheckBox buySoldiers_chk;
        private Label label13;
        private Label label15;
        private Label label16;
        private Label label21;
        private Label label23;
        private Label buySoldiers_lbl;
        private GroupBox buyStats_gb;
        private Label strikeAction_lbl;
        private Label defensiveAction_lbl;
        private Label sentryRating_lbl;
        private Label label25;
        private Label spyRating_lbl;
        private Label label27;
        private Label label28;
        private Label label29;
        private CheckBox buyAllSoldiers_chk;
        private GroupBox eventSettings_gb;
        private CheckBox eventStudyHall_chk;
        private CheckBox eventTaxFree_chk;
        private CheckBox eventWhatSent_chk;
        private CheckBox eventGoldRush_chk;
        private Label label24;
        private Label label30;
        private Label label31;
        private Label label32;
        private CheckBox darkTheme_Chk;

        public mainForm()
        {
            if (!this.checkForPhantom())
            {
                MessageBox.Show("You are missing phantomjs.exe, please download phantomjs.exe and place it in the folder.", "Missing PhantomJS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

            else
            {
                eventManagementList = "";
                this.updating = false;
                this.buyer = new Buyer();
                buyer.buySoldiers = false;
                this.clicker = new Clicker();
                this.banking = true;
                this.minimizeNotification = false;
                this.promptedUpdate = false;
                this.InitializeComponent();
                this.disableConstruction();
                this.loadInfo("awbuyupdate");
            }
        }

        /// <summary>
        /// Checks for phantonjs.exe in localappdata. If it exists, deletes copy of phantomjs in local folder
        /// Otherwise, it will move phantomjs.exe from local folder to applicationdata
        /// </summary>
        /// <returns>Returns true if phantomjs is found. False if not.</returns>
        private bool checkForPhantom()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/phantomjs.exe";
            if (File.Exists(path))
            {
                try
                {
                    if (File.Exists("phantomjs.exe"))
                    {
                        File.Delete("phantomjs.exe");
                    }
                }
                catch (Exception)
                {
                }
                return true;
            }
            if (File.Exists("phantomjs.exe"))
            {
                File.Move("phantomjs.exe", path);
                return true;
            }
            return false;
        }

        private bool CheckForUpdate()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CheckForUpdate();
            }
            catch (Exception)
            {
            }
            return false;
        }

        private bool checkLogin(string email, string password, string defaultEmail = "", string defaultPass = "", int minEmailLength = 6, int minPassLength = 0)
        {
            if ((email.Equals(defaultEmail) || password.Equals(defaultPass)) || ((email.Length < minEmailLength) || (password.Length < minPassLength)))
            {
                return false;
            }
            return ((email.Contains<char>('@') && email.Substring(email.IndexOf('@')).Contains<char>('.')) && (email.Substring(email.IndexOf('@')).Length != 2));
        }

        private bool deleteInfo(string fileName)
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + fileName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Disables all parts of the form that are under construction
        /// </summary>
        private void disableConstruction()
        {
            //this.moreSettings_tab.Enabled = false;
            eventStudyHall_chk.Enabled = false;
        }

        /// <summary>
        /// Disables all fields that should not be modified while the program is running
        /// </summary>
        private void disableFields()
        {
            this.email_txt.Enabled = false;
            this.password_txt.Enabled = false;
            this.startMins.Enabled = false;
            this.endMins.Enabled = false;
            this.GoalBox.Enabled = false;
            this.Click_chk.Enabled = false;
            this.sendMorale_chk.Enabled = false;
            this.sendUserID_txt.Enabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Loads all the info to fill out a form from a saved file.
        /// </summary>
        /// <param name="fileName">Name of the saved file</param>
        /// <returns></returns>
        private bool loadInfo(string fileName)
        {
            try
            {
                int num = 22;
                StreamReader reader = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + fileName);
                string[] strArray = new string[num];
                for (int i = 0; i < num; i++)
                {
                    strArray[i] = reader.ReadLine();
                }
                reader.Close();
                reader.Dispose();
                this.email_txt.Text = strArray[0];
                this.password_txt.Text = strArray[1];
                this.startMins.Text = strArray[3];
                this.endMins.Text = strArray[4];
                this.buyer.purchased = Convert.ToInt64(strArray[5]);
                this.buyer.banked = Convert.ToInt64(strArray[6]);
                this.clicker.totalClicked = Convert.ToInt32(strArray[7]);
                this.GoalBox.Text = strArray[8];
                this.bankBuy_chk.Checked = Convert.ToBoolean(strArray[9]);
                this.Click_chk.Checked = Convert.ToBoolean(strArray[10]);
                this.buyPct_txt.Text = strArray[11];
                this.bankPct_txt.Text = strArray[12];
                this.sendMorale_chk.Checked = Convert.ToBoolean(strArray[13]);
                this.sendUserID_txt.Text = strArray[14];
                this.autoUpdate_chk.Checked = Convert.ToBoolean(strArray[15]);
                this.saveOnExit_chk.Checked = Convert.ToBoolean(strArray[0x10]);
                buySoldiers_chk.Checked = Convert.ToBoolean(strArray[17]);
                buyAllSoldiers_chk.Checked = Convert.ToBoolean(strArray[18]);
                eventGoldRush_chk.Checked = Convert.ToBoolean(strArray[19]);
                eventWhatSent_chk.Checked = Convert.ToBoolean(strArray[20]);
                eventTaxFree_chk.Checked = Convert.ToBoolean(strArray[21]);
                if (Convert.ToBoolean(strArray[2]))
                {
                    this.startStop_btn.PerformClick();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool saveInfo(string fileName)
        {
            int num = 22;
            if (fileName.Equals(""))
            {
                fileName = Convert.ToString(Environment.TickCount);
            }
            try
            {
                string[] contents = new string[num];
                contents[0] = this.email_txt.Text;
                contents[1] = this.password_txt.Text;
                if (this.updating)
                {
                    contents[2] = this.buyer.isRunning().ToString();
                }
                else
                {
                    contents[2] = Convert.ToString(false);
                }
                contents[3] = this.startMins.Text;
                contents[4] = this.endMins.Text;
                contents[5] = Convert.ToString(this.buyer.purchased);
                contents[6] = Convert.ToString(this.buyer.banked);
                contents[7] = Convert.ToString(this.clicker.getClicked());
                contents[8] = this.GoalBox.Text;
                contents[9] = this.bankBuy_chk.Checked.ToString();
                contents[10] = this.Click_chk.Checked.ToString();
                contents[11] = this.buyPct_txt.Text;
                contents[12] = this.bankPct_txt.Text;
                contents[13] = Convert.ToString(this.sendMorale_chk.Checked);
                contents[14] = this.sendUserID_txt.Text;
                contents[15] = Convert.ToString(this.autoUpdate_chk.Checked);
                contents[0x10] = Convert.ToString(this.saveOnExit_chk.Checked);
                contents[17] = Convert.ToString(buySoldiers_chk.Checked);
                contents[18] = Convert.ToString(buyAllSoldiers_chk.Checked);
                contents[19] = Convert.ToString(eventGoldRush_chk.Checked);
                contents[20] = Convert.ToString(eventWhatSent_chk.Checked);
                contents[21] = Convert.ToString(eventTaxFree_chk.Checked);
                File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + fileName, contents);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool invalidDigit(char testChar) => (!char.IsControl(testChar) && !char.IsDigit(testChar));

        private int minsTillTime()
        {
            int minute = DateTime.Now.Minute;
            return (this.checkUpdates - (minute % 15));
        }

        private int occuranceCount(char target, string mainString)
        {
            int num = 0;
            for (int i = 0; i < mainString.Length; i++)
            {
                if (mainString[i] == target)
                {
                    num++;
                }
            }
            return num;
        }

        private int occuranceCount(string target, string mainString)
        {
            int num = 0;
            int num2 = (mainString.Length - target.Length) + 1;
            for (int i = 0; i < num2; i++)
            {
                if (mainString.Substring(i, target.Length).Equals(target))
                {
                    num++;
                }
            }
            return num;
        }

        private void promptUpdate()
        {
            this.updating = true;
            this.saveInfo("awbuyupdate");
            base.Visible = false;
            this.clicker.running = false;
            new Thread(new ThreadStart(this.clicker.killDriver)).Start();
            new Thread(new ThreadStart(this.buyer.Stop)).Start();
            Thread.Sleep(0x3e8);
            ApplicationDeployment.CurrentDeployment.Update();
            Application.Restart();
        }

        /// <summary>
        /// Re-enables the controls that are disabled when the program is running.
        /// </summary>
        private void resetForm()
        {
            this.startStop_btn.Text = "Start";
            this.email_txt.Enabled = true;
            this.password_txt.Enabled = true;
            this.startMins.Enabled = true;
            this.endMins.Enabled = true;
            this.GoalBox.Enabled = true;
            this.Click_chk.Enabled = true;
            this.buyPct_txt.Enabled = true;
            this.bankPct_txt.Enabled = true;
            this.sendMorale_chk.Enabled = true;
            this.sendUserID_txt.Enabled = true;
        }

        /// <summary>
        /// Atttempts to force kill both clicker driver and buyer driver.
        /// </summary>
        public void KillDrivers()
        {
            buyer.killDriver();
            clicker.killDriver();
        }

        private bool setClickerMorale()
        {
            if (this.sendMorale_chk.Checked)
            {
                int num;
                if (!int.TryParse(this.sendUserID_txt.Text, out num))
                {
                    MessageBox.Show("Invalid clicker ID. It MUST be the user's ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }
                this.clicker.setSendMorale(this.sendMorale_chk.Checked);
                this.clicker.setSendToUser(this.sendUserID_txt.Text);
            }
            return true;
        }

        /// Running clicker and buyer
        private void StartThreads()
        {
            if (!buyer.CurrentGoing)
            {
                this.bThread = new Thread(new ThreadStart(this.buyer.Run));
                this.bThread.Start();
            }

        }

        private void updateClickerTab()
        {
            this.Clicked_txt.Text = this.buyer.formatInt((long)this.clicker.getClicked());
            this.totalClicks_txt.Text = this.buyer.formatInt((long)this.clicker.getClicked());
            this.captchaAttempts_txt.Text = this.buyer.formatInt((long)(this.clicker.captchaAttempts + buyer.captchaAttempts));
            this.captchaSuccess_txt.Text = this.buyer.formatInt((long)(this.clicker.captchaSuccess + buyer.captchaSuccess));
            if (this.clicker.captchaAttempts > 0)
            {
                this.captchaRate_txt.Text = Convert.ToString((int)((((double)this.clicker.captchaSuccess) / ((double)this.clicker.captchaAttempts)) * 100.0)) + " %";
            }
        }

        private void updateClicks()
        {
            if (this.Click_chk.Checked)
            {
                if (this.clicker.running || this.clicker.getClickedToday().Contains("error"))
                {
                    if (!this.clicker.SolvingCaptcha)
                    {
                        this.bottomStrip_txt.Text = "Clicker: " + this.clicker.getClickedToday();
                    }
                    else
                    {
                        this.bottomStrip_txt.Text = "Captcha: " + this.clicker.getClickedToday();
                    }
                }
                else
                {
                    this.bottomStrip_txt.Text = "Clicker: Not Running";
                }
            }
            this.balance_txt.Text = "Balance: " + this.buyer.bankAmt;
        }

        /// <summary>
        /// Update labels and start clicker and buyer.
        /// </summary>
        private void updateLabels()
        {
            this.buyer.doTransactions = this.bankBuy_chk.Checked;
            this.wepPurch_lbl.Text = this.buyer.formatInt(this.buyer.GetPurchased());
            this.goldBank_lbl.Text = this.buyer.formatInt(this.buyer.GetBanked());
            
            if (this.buyer.isRunning())
            {
                this.time_lbl.Text = this.buyer.TimeTillPurchase();
                this.updateTips();
                if (Environment.TickCount > this.buyer.timeToBuy)
                {
                    if (!this.buyer.CurrentGoing)
                    {
                        new Thread(new ThreadStart(this.buyer.Run)).Start();
                    }
                }
                if (this.Click_chk.Checked && !clicker.justFinished && !this.clicker.running && buyer.clicksAvail)
                {
                    new Thread(new ThreadStart(this.clicker.Start)).Start();
                }
            }
            else
            {
                this.time_lbl.Text = "Not Running";
            }
            if (!this.buyer.isRunning() && !this.email_txt.Enabled)
            {
                this.resetForm();
                if (this.buyer.loginError)
                {
                    MessageBox.Show("Invalid email or password?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        /// <summary>
        /// Controls the text that the user see when they scroll over icon in task bar.
        /// </summary>
        private void updateTips()
        {
            notifyIcon.Text = "";
            StringBuilder builder = new StringBuilder();
            if (!this.banking || (Convert.ToInt16(this.buyPct_txt.Text) > 0))
            {
                builder.Append("Weps Purchased: ").Append(this.buyer.formatInt(this.buyer.GetPurchased())).Append('\n');
            }
            if (this.banking)
            {
                string goldtext = "";
                long gold = this.buyer.GetBanked();
                if (gold > 1000000000)
                {
                    goldtext = gold.ToString().Substring(0, 4).Insert(1 + (int)(gold / 10000000000), ".") + "B";
                    builder.Append("Banked: ").Append(goldtext).Append('\n');
                }
                else
                {
                    builder.Append("Banked: ").Append(this.buyer.formatInt(this.buyer.GetBanked())).Append('\n');
                }
            }
            builder.Append("Next Bank: ").Append(this.buyer.TimeTillPurchase());
            this.notifyIcon.Text = builder.ToString();
        }

        /// <summary>
        /// Updates the stats of the character on the buyer page.
        /// </summary>
        private void updateStats()
        {
            strikeAction_lbl.Text = buyer.accountStats.strikeAction;
            defensiveAction_lbl.Text = buyer.accountStats.defensiveAction;
            spyRating_lbl.Text = buyer.accountStats.spyRating;
            sentryRating_lbl.Text = buyer.accountStats.sentryRating;
        }

        /// <summary>
        /// Handles an event being added to eventManagementlist
        /// </summary>
        /// <param name="add">True if the event is being added, false to remove</param>
        /// <param name="name">name of the event, case sensitive</param>
        private void ChangedEvent(bool add, string name)
        {
            if (add && !eventManagementList.Contains(name))
            {
                eventManagementList += name + ';';
            }
            else if (!add && eventManagementList.Contains(name))
            {
                eventManagementList = eventManagementList.Replace(name + ";", "");
            }
            buyer.eventsToManage(eventManagementList);
        }


        //
        // Event Functions 
        //
        private void bankBuy_chk_CheckedChanged(object sender, EventArgs e)
        {
            this.bankPct_txt.Enabled = this.buyPct_txt.Enabled = this.bankBuy_chk.Checked;
            this.buyer.doTransactions = this.bankBuy_chk.Checked;
        }

        private void bankPct_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.invalidDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void bankPct_txt_TextChanged(object sender, EventArgs e)
        {
            if (this.bankPct_txt.Text == "")
            {
                this.buyPct_txt.Text = "100";
            }
            else
            {
                int num = Convert.ToInt16(this.bankPct_txt.Text);
                if (num > 100)
                {
                    this.bankPct_txt.Text = "100";
                }
                this.bankPct_txt.Select(this.bankPct_txt.TextLength, 0);
                this.buyPct_txt.Text = Convert.ToString((int) (100 - Convert.ToInt16(this.bankPct_txt.Text)));
                if (num == 0)
                {
                    this.banking = false;
                }
                else
                {
                    this.banking = true;
                }
                if (!this.buyer.isRunning())
                {
                    this.GoalBox.Enabled = this.banking;
                }
                this.buyer.setPercents(Convert.ToInt16(this.buyPct_txt.Text), Convert.ToInt16(this.bankPct_txt.Text));
            }
        }

        private void buyPct_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.invalidDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void buyPct_txt_TextChanged(object sender, EventArgs e)
        {
            if (this.buyPct_txt.Text == "")
            {
                this.bankPct_txt.Text = "100";
            }
            else
            {
                if (Convert.ToInt16(this.buyPct_txt.Text) > 100)
                {
                    this.buyPct_txt.Text = "100";
                }
                this.buyPct_txt.Select(this.buyPct_txt.TextLength, 0);
                this.bankPct_txt.Text = Convert.ToString((int) (100 - Convert.ToInt16(this.buyPct_txt.Text)));
                this.buyer.setPercents(Convert.ToInt16(this.buyPct_txt.Text), Convert.ToInt16(this.bankPct_txt.Text));
            }
        }

        private void endMins_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.invalidDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GoalBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar)) && (e.KeyChar != ',')) || (((e.KeyChar == '\b') && (this.GoalBox.SelectionStart == 0)) && (this.GoalBox.SelectionLength == 0)))
            {
                e.Handled = true;
            }
            if (((this.GoalBox.SelectionStart > 0) && (e.KeyChar == '\b')) && ((this.GoalBox.SelectionLength == 0) && (this.GoalBox.Text[this.GoalBox.SelectionStart - 1] == ',')))
            {
                int selectionStart = this.GoalBox.SelectionStart;
                this.GoalBox.Text = this.GoalBox.Text.Remove(selectionStart - 2, 1);
                this.GoalBox.Select(selectionStart - 1, 0);
            }
        }

        private void GoalBox_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = this.GoalBox.SelectionStart;
            int num2 = 0;
            for (int i = 0; i < this.GoalBox.Text.Length; i++)
            {
                if (!char.IsDigit(this.GoalBox.Text[i]) && (this.GoalBox.Text[i] != ','))
                {
                    num2++;
                    if (num2 == 1)
                    {
                        selectionStart = i;
                    }
                    this.GoalBox.Text = this.GoalBox.Text.Remove(i, 1);
                    i--;
                }
            }
            if (selectionStart >= this.GoalBox.Text.Length)
            {
                selectionStart = this.GoalBox.Text.Length;
            }
            if (num2 > 0)
            {
                this.GoalBox.Select(selectionStart, 0);
            }
            if (!this.GoalBox.Text.Equals(""))
            {
                if (Convert.ToInt64(this.GoalBox.Text.Replace(",", "")) > 0x2540be400L)
                {
                    this.GoalBox.Text = "10,000,000,000";
                    this.GoalBox.Select(this.GoalBox.TextLength, 0);
                }
                else
                {
                    selectionStart = this.GoalBox.SelectionStart;
                    num2 = this.occuranceCount(',', this.GoalBox.Text);
                    this.GoalBox.Text = this.buyer.formatInt(Convert.ToInt64(this.GoalBox.Text.Replace(",", "")));
                    num2 = this.occuranceCount(',', this.GoalBox.Text) - num2;
                    if (selectionStart > 0)
                    {
                        this.GoalBox.Select(selectionStart + num2, 0);
                    }
                }
            }
        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.clicker.running = false;
            new Thread(new ThreadStart(this.clicker.killDriver)).Start();
            new Thread(new ThreadStart(this.buyer.Stop)).Start();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.saveOnExit_chk.Checked)
            {
                this.saveInfo("awbuyupdate");
            }
            else if ((!this.updating && !this.email_txt.Text.Equals("Email")) && !this.password_txt.Text.Equals("♫♫♫♫♫♫"))
            {
                if (MessageBox.Show("Would you like to save your info and stats?", "Save Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    this.deleteInfo("awbuyupdate");
                }
                else
                {
                    this.saveInfo("awbuyupdate");
                }
            }
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            this.timer.Tick += new EventHandler(this.timer_Tick);
            this.timer.Interval = 0x3e8;
            this.timer.Start();
            this.updateCheckTimer.Tick += new EventHandler(this.UpChk);
            this.updateCheckTimer.Interval = (this.minsTillTime() * 60) * 0x3e8;
            this.updateCheckTimer.Start();
            this.notifyIcon.Icon = Properties.Resources.favicon;
            this.notifyIcon.Text = "Total weapons purchased: " + this.buyer.formatInt(this.buyer.GetPurchased());
            this.notifyIcon.Click += new EventHandler(this.NotifyIcon_Click);
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == base.WindowState)
            {
                this.notifyIcon.Visible = true;
                if (!minimizeNotification)
                {
                    this.notifyIcon.BalloonTipTitle = "Arcane Warriors Buyer";
                    this.notifyIcon.BalloonTipText = "Minimized to tray.";
                    this.notifyIcon.ShowBalloonTip(1000);
                    minimizeNotification = true;
                }
                base.Hide();
            }
            else if (base.WindowState == FormWindowState.Normal)
            {
                this.notifyIcon.Visible = false;
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            base.Show();
            base.WindowState = FormWindowState.Normal;
        }

        private void SendByID_CheckedChanged(object sender, EventArgs e)
        {
            this.sendUserID_txt.Enabled = this.sendMorale_chk.Checked;
        }

        private void startMins_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.invalidDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// START/STOP Button execution
        private void startStop_btn_Click(object sender, EventArgs e)
        {
            this.buyer.fullBank = false;
            if (this.buyer.isRunning())
            {
                new Thread(new ThreadStart(this.buyer.Stop)).Start();
                this.clicker.running = false;
                new Thread(new ThreadStart(this.clicker.killDriver)).Start();
                this.resetForm();
            }
            else if (!this.checkLogin(this.email_txt.Text, this.password_txt.Text, "Email", "♫♫♫♫♫♫", 6, 0))
            {
                MessageBox.Show("Invalid login info format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (!this.bankBuy_chk.Checked && !this.Click_chk.Checked)
            {
                MessageBox.Show("You chose not to not bank, buy or click.\nWhat are you doing?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (this.setClickerMorale())
            {
                try
                {
                    if (Convert.ToInt32(this.startMins.Text) > Convert.ToInt32(this.endMins.Text))
                    {
                        this.buyer.setDetails(this.email_txt.Text, this.password_txt.Text, this.endMins.Text, this.startMins.Text);
                    }
                    else
                    {
                        this.buyer.setDetails(this.email_txt.Text, this.password_txt.Text, this.startMins.Text, this.endMins.Text);
                    }
                    this.buyer.setPercents(Convert.ToInt16(this.buyPct_txt.Text), Convert.ToInt16(this.bankPct_txt.Text));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Invalid time interval.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                if (Convert.ToInt64(this.GoalBox.Text.Replace(",", "")) == 0L)
                {
                    this.buyer.BankGoal = 20000000000;
                }
                else
                {
                    this.buyer.BankGoal = Convert.ToInt64(this.GoalBox.Text.Replace(",", ""));
                }

                this.buyer.Start();
                if (this.buyer.isRunning())
                {
                    this.buyer.doTransactions = this.bankBuy_chk.Checked;
                    this.disableFields();
                    this.startStop_btn.Text = "Stop";
                    if (!this.Click_chk.Checked)
                    {
                        this.Clicked_txt.Text = "Not Clicking.";
                    }
                    this.clicker.SetInfo(this.email_txt.Text, this.password_txt.Text);
                    this.StartThreads();
                }
            }
        }

        /// Checking for an update
        private void UpChk(object sender, EventArgs e)
        {
            return;
            if (this.autoUpdate_chk.Checked)
            {
                if (this.updateCheckTimer.Interval != ((this.checkUpdates * 60) * 0x3e8))
                {
                    this.updateCheckTimer.Interval = (this.checkUpdates * 60) * 0x3e8;
                }
                if (this.CheckForUpdate() && !this.promptedUpdate)
                {
                    this.promptedUpdate = true;
                    this.promptUpdate();
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            updateClicks();
            updateLabels();
            updateClickerTab();
            updateStats();
        }

        private void darkTheme_Chk_CheckedChanged(object sender, EventArgs e)
        {
            if (this.darkTheme_Chk.Checked)
            {
                this.general_tab.ForeColor = Color.WhiteSmoke;
                this.general_tab.BackColor = Color.FromArgb(50, 50, 50);
                this.BackColor = Color.FromArgb(50, 50, 50);
                this.bottomStrip.BackColor = Color.FromArgb(90, 90, 90);
                //this.Color
            }
            else
            {
                this.general_tab.ForeColor = Color.Black;
                this.general_tab.BackColor = Color.White;
                this.BackColor = Color.White;
            }
        }

        private void buySoldiers_chk_CheckedChanged(object sender, EventArgs e)
        {
            buyer.buySoldiers = buySoldiers_chk.Checked;
        }

        private void buyAllSoldiers_chk_CheckedChanged(object sender, EventArgs e)
        {
            buyer.matchSoldiers = buyAllSoldiers_chk.Checked;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.bottomStrip = new System.Windows.Forms.StatusStrip();
            this.bottomStrip_txt = new System.Windows.Forms.ToolStripStatusLabel();
            this.gapfiller = new System.Windows.Forms.ToolStripStatusLabel();
            this.balance_txt = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.general_tab = new System.Windows.Forms.TabPage();
            this.bankBuy_chk = new System.Windows.Forms.CheckBox();
            this.goldBank_lbl = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Clicked_txt = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Click_chk = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buyPct_txt = new System.Windows.Forms.TextBox();
            this.bankPct_txt = new System.Windows.Forms.TextBox();
            this.GoalBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.endMins = new System.Windows.Forms.TextBox();
            this.startMins = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.time_lbl = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.wepPurch_lbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.startStop_btn = new System.Windows.Forms.Button();
            this.password_txt = new System.Windows.Forms.TextBox();
            this.email_txt = new System.Windows.Forms.TextBox();
            this.clicker_tab = new System.Windows.Forms.TabPage();
            this.clicksettings_box = new System.Windows.Forms.GroupBox();
            this.sendUserID_txt = new System.Windows.Forms.TextBox();
            this.sendMorale_chk = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.clickstats_box = new System.Windows.Forms.GroupBox();
            this.captchaRate_txt = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.captchaSuccess_txt = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.captchaAttempts_txt = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.totalClicks_txt = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.buyer_tab = new System.Windows.Forms.TabPage();
            this.buyStats_gb = new System.Windows.Forms.GroupBox();
            this.strikeAction_lbl = new System.Windows.Forms.Label();
            this.defensiveAction_lbl = new System.Windows.Forms.Label();
            this.sentryRating_lbl = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.spyRating_lbl = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.buyerSettings_gb = new System.Windows.Forms.GroupBox();
            this.buyAllSoldiers_chk = new System.Windows.Forms.CheckBox();
            this.buySoldiers_chk = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.buySoldiers_lbl = new System.Windows.Forms.Label();
            this.moreSettings_tab = new System.Windows.Forms.TabPage();
            this.settings_tab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.darkTheme_lbl = new System.Windows.Forms.Label();
            this.darkTheme_Chk = new System.Windows.Forms.CheckBox();
            this.saveOnExit_chk = new System.Windows.Forms.CheckBox();
            this.saveExit_lbl = new System.Windows.Forms.Label();
            this.autoUpdate_chk = new System.Windows.Forms.CheckBox();
            this.autoUpdate_lbl = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.eventSettings_gb = new System.Windows.Forms.GroupBox();
            this.eventWhatSent_chk = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.eventGoldRush_chk = new System.Windows.Forms.CheckBox();
            this.eventTaxFree_chk = new System.Windows.Forms.CheckBox();
            this.eventStudyHall_chk = new System.Windows.Forms.CheckBox();
            this.bottomStrip.SuspendLayout();
            this.tabs.SuspendLayout();
            this.general_tab.SuspendLayout();
            this.clicker_tab.SuspendLayout();
            this.clicksettings_box.SuspendLayout();
            this.clickstats_box.SuspendLayout();
            this.buyer_tab.SuspendLayout();
            this.buyStats_gb.SuspendLayout();
            this.buyerSettings_gb.SuspendLayout();
            this.moreSettings_tab.SuspendLayout();
            this.settings_tab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.eventSettings_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // bottomStrip
            // 
            this.bottomStrip.Enabled = false;
            this.bottomStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.bottomStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bottomStrip_txt,
            this.gapfiller,
            this.balance_txt});
            this.bottomStrip.Location = new System.Drawing.Point(0, 219);
            this.bottomStrip.Name = "bottomStrip";
            this.bottomStrip.Size = new System.Drawing.Size(264, 22);
            this.bottomStrip.SizingGrip = false;
            this.bottomStrip.TabIndex = 27;
            this.bottomStrip.Text = "Test";
            // 
            // bottomStrip_txt
            // 
            this.bottomStrip_txt.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.bottomStrip_txt.Font = new System.Drawing.Font("Tahoma", 7.5F);
            this.bottomStrip_txt.Name = "bottomStrip_txt";
            this.bottomStrip_txt.Size = new System.Drawing.Size(100, 17);
            this.bottomStrip_txt.Text = "Clicker: Not Running";
            // 
            // gapfiller
            // 
            this.gapfiller.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.gapfiller.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.gapfiller.Name = "gapfiller";
            this.gapfiller.Size = new System.Drawing.Size(99, 17);
            this.gapfiller.Spring = true;
            // 
            // balance_txt
            // 
            this.balance_txt.Font = new System.Drawing.Font("Tahoma", 7.5F);
            this.balance_txt.Name = "balance_txt";
            this.balance_txt.Size = new System.Drawing.Size(50, 17);
            this.balance_txt.Text = "Balance: -";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.general_tab);
            this.tabs.Controls.Add(this.clicker_tab);
            this.tabs.Controls.Add(this.buyer_tab);
            this.tabs.Controls.Add(this.moreSettings_tab);
            this.tabs.Controls.Add(this.settings_tab);
            this.tabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Margin = new System.Windows.Forms.Padding(0);
            this.tabs.Name = "tabs";
            this.tabs.RightToLeftLayout = true;
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(266, 220);
            this.tabs.TabIndex = 28;
            // 
            // general_tab
            // 
            this.general_tab.BackColor = System.Drawing.Color.White;
            this.general_tab.Controls.Add(this.bankBuy_chk);
            this.general_tab.Controls.Add(this.goldBank_lbl);
            this.general_tab.Controls.Add(this.label11);
            this.general_tab.Controls.Add(this.Clicked_txt);
            this.general_tab.Controls.Add(this.label10);
            this.general_tab.Controls.Add(this.Click_chk);
            this.general_tab.Controls.Add(this.label9);
            this.general_tab.Controls.Add(this.label8);
            this.general_tab.Controls.Add(this.buyPct_txt);
            this.general_tab.Controls.Add(this.bankPct_txt);
            this.general_tab.Controls.Add(this.GoalBox);
            this.general_tab.Controls.Add(this.label7);
            this.general_tab.Controls.Add(this.label6);
            this.general_tab.Controls.Add(this.endMins);
            this.general_tab.Controls.Add(this.startMins);
            this.general_tab.Controls.Add(this.label5);
            this.general_tab.Controls.Add(this.time_lbl);
            this.general_tab.Controls.Add(this.label4);
            this.general_tab.Controls.Add(this.label3);
            this.general_tab.Controls.Add(this.label2);
            this.general_tab.Controls.Add(this.wepPurch_lbl);
            this.general_tab.Controls.Add(this.label1);
            this.general_tab.Controls.Add(this.startStop_btn);
            this.general_tab.Controls.Add(this.password_txt);
            this.general_tab.Controls.Add(this.email_txt);
            this.general_tab.Location = new System.Drawing.Point(4, 22);
            this.general_tab.Name = "general_tab";
            this.general_tab.Padding = new System.Windows.Forms.Padding(3);
            this.general_tab.Size = new System.Drawing.Size(258, 194);
            this.general_tab.TabIndex = 0;
            this.general_tab.Text = "General";
            // 
            // bankBuy_chk
            // 
            this.bankBuy_chk.AutoSize = true;
            this.bankBuy_chk.Checked = true;
            this.bankBuy_chk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bankBuy_chk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bankBuy_chk.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bankBuy_chk.Location = new System.Drawing.Point(159, 145);
            this.bankBuy_chk.Name = "bankBuy_chk";
            this.bankBuy_chk.Size = new System.Drawing.Size(77, 18);
            this.bankBuy_chk.TabIndex = 51;
            this.bankBuy_chk.Text = "Bank/Buy";
            this.bankBuy_chk.UseVisualStyleBackColor = true;
            // 
            // goldBank_lbl
            // 
            this.goldBank_lbl.AutoSize = true;
            this.goldBank_lbl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goldBank_lbl.Location = new System.Drawing.Point(156, 94);
            this.goldBank_lbl.Name = "goldBank_lbl";
            this.goldBank_lbl.Size = new System.Drawing.Size(13, 13);
            this.goldBank_lbl.TabIndex = 50;
            this.goldBank_lbl.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 13);
            this.label11.TabIndex = 49;
            this.label11.Text = "Gold Banked:";
            // 
            // Clicked_txt
            // 
            this.Clicked_txt.AutoSize = true;
            this.Clicked_txt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clicked_txt.Location = new System.Drawing.Point(156, 109);
            this.Clicked_txt.Name = "Clicked_txt";
            this.Clicked_txt.Size = new System.Drawing.Size(13, 13);
            this.Clicked_txt.TabIndex = 48;
            this.Clicked_txt.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label10.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(8, 109);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 47;
            this.label10.Text = "Total Links Clicked:";
            // 
            // Click_chk
            // 
            this.Click_chk.AutoSize = true;
            this.Click_chk.Checked = true;
            this.Click_chk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Click_chk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Click_chk.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Click_chk.Location = new System.Drawing.Point(11, 170);
            this.Click_chk.Name = "Click_chk";
            this.Click_chk.Size = new System.Drawing.Size(53, 18);
            this.Click_chk.TabIndex = 46;
            this.Click_chk.Text = "Click";
            this.Click_chk.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label9.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(66, 172);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 45;
            this.label9.Text = "Bank %:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(173, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Buy %:";
            // 
            // buyPct_txt
            // 
            this.buyPct_txt.Location = new System.Drawing.Point(224, 170);
            this.buyPct_txt.MaxLength = 14;
            this.buyPct_txt.Multiline = true;
            this.buyPct_txt.Name = "buyPct_txt";
            this.buyPct_txt.Size = new System.Drawing.Size(27, 20);
            this.buyPct_txt.TabIndex = 43;
            this.buyPct_txt.Text = "0";
            this.buyPct_txt.TextChanged += new System.EventHandler(this.buyPct_txt_TextChanged);
            this.buyPct_txt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.buyPct_txt_KeyPress);
            // 
            // bankPct_txt
            // 
            this.bankPct_txt.Location = new System.Drawing.Point(126, 168);
            this.bankPct_txt.MaxLength = 14;
            this.bankPct_txt.Multiline = true;
            this.bankPct_txt.Name = "bankPct_txt";
            this.bankPct_txt.Size = new System.Drawing.Size(27, 20);
            this.bankPct_txt.TabIndex = 42;
            this.bankPct_txt.Text = "100";
            this.bankPct_txt.TextChanged += new System.EventHandler(this.bankPct_txt_TextChanged);
            this.bankPct_txt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.bankPct_txt_KeyPress);
            // 
            // GoalBox
            // 
            this.GoalBox.Location = new System.Drawing.Point(62, 142);
            this.GoalBox.MaxLength = 14;
            this.GoalBox.Multiline = true;
            this.GoalBox.Name = "GoalBox";
            this.GoalBox.Size = new System.Drawing.Size(91, 20);
            this.GoalBox.TabIndex = 41;
            this.GoalBox.Text = "0";
            this.GoalBox.TextChanged += new System.EventHandler(this.GoalBox_TextChanged);
            this.GoalBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GoalBox_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 40;
            this.label7.Text = "Bank Goal:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label6.Location = new System.Drawing.Point(199, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "to";
            // 
            // endMins
            // 
            this.endMins.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endMins.Location = new System.Drawing.Point(222, 55);
            this.endMins.MaxLength = 3;
            this.endMins.Name = "endMins";
            this.endMins.Size = new System.Drawing.Size(29, 21);
            this.endMins.TabIndex = 38;
            this.endMins.Text = "120";
            // 
            // startMins
            // 
            this.startMins.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startMins.Location = new System.Drawing.Point(159, 55);
            this.startMins.MaxLength = 3;
            this.startMins.Name = "startMins";
            this.startMins.Size = new System.Drawing.Size(27, 21);
            this.startMins.TabIndex = 37;
            this.startMins.Text = "60";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Time Interval (mins):";
            // 
            // time_lbl
            // 
            this.time_lbl.AutoSize = true;
            this.time_lbl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.time_lbl.Location = new System.Drawing.Point(156, 124);
            this.time_lbl.Name = "time_lbl";
            this.time_lbl.Size = new System.Drawing.Size(66, 13);
            this.time_lbl.TabIndex = 35;
            this.time_lbl.Text = "Not Running";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Next Transaction In:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Pass:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label2.Location = new System.Drawing.Point(8, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Email:";
            // 
            // wepPurch_lbl
            // 
            this.wepPurch_lbl.AutoSize = true;
            this.wepPurch_lbl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wepPurch_lbl.Location = new System.Drawing.Point(156, 79);
            this.wepPurch_lbl.Name = "wepPurch_lbl";
            this.wepPurch_lbl.Size = new System.Drawing.Size(13, 13);
            this.wepPurch_lbl.TabIndex = 31;
            this.wepPurch_lbl.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Weapons Purchased:";
            // 
            // startStop_btn
            // 
            this.startStop_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startStop_btn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.startStop_btn.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.startStop_btn.ForeColor = System.Drawing.Color.Transparent;
            this.startStop_btn.Location = new System.Drawing.Point(159, 3);
            this.startStop_btn.Name = "startStop_btn";
            this.startStop_btn.Size = new System.Drawing.Size(92, 48);
            this.startStop_btn.TabIndex = 29;
            this.startStop_btn.Text = "Start";
            this.startStop_btn.UseVisualStyleBackColor = true;
            this.startStop_btn.Click += new System.EventHandler(this.startStop_btn_Click);
            // 
            // password_txt
            // 
            this.password_txt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password_txt.Location = new System.Drawing.Point(43, 30);
            this.password_txt.Name = "password_txt";
            this.password_txt.PasswordChar = '*';
            this.password_txt.Size = new System.Drawing.Size(110, 21);
            this.password_txt.TabIndex = 28;
            this.password_txt.Text = "♫♫♫♫♫♫";
            // 
            // email_txt
            // 
            this.email_txt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.email_txt.Location = new System.Drawing.Point(43, 3);
            this.email_txt.MaxLength = 254;
            this.email_txt.Name = "email_txt";
            this.email_txt.Size = new System.Drawing.Size(110, 21);
            this.email_txt.TabIndex = 27;
            this.email_txt.Text = "Email";
            // 
            // clicker_tab
            // 
            this.clicker_tab.BackColor = System.Drawing.Color.White;
            this.clicker_tab.Controls.Add(this.clicksettings_box);
            this.clicker_tab.Controls.Add(this.clickstats_box);
            this.clicker_tab.Location = new System.Drawing.Point(4, 22);
            this.clicker_tab.Name = "clicker_tab";
            this.clicker_tab.Padding = new System.Windows.Forms.Padding(3);
            this.clicker_tab.Size = new System.Drawing.Size(258, 194);
            this.clicker_tab.TabIndex = 1;
            this.clicker_tab.Text = "Clicker";
            // 
            // clicksettings_box
            // 
            this.clicksettings_box.Controls.Add(this.sendUserID_txt);
            this.clicksettings_box.Controls.Add(this.sendMorale_chk);
            this.clicksettings_box.Controls.Add(this.label17);
            this.clicksettings_box.Controls.Add(this.label19);
            this.clicksettings_box.Location = new System.Drawing.Point(8, 93);
            this.clicksettings_box.Name = "clicksettings_box";
            this.clicksettings_box.Size = new System.Drawing.Size(240, 62);
            this.clicksettings_box.TabIndex = 2;
            this.clicksettings_box.TabStop = false;
            this.clicksettings_box.Text = "Clicker Settings:";
            // 
            // sendUserID_txt
            // 
            this.sendUserID_txt.Location = new System.Drawing.Point(118, 35);
            this.sendUserID_txt.Name = "sendUserID_txt";
            this.sendUserID_txt.Size = new System.Drawing.Size(78, 20);
            this.sendUserID_txt.TabIndex = 5;
            // 
            // sendMorale_chk
            // 
            this.sendMorale_chk.AutoSize = true;
            this.sendMorale_chk.Location = new System.Drawing.Point(118, 16);
            this.sendMorale_chk.Name = "sendMorale_chk";
            this.sendMorale_chk.Size = new System.Drawing.Size(15, 14);
            this.sendMorale_chk.TabIndex = 4;
            this.sendMorale_chk.UseVisualStyleBackColor = true;
            this.sendMorale_chk.CheckedChanged += new System.EventHandler(this.SendByID_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 38);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(46, 13);
            this.label17.TabIndex = 2;
            this.label17.Text = "User ID:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 16);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Send Morale:";
            // 
            // clickstats_box
            // 
            this.clickstats_box.Controls.Add(this.captchaRate_txt);
            this.clickstats_box.Controls.Add(this.label20);
            this.clickstats_box.Controls.Add(this.captchaSuccess_txt);
            this.clickstats_box.Controls.Add(this.label18);
            this.clickstats_box.Controls.Add(this.captchaAttempts_txt);
            this.clickstats_box.Controls.Add(this.label14);
            this.clickstats_box.Controls.Add(this.totalClicks_txt);
            this.clickstats_box.Controls.Add(this.label12);
            this.clickstats_box.Location = new System.Drawing.Point(8, 6);
            this.clickstats_box.Name = "clickstats_box";
            this.clickstats_box.Size = new System.Drawing.Size(240, 81);
            this.clickstats_box.TabIndex = 1;
            this.clickstats_box.TabStop = false;
            this.clickstats_box.Text = "Statistics:";
            // 
            // captchaRate_txt
            // 
            this.captchaRate_txt.AutoSize = true;
            this.captchaRate_txt.Location = new System.Drawing.Point(115, 64);
            this.captchaRate_txt.Name = "captchaRate_txt";
            this.captchaRate_txt.Size = new System.Drawing.Size(10, 13);
            this.captchaRate_txt.TabIndex = 7;
            this.captchaRate_txt.Text = "-";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 64);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(77, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Success Rate:";
            // 
            // captchaSuccess_txt
            // 
            this.captchaSuccess_txt.AutoSize = true;
            this.captchaSuccess_txt.Location = new System.Drawing.Point(115, 48);
            this.captchaSuccess_txt.Name = "captchaSuccess_txt";
            this.captchaSuccess_txt.Size = new System.Drawing.Size(10, 13);
            this.captchaSuccess_txt.TabIndex = 5;
            this.captchaSuccess_txt.Text = "-";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 48);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(94, 13);
            this.label18.TabIndex = 4;
            this.label18.Text = "Captcha Success:";
            // 
            // captchaAttempts_txt
            // 
            this.captchaAttempts_txt.AutoSize = true;
            this.captchaAttempts_txt.Location = new System.Drawing.Point(115, 32);
            this.captchaAttempts_txt.Name = "captchaAttempts_txt";
            this.captchaAttempts_txt.Size = new System.Drawing.Size(10, 13);
            this.captchaAttempts_txt.TabIndex = 3;
            this.captchaAttempts_txt.Text = "-";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(94, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Captcha Attempts:";
            // 
            // totalClicks_txt
            // 
            this.totalClicks_txt.AutoSize = true;
            this.totalClicks_txt.Location = new System.Drawing.Point(115, 16);
            this.totalClicks_txt.Name = "totalClicks_txt";
            this.totalClicks_txt.Size = new System.Drawing.Size(10, 13);
            this.totalClicks_txt.TabIndex = 1;
            this.totalClicks_txt.Text = "-";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Total Links Clicked:";
            // 
            // buyer_tab
            // 
            this.buyer_tab.Controls.Add(this.buyStats_gb);
            this.buyer_tab.Controls.Add(this.buyerSettings_gb);
            this.buyer_tab.Location = new System.Drawing.Point(4, 22);
            this.buyer_tab.Name = "buyer_tab";
            this.buyer_tab.Padding = new System.Windows.Forms.Padding(3);
            this.buyer_tab.Size = new System.Drawing.Size(258, 194);
            this.buyer_tab.TabIndex = 2;
            this.buyer_tab.Text = "Buyer";
            this.buyer_tab.UseVisualStyleBackColor = true;
            // 
            // buyStats_gb
            // 
            this.buyStats_gb.Controls.Add(this.strikeAction_lbl);
            this.buyStats_gb.Controls.Add(this.defensiveAction_lbl);
            this.buyStats_gb.Controls.Add(this.sentryRating_lbl);
            this.buyStats_gb.Controls.Add(this.label25);
            this.buyStats_gb.Controls.Add(this.spyRating_lbl);
            this.buyStats_gb.Controls.Add(this.label27);
            this.buyStats_gb.Controls.Add(this.label28);
            this.buyStats_gb.Controls.Add(this.label29);
            this.buyStats_gb.Location = new System.Drawing.Point(8, 93);
            this.buyStats_gb.Name = "buyStats_gb";
            this.buyStats_gb.Size = new System.Drawing.Size(240, 95);
            this.buyStats_gb.TabIndex = 3;
            this.buyStats_gb.TabStop = false;
            this.buyStats_gb.Text = "Stats";
            // 
            // strikeAction_lbl
            // 
            this.strikeAction_lbl.AutoSize = true;
            this.strikeAction_lbl.Location = new System.Drawing.Point(115, 16);
            this.strikeAction_lbl.Name = "strikeAction_lbl";
            this.strikeAction_lbl.Size = new System.Drawing.Size(10, 13);
            this.strikeAction_lbl.TabIndex = 9;
            this.strikeAction_lbl.Text = "-";
            // 
            // defensiveAction_lbl
            // 
            this.defensiveAction_lbl.AutoSize = true;
            this.defensiveAction_lbl.Location = new System.Drawing.Point(115, 32);
            this.defensiveAction_lbl.Name = "defensiveAction_lbl";
            this.defensiveAction_lbl.Size = new System.Drawing.Size(10, 13);
            this.defensiveAction_lbl.TabIndex = 8;
            this.defensiveAction_lbl.Text = "-";
            // 
            // sentryRating_lbl
            // 
            this.sentryRating_lbl.AutoSize = true;
            this.sentryRating_lbl.Location = new System.Drawing.Point(115, 64);
            this.sentryRating_lbl.Name = "sentryRating_lbl";
            this.sentryRating_lbl.Size = new System.Drawing.Size(10, 13);
            this.sentryRating_lbl.TabIndex = 7;
            this.sentryRating_lbl.Text = "-";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 64);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(71, 13);
            this.label25.TabIndex = 6;
            this.label25.Text = "Sentry Rating";
            // 
            // spyRating_lbl
            // 
            this.spyRating_lbl.AutoSize = true;
            this.spyRating_lbl.Location = new System.Drawing.Point(115, 48);
            this.spyRating_lbl.Name = "spyRating_lbl";
            this.spyRating_lbl.Size = new System.Drawing.Size(10, 13);
            this.spyRating_lbl.TabIndex = 5;
            this.spyRating_lbl.Text = "-";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(6, 48);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(59, 13);
            this.label27.TabIndex = 4;
            this.label27.Text = "Spy Rating";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 16);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(67, 13);
            this.label28.TabIndex = 3;
            this.label28.Text = "Strike Action";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(6, 32);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(88, 13);
            this.label29.TabIndex = 2;
            this.label29.Text = "Defensive Action";
            // 
            // buyerSettings_gb
            // 
            this.buyerSettings_gb.Controls.Add(this.buyAllSoldiers_chk);
            this.buyerSettings_gb.Controls.Add(this.buySoldiers_chk);
            this.buyerSettings_gb.Controls.Add(this.label13);
            this.buyerSettings_gb.Controls.Add(this.label15);
            this.buyerSettings_gb.Controls.Add(this.label16);
            this.buyerSettings_gb.Controls.Add(this.label21);
            this.buyerSettings_gb.Controls.Add(this.label23);
            this.buyerSettings_gb.Controls.Add(this.buySoldiers_lbl);
            this.buyerSettings_gb.Location = new System.Drawing.Point(6, 6);
            this.buyerSettings_gb.Name = "buyerSettings_gb";
            this.buyerSettings_gb.Size = new System.Drawing.Size(240, 81);
            this.buyerSettings_gb.TabIndex = 2;
            this.buyerSettings_gb.TabStop = false;
            this.buyerSettings_gb.Text = "Buyer Settings";
            // 
            // buyAllSoldiers_chk
            // 
            this.buyAllSoldiers_chk.AutoSize = true;
            this.buyAllSoldiers_chk.Location = new System.Drawing.Point(118, 31);
            this.buyAllSoldiers_chk.Name = "buyAllSoldiers_chk";
            this.buyAllSoldiers_chk.Size = new System.Drawing.Size(15, 14);
            this.buyAllSoldiers_chk.TabIndex = 9;
            this.buyAllSoldiers_chk.UseVisualStyleBackColor = true;
            this.buyAllSoldiers_chk.CheckedChanged += new System.EventHandler(this.buyAllSoldiers_chk_CheckedChanged);
            // 
            // buySoldiers_chk
            // 
            this.buySoldiers_chk.AutoSize = true;
            this.buySoldiers_chk.Location = new System.Drawing.Point(118, 15);
            this.buySoldiers_chk.Name = "buySoldiers_chk";
            this.buySoldiers_chk.Size = new System.Drawing.Size(15, 14);
            this.buySoldiers_chk.TabIndex = 8;
            this.buySoldiers_chk.UseVisualStyleBackColor = true;
            this.buySoldiers_chk.CheckedChanged += new System.EventHandler(this.buySoldiers_chk_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(115, 64);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(10, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "-";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 64);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(10, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "-";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(115, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(10, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "-";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 48);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(10, 13);
            this.label21.TabIndex = 4;
            this.label21.Text = "-";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Location = new System.Drawing.Point(6, 32);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(79, 13);
            this.label23.TabIndex = 2;
            this.label23.Text = "Buy All Soldiers";
            // 
            // buySoldiers_lbl
            // 
            this.buySoldiers_lbl.AutoSize = true;
            this.buySoldiers_lbl.Location = new System.Drawing.Point(6, 16);
            this.buySoldiers_lbl.Name = "buySoldiers_lbl";
            this.buySoldiers_lbl.Size = new System.Drawing.Size(99, 13);
            this.buySoldiers_lbl.TabIndex = 0;
            this.buySoldiers_lbl.Text = "Buy Attack Soldiers";
            // 
            // moreSettings_tab
            // 
            this.moreSettings_tab.Controls.Add(this.eventSettings_gb);
            this.moreSettings_tab.Location = new System.Drawing.Point(4, 22);
            this.moreSettings_tab.Name = "moreSettings_tab";
            this.moreSettings_tab.Padding = new System.Windows.Forms.Padding(3);
            this.moreSettings_tab.Size = new System.Drawing.Size(258, 194);
            this.moreSettings_tab.TabIndex = 3;
            this.moreSettings_tab.Text = "More Settings";
            this.moreSettings_tab.UseVisualStyleBackColor = true;
            // 
            // settings_tab
            // 
            this.settings_tab.BackColor = System.Drawing.Color.Transparent;
            this.settings_tab.Controls.Add(this.groupBox1);
            this.settings_tab.Location = new System.Drawing.Point(4, 22);
            this.settings_tab.Name = "settings_tab";
            this.settings_tab.Padding = new System.Windows.Forms.Padding(3);
            this.settings_tab.Size = new System.Drawing.Size(258, 194);
            this.settings_tab.TabIndex = 4;
            this.settings_tab.Text = "Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.darkTheme_lbl);
            this.groupBox1.Controls.Add(this.darkTheme_Chk);
            this.groupBox1.Controls.Add(this.saveOnExit_chk);
            this.groupBox1.Controls.Add(this.saveExit_lbl);
            this.groupBox1.Controls.Add(this.autoUpdate_chk);
            this.groupBox1.Controls.Add(this.autoUpdate_lbl);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // darkTheme_lbl
            // 
            this.darkTheme_lbl.AutoSize = true;
            this.darkTheme_lbl.Location = new System.Drawing.Point(6, 48);
            this.darkTheme_lbl.Name = "darkTheme_lbl";
            this.darkTheme_lbl.Size = new System.Drawing.Size(66, 13);
            this.darkTheme_lbl.TabIndex = 8;
            this.darkTheme_lbl.Text = "Dark Theme";
            // 
            // darkTheme_Chk
            // 
            this.darkTheme_Chk.AutoSize = true;
            this.darkTheme_Chk.Enabled = false;
            this.darkTheme_Chk.Location = new System.Drawing.Point(118, 48);
            this.darkTheme_Chk.Name = "darkTheme_Chk";
            this.darkTheme_Chk.Size = new System.Drawing.Size(15, 14);
            this.darkTheme_Chk.TabIndex = 7;
            this.darkTheme_Chk.UseVisualStyleBackColor = true;
            this.darkTheme_Chk.CheckedChanged += new System.EventHandler(this.darkTheme_Chk_CheckedChanged);
            // 
            // saveOnExit_chk
            // 
            this.saveOnExit_chk.AutoSize = true;
            this.saveOnExit_chk.Checked = true;
            this.saveOnExit_chk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveOnExit_chk.Location = new System.Drawing.Point(118, 32);
            this.saveOnExit_chk.Name = "saveOnExit_chk";
            this.saveOnExit_chk.Size = new System.Drawing.Size(15, 14);
            this.saveOnExit_chk.TabIndex = 6;
            this.saveOnExit_chk.UseVisualStyleBackColor = true;
            // 
            // saveExit_lbl
            // 
            this.saveExit_lbl.AutoSize = true;
            this.saveExit_lbl.Location = new System.Drawing.Point(6, 32);
            this.saveExit_lbl.Name = "saveExit_lbl";
            this.saveExit_lbl.Size = new System.Drawing.Size(67, 13);
            this.saveExit_lbl.TabIndex = 5;
            this.saveExit_lbl.Text = "Save on Exit";
            // 
            // autoUpdate_chk
            // 
            this.autoUpdate_chk.AutoSize = true;
            this.autoUpdate_chk.Checked = true;
            this.autoUpdate_chk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoUpdate_chk.Location = new System.Drawing.Point(118, 16);
            this.autoUpdate_chk.Name = "autoUpdate_chk";
            this.autoUpdate_chk.Size = new System.Drawing.Size(15, 14);
            this.autoUpdate_chk.TabIndex = 4;
            this.autoUpdate_chk.UseVisualStyleBackColor = true;
            // 
            // autoUpdate_lbl
            // 
            this.autoUpdate_lbl.AutoSize = true;
            this.autoUpdate_lbl.Location = new System.Drawing.Point(6, 16);
            this.autoUpdate_lbl.Name = "autoUpdate_lbl";
            this.autoUpdate_lbl.Size = new System.Drawing.Size(67, 13);
            this.autoUpdate_lbl.TabIndex = 0;
            this.autoUpdate_lbl.Text = "Auto Update";
            // 
            // eventSettings_gb
            // 
            this.eventSettings_gb.Controls.Add(this.eventStudyHall_chk);
            this.eventSettings_gb.Controls.Add(this.eventTaxFree_chk);
            this.eventSettings_gb.Controls.Add(this.eventWhatSent_chk);
            this.eventSettings_gb.Controls.Add(this.eventGoldRush_chk);
            this.eventSettings_gb.Controls.Add(this.label24);
            this.eventSettings_gb.Controls.Add(this.label30);
            this.eventSettings_gb.Controls.Add(this.label31);
            this.eventSettings_gb.Controls.Add(this.label32);
            this.eventSettings_gb.Location = new System.Drawing.Point(6, 6);
            this.eventSettings_gb.Name = "eventSettings_gb";
            this.eventSettings_gb.Size = new System.Drawing.Size(240, 81);
            this.eventSettings_gb.TabIndex = 3;
            this.eventSettings_gb.TabStop = false;
            this.eventSettings_gb.Text = "Event Settings";
            // 
            // eventWhatSent_chk
            // 
            this.eventWhatSent_chk.AutoSize = true;
            this.eventWhatSent_chk.Location = new System.Drawing.Point(187, 31);
            this.eventWhatSent_chk.Name = "eventWhatSent_chk";
            this.eventWhatSent_chk.Size = new System.Drawing.Size(15, 14);
            this.eventWhatSent_chk.TabIndex = 9;
            this.eventWhatSent_chk.UseVisualStyleBackColor = true;
            this.eventWhatSent_chk.CheckedChanged += new System.EventHandler(this.eventWhatSent_chk_CheckedChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 63);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(165, 13);
            this.label24.TabIndex = 6;
            this.label24.Text = "Transfer turns during \"Study Hall\"";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(6, 47);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(116, 13);
            this.label30.TabIndex = 4;
            this.label30.Text = "Bank during \"Tax free\"";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.Color.Transparent;
            this.label31.Location = new System.Drawing.Point(6, 31);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(156, 13);
            this.label31.TabIndex = 2;
            this.label31.Text = "Buy faster during \"What sentry\"";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(6, 15);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(149, 13);
            this.label32.TabIndex = 0;
            this.label32.Text = "Buy faster during \"Gold Rush\"";
            // 
            // eventGoldRush_chk
            // 
            this.eventGoldRush_chk.AutoSize = true;
            this.eventGoldRush_chk.Location = new System.Drawing.Point(187, 15);
            this.eventGoldRush_chk.Name = "eventGoldRush_chk";
            this.eventGoldRush_chk.Size = new System.Drawing.Size(15, 14);
            this.eventGoldRush_chk.TabIndex = 8;
            this.eventGoldRush_chk.UseVisualStyleBackColor = true;
            this.eventGoldRush_chk.CheckedChanged += new System.EventHandler(this.eventGoldRush_chk_CheckedChanged);
            // 
            // eventTaxFree_chk
            // 
            this.eventTaxFree_chk.AutoSize = true;
            this.eventTaxFree_chk.Location = new System.Drawing.Point(187, 47);
            this.eventTaxFree_chk.Name = "eventTaxFree_chk";
            this.eventTaxFree_chk.Size = new System.Drawing.Size(15, 14);
            this.eventTaxFree_chk.TabIndex = 10;
            this.eventTaxFree_chk.UseVisualStyleBackColor = true;
            this.eventTaxFree_chk.CheckedChanged += new System.EventHandler(this.eventTaxFree_chk_CheckedChanged);
            // 
            // eventStudyHall_chk
            // 
            this.eventStudyHall_chk.AutoSize = true;
            this.eventStudyHall_chk.Location = new System.Drawing.Point(187, 63);
            this.eventStudyHall_chk.Name = "eventStudyHall_chk";
            this.eventStudyHall_chk.Size = new System.Drawing.Size(15, 14);
            this.eventStudyHall_chk.TabIndex = 11;
            this.eventStudyHall_chk.UseVisualStyleBackColor = true;
            this.eventStudyHall_chk.CheckedChanged += new System.EventHandler(this.eventStudyHall_chk_CheckedChanged);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 241);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.bottomStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.RightToLeftLayout = true;
            this.Text = "AWBuy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainForm_FormClosed);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.Resize += new System.EventHandler(this.mainForm_Resize);
            this.bottomStrip.ResumeLayout(false);
            this.bottomStrip.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.general_tab.ResumeLayout(false);
            this.general_tab.PerformLayout();
            this.clicker_tab.ResumeLayout(false);
            this.clicksettings_box.ResumeLayout(false);
            this.clicksettings_box.PerformLayout();
            this.clickstats_box.ResumeLayout(false);
            this.clickstats_box.PerformLayout();
            this.buyer_tab.ResumeLayout(false);
            this.buyStats_gb.ResumeLayout(false);
            this.buyStats_gb.PerformLayout();
            this.buyerSettings_gb.ResumeLayout(false);
            this.buyerSettings_gb.PerformLayout();
            this.moreSettings_tab.ResumeLayout(false);
            this.settings_tab.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.eventSettings_gb.ResumeLayout(false);
            this.eventSettings_gb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void eventGoldRush_chk_CheckedChanged(object sender, EventArgs e)
        {
            ChangedEvent(eventGoldRush_chk.Checked, "Gold Rush");
        }

        private void eventWhatSent_chk_CheckedChanged(object sender, EventArgs e)
        {
            ChangedEvent(eventWhatSent_chk.Checked, "What sentry?");
        }

        private void eventTaxFree_chk_CheckedChanged(object sender, EventArgs e)
        {
            ChangedEvent(eventTaxFree_chk.Checked, "Tax free");
        }

        private void eventStudyHall_chk_CheckedChanged(object sender, EventArgs e)
        {
            ChangedEvent(eventStudyHall_chk.Checked, "Study Hall");
        }
    }
}

