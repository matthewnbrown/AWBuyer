namespace AWbuy.AWbuy.Res
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.PhantomJS;
    using OpenQA.Selenium.Chrome;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.NetworkInformation;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using AWbuy.Res;
   
    internal class Clicker
    {
        private string _clickedToday;
        private const string BASE_URL = "www.AW.net";
        private const string CLICKER_URL = "http://www.aw.net/index.php/yar";
        //private ChromeDriver driver;
        private PhantomJSDriver driver;
        private PhantomJSDriverService driverService;
        private int[] keys = new int[] { 5126, 4834, 5282, 3598, 4354, 5006, 3618, 5066, 6286 };
        private string[] letters = new string[] { "a", "c", "e", "i", "n", "o", "r", "s", "w" };
        private const string LOGIN_URL = "http://www.aw.net/index.php/index";
        private const string MORALE_URL = "http://www.aw.net/index.php/morale_transfer";
        private string phantomLoc = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private Random rand = new Random();
        private string sendID;
        private bool sendToMatt = false;
        private AWbuy.Res.AWTools Toolbox;
        public int getClicked() => this.totalClicked;
        //private string email;
        //private string password;
        private AWbuy.Res.AWTools.Account account;
        public string getClickedToday() => this._clickedToday;
        public bool running;

        // Properties 

        public int captchaAttempts { get { return Toolbox.captchaAttempts; } }

        public int captchaSuccess { get { return Toolbox.captchaSuccess; } }

        public int connectErrors { get; private set; }

        public bool SolvingCaptcha { get; private set; }
        
        public bool justFinished { get; private set; }

        public int totalClicked { get; set; }

        // Public Methods

        public Clicker()
        {
            this.running = false;
            this.SolvingCaptcha = false;
            driverService = PhantomJSDriverService.CreateDefaultService(this.phantomLoc);
            driverService.HideCommandPromptWindow = true;
            this._clickedToday = "";
            this.totalClicked = 0;
            justFinished = false;
            Toolbox = new AWbuy.Res.AWTools();
        }

        public void killDriver()
        {
            try
            {
                driver.Close();
                driver.Dispose();
            }
            catch (Exception)
            {
            }
            this.running = false;
        }

        public void SetInfo(string email, string password)
        {
            account.email = email;
            account.password = password;
        }

        public void setSendMorale(bool status)
        {
            this.sendToMatt = status;
            this.sendID = "";
        }

        public void setSendToUser(string TargetUser)
        {
            this.sendID = TargetUser;
        }

        public void Start()
        {
            this.running = true;
            this._clickedToday = "Loading Clicker";
            this.connectErrors = 0;
            while (!Toolbox.checkConnection())
            {
                this.connectErrors++;
                this._clickedToday = "Connection error";
                Thread.Sleep(0x1388);
                if (this.connectErrors == 10)
                {
                    this.running = false;
                    return;
                }
            }
            try
            {
                driver = new PhantomJSDriver(driverService);
                //driver = new ChromeDriver();
                Toolbox.Login(account, driver);
            }
            catch (Exception)
            {
                this.running = false;
            }
            if (this.running)
            {
                try
                {
                    if ((this.running && !driver.Url.Contains("error")) && !driver.Url.Contains("index.php/index"))
                    {
                        try
                        {
                            this.Run();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception exception)
                {
                    Toolbox.logError(exception);
                }
            }
            this.killDriver();
            Thread.Sleep(5000);
            this.running = false;
            finishedTimer();
        }



        // Private Methods

        private void getCaptcha()
        {
            SolvingCaptcha = true;
            Toolbox.getCaptcha(ref driver, ref running, ref _clickedToday, account);
            SolvingCaptcha = false;

        }

        private void Run()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int totalClicked = this.totalClicked;
            int millisecondsTimeout = 35;
            bool flag = false;
            bool flag2 = false;
            string attribute = "";
            driver.Navigate().GoToUrl(CLICKER_URL);
            getCaptcha();

            if (!checkDone(""))
            {
                IWebElement element2;
                try
                {
                    driver.FindElement(By.Id("overlay-shade"));
                    element2 = driver.FindElement(By.Id("clicked_today"));
                }
                catch (Exception)
                {
                    return;
                }
                this._clickedToday = "Waiting for website..";
                while (num == 0)
                {
                    if (!this.running)
                    {
                        return;
                    }
                    try
                    {
                        Thread.Sleep(50);
                        num3 = Convert.ToInt32(driver.FindElement(By.Id("total_clicked")).Text);
                        num++;
                        continue;
                    }
                    catch (Exception)
                    {
                        driver.Navigate().GoToUrl(CLICKER_URL);
                        getCaptcha();
                        continue;
                    }
                }
                IWebElement element = driver.FindElement(By.XPath("//*[@id=\"info_box\"]/input"));
                if (!this.checkDone(element2.Text))
                {
                    IList<IWebElement> list = driver.FindElement(By.Id("captcha_box")).FindElements(By.XPath("//img"));
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].GetAttribute("id") == "0")
                        {
                            num5 = i;
                            break;
                        }
                    }
                    flag = true;
                    while (this.running)
                    {
                        try
                        {
                            while (!Toolbox.checkConnection())
                            {
                                this.connectErrors++;
                                this._clickedToday = "Connection error";
                                Thread.Sleep(0x1388);
                                if (this.connectErrors == 10)
                                {
                                    this.running = false;
                                    return;
                                }
                            }
                            if (!driver.Url.ToLower().Contains("AW"))
                            {
                                this._clickedToday = "Returned!";
                                return;
                            }

                            if (Toolbox.ElementIsVisible(By.Id("overlay-captcha"), driver))
                            {
                                getCaptcha();
                            }

                            element2 = driver.FindElement(By.Id("clicked_today"));
                            this._clickedToday = element2.Text;

                            if (this.checkDone(element2.Text))
                            {
                                break;
                            }

                            num = Convert.ToInt32(driver.FindElement(By.Id("links_loaded")).Text);
                            element = driver.FindElement(By.XPath("//*[@id=\"info_box\"]/input"));
                            driver.FindElement(By.Id("overlay-shade"));
                            list = driver.FindElement(By.Id("captcha_box")).FindElements(By.XPath("//img"));
                            num6 = Convert.ToInt32(driver.FindElementById("total_clicked").Text) - num3;
                            this.totalClicked = totalClicked + num6;
                            Thread.Sleep(2000);

                            for (int k = num5; k < list.Count; k++)
                            {
                                //if (list[k].GetAttribute("id") == "0")
                                if(int.TryParse(list[k].GetAttribute("id"), out num4))
                                {
                                    num4 = k;
                                    break;
                                }
                            }
                            flag = true;
                        }
                        catch (Exception e)
                        {
                            this._clickedToday = "Exception!";
                            if (!driver.Url.Contains("yar"))
                            {
                                Toolbox.Login(account, driver);
                            }
                            driver.Navigate().GoToUrl(CLICKER_URL);
                            flag = false;
                        }
                        if (flag)
                        {
                            for (int m = 0; m < num; m++)
                            {
                                try
                                {
                                    attribute = list[num4].GetAttribute("src");
                                    if (Toolbox.ElementIsVisible(By.Id("overlay-shade"), driver))
                                    {
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                                for (int n = 0; n < this.keys.Length; n++)
                                {
                                    if(attribute.Length == keys[n])
                                    {
                                        Thread.Sleep(millisecondsTimeout);
                                        element.SendKeys(this.letters[n]);
                                        num2++;
                                        break;
                                    }
                                }
                                num4++;
                            }
                            if (this.sendToMatt && (num2 > (600 + this.rand.Next(100))))
                            {
                                num2 = 0;
                                this.SendToMatt();
                            }
                        }

                        for (int j = 0; j < 15; j++)
                        {
                            if (!Toolbox.ElementExists(By.Id("98"), driver))
                            {
                                flag2 = true;
                                break;
                            }
                            if (Toolbox.ElementIsVisible(By.Id("overlay-shade"), driver))
                            {
                                break;
                            }
                            if (!flag2 && (j == 14))
                            {
                                millisecondsTimeout += 5;
                                if (millisecondsTimeout > 30)
                                {
                                    flag2 = true;
                                }
                            }
                            else if (flag2 && (j == 14))
                            {
                                millisecondsTimeout++;
                            }
                            Thread.Sleep(250);
                        }
                        //driver.Navigate().Refresh();
                        //this.getCaptcha();
                    }
                    if (this.sendToMatt)
                    {
                        this.SendToMatt();
                    }
                    this.totalClicked = totalClicked + num6;
                }
            }
        }

        private void SendToMatt()
        {
            if (!this.sendID.Equals(""))
            {
                driver.Navigate().GoToUrl(MORALE_URL);
                try
                {
                    driver.FindElement(By.Name("transfer_recipient")).SendKeys(this.sendID);
                    driver.FindElement(By.XPath("//*[@id=\"morale_transfer\"]/tbody/tr[3]/td[2]/input[2]")).Click();
                }
                catch (NoSuchElementException)
                {
                    Toolbox.Login(account, driver);
                    driver.Navigate().GoToUrl(MORALE_URL);
                    driver.FindElement(By.Name("transfer_recipient")).SendKeys(this.sendID);
                    driver.FindElement(By.XPath("//*[@id=\"morale_transfer\"]/tbody/tr[3]/td[2]/input[2]")).Click();
                }
                string text = driver.FindElement(By.XPath("//*[@id=\"player_soldiers\"]/tbody/tr[8]/td[2]")).Text.Replace(",", "");
                driver.FindElement(By.Name("transfer_amount")).SendKeys(text);
                driver.FindElement(By.XPath("//*[@id=\"morale_transfer\"]/tbody/tr[4]/td/input[2]")).Submit();
                Thread.Sleep(150);
                driver.Navigate().GoToUrl(CLICKER_URL);
                this.getCaptcha();
            }
        }

        private bool checkDone(string clickedToday = "")
        {
            int result = 0;
            try
            {
                if (int.TryParse(driver.FindElement(By.Id("clicks_left")).Text, out result) && (result <= 1))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            if (!driver.FindElementByXPath("//*[@id=\"overlay-inAbox\"]/div[2]").Text.Contains("You are out") && (clickedToday.Equals("") || this.haveClicks(clickedToday)))
            {
                return false;
            }
            return true;
        }

        private bool haveClicks(string clickedtoday)
        {
            int num = 0;
            for (int i = 0; i < clickedtoday.Length; i++)
            {
                char ch = clickedtoday[i];
                if (ch.Equals('/'))
                {
                    num = i;
                }
            }
            if (num > 0)
            {
                try
                {
                    return ((Convert.ToInt32(clickedtoday.Substring(num + 1, (clickedtoday.Length - num) - 1)) - 2) > Convert.ToInt32(clickedtoday.Substring(0, num - 1)));
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        private void finishedTimer()
        {
            justFinished = true;
            Thread.Sleep(30 * 60000);
            justFinished = false;
        }


    }
}

