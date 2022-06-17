namespace AWbuy.AWbuy.Res
{
    using System.IO;
    using OpenQA.Selenium;
    using OpenQA.Selenium.PhantomJS;
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Net.NetworkInformation;
    using System.Drawing;
    using System.Drawing.Imaging;
    internal class Buyer
    {
        private const string ARMORY_URL = "http://www.aw.net/index.php/armory";
        private const string BANK_URL = "http://www.aw.net/index.php/bank";
        private const string LOGIN_URL = "http://www.aw.net/index.php/index";
        private const string BASE_URL = "http://www.aw.net/index.php/base";
        private const string TRAINING_URL = "http://aw.net/index.php/training";

        private bool firstRun;
        private bool bank;

        private float timeMultiplier;

        private short bankPct;
        private short buyPct;

        private string logName;

        private int higherTime;
        private int lowerTime;
        private bool running;
        private long startTime;
        private Random rand;

        private PhantomJSDriver driver;
        private PhantomJSDriverService driverService;
        private string phantomLoc = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private AWTools Toolbox;
        private AWTools.Account account;

        public AWEventManager eventManager = new AWEventManager();

        public stats accountStats;

        public string bankAmt;
        public long banked;
        public bool CurrentGoing;
        public bool fullBank;
        public bool loginError;
        public long purchased;


        public struct stats
        {
            public string strikeAction;
            public string defensiveAction;
            public string spyRating;
            public string sentryRating;
            private int[] WeaponCount;
            private int[] SoldierCount;

            /// <summary>
            /// Gets the number of soldiers that need to be trained to match weapons
            /// </summary>
            /// <returns>An array of 4 ints, SA, DA, Spy, Sentry soldiers needed to be trained</returns>
            public int [] getSoldierDeficit()
            {
                int[] deficit = new int[4];
                for (int i = 0; i < 4; i++)
                    deficit[i] = WeaponCount[i] - SoldierCount[i];

                return deficit;
            }
            /// <summary>
            /// Sets the number of soldiers that the account has;
            /// </summary>
            /// <param name="soldiers">A 4 element array of soldier counts: SA, DA, Spy, Sentry</param>
            public void setSoldiers(int[] soldiers)
            {
                if(soldiers.Length == 4)
                    SoldierCount = soldiers;
            }
            /// <summary>
            /// Sets the number of weapons that the account has;
            /// </summary>
            /// <param name="soldiers">A 4 element array of weapon counts: SA, DA, Spy, Sentry</param>
            public void setWeapons(int[] weapons)
            {
                if (weapons.Length == 4)
                    WeaponCount = weapons;
            }
        }
        
        public Buyer()
        {
            firstRun = true;
            buySoldiers = false;
            startTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            timeMultiplier = 1.0f;
            this.idling = false;
            this.doTransactions = true;
            this.rand = new Random();
            this.resetAll();
            this.running = false;
            this.bankAmt = "-";

            matchSoldiers = false;
            eventList = null;
            logging = true;
            SetLogName();
            logMessage("AWBuy Starting\n");


            defaultStats();

            this.driverService = PhantomJSDriverService.CreateDefaultService(this.phantomLoc);
            this.driverService.HideCommandPromptWindow = true;

            Toolbox = new AWTools();
            clicksAvail = false;
        }

        // Primary AW Functions

        public void Run()
        {
            CurrentGoing = true;

            EndIdle();

            if (firstRun)
            {
                FirstRun();
            }
            HandleEvents();
            timeToBuy = Environment.TickCount + (int)((lowerTime + rand.Next(higherTime)) / timeMultiplier);
            logMessage("Timer reset to: " + TimeTillPurchase() + ", with multiplier of " + timeMultiplier.ToString());
            Buy();
        }

        private void Buy()
        {
            if (this.doTransactions)
            {
                
                if (driver == null || Toolbox.isDriverDead(driver))
                {
                    driverService.HideCommandPromptWindow = true;
                    driver = new PhantomJSDriver(driverService);
                }
                try
                {
                    if (!checkLogin() && !this.login())
                    {
                        this.loginError = true;
                        this.running = false;
                    }
                    else
                    {
                        this.loginError = false;
                    }
                    logMessage("Buying...");
                    checkStats();
                    checkBankStatus();

                    if (running)
                    {
                        BuyMatchingSoldiers();
                        BuyAttackSoldiers();

                        // Optional to move it before or after buying soldiers
                        if (taxFree && eventList.Contains("Tax free") && !fullBank)
                        {
                            depositBank();
                        }

                        if (bank && !fullBank)
                        {
                            depositBank();
                        }
                        if (!bank || (fullBank || (buyPct > 0)))
                        {
                            buyWeapons();
                        }
                    }


                    this.CurrentGoing = false;
                }
                catch (Exception e)
                {
                    this.CurrentGoing = false;
                    Toolbox.logError(e);
                    return;
                }
            }

            Idle();

        }

        private void Idle()
        {
            idling = true;
            doIdle = true;
            Toolbox.idling = true;
            logMessage("Idling");
            int waitTime = 0;
            checkStats();
            GetEvents();
            while (idling)
            {
                waitTime = 300000 + rand.Next(1000000);

                checkClicks();

                if (studyHall)
                {
                    TradeTurns();
                }

                for (int i = 0; i < waitTime; i += 500)
                {
                    if (!idling)
                    {
                        doIdle = false;
                        return;
                    }
                    Thread.Sleep(500);
                }
                if (!checkLogin())
                    Toolbox.Login(account, driver);

                waitTime = rand.Next(3);
                if (waitTime == 0)
                    driver.Navigate().GoToUrl(BASE_URL);
                if (waitTime == 1)
                    checkStats();
                else
                    driver.Navigate().GoToUrl(BANK_URL);

                BuyMatchingSoldiers();


            }
            doIdle = false;
            Toolbox.idling = false;
        }

        private void FirstRun()
        {
            firstRun = false;

            if (manageEvents)
            {
                InitEventCheck();
            }
        }

        // -----------------------

        // Gold Spending Functions

        private void buyWeapons()
        {
            long num = 0L;
            if (this.fullBank && this.bank)
            {
                this.checkBankStatus();
            }

            this.driver.Navigate().GoToUrl(ARMORY_URL);
            Thread.Sleep(500);

            for (int i = 1; i <= 5; i++)
            {
                num += Convert.ToInt32(this.driver.FindElement(By.Name("buy_weapon[sa-" + Convert.ToString(i) + "]")).GetAttribute("value"));
                num += Convert.ToInt32(this.driver.FindElement(By.Name("buy_weapon[da-" + Convert.ToString(i) + "]")).GetAttribute("value"));
                num += Convert.ToInt32(this.driver.FindElement(By.Name("buy_weapon[spy-" + Convert.ToString(i) + "]")).GetAttribute("value"));
                num += Convert.ToInt32(this.driver.FindElement(By.Name("buy_weapon[sentry-" + Convert.ToString(i) + "]")).GetAttribute("value"));
            }
            this.purchased += num;
            Thread.Sleep(500);
            this.driver.FindElement(By.Id("game_submitButton")).Submit();
        }

        private void depositBank()
        {
            this.driver.Navigate().GoToUrl(BANK_URL);
            Thread.Sleep(500);

            Int64 num = Convert.ToInt64(this.driver.FindElement(By.XPath("//*[@id=\"deposit_table\"]/tbody/tr[2]/td[3]/input")).GetAttribute("value").Replace(",", ""));
            if (!taxFree)
            {
                num = (num * this.bankPct) / 100L;
            }

            long num2 = Convert.ToInt64(this.driver.FindElement(By.Id("gold_bank_amount")).Text.Replace(",", ""));

            if ((num2 + num) > this.BankGoal)
            {
                num = ((this.BankGoal - num2) * 20L) / 0x11L;
            }

            this.banked += num;
            this.driver.FindElement(By.XPath("//*[@id=\"deposit_table\"]/tbody/tr[2]/td[2]/input")).SendKeys(Convert.ToString(num));

            Thread.Sleep(700);

            this.driver.FindElement(By.XPath("//*[@id=\"deposit_table\"]/tbody/tr[4]/td/input[2]")).Click();
            this.checkBankStatus();
        }

        private void BuyMatchingSoldiers()
        {
            if (matchSoldiers)
            {
                try
                {
                    if (!this.checkLogin())
                    {
                        this.login();
                    }

                    driver.FindElement(By.CssSelector("div#training.menu")).Click();
                    Thread.Sleep(200);

                    int[] purchaseOrder = GetSoldierPurchase(accountStats, GetCurrentGold(), UnformatInt(driver.FindElement(By.Id("untrained")).Text));
                    string[] amtBoxIndex = { "1", "2", "4", "5" };

                    int k = 0;
                    for(int i = 0; i < purchaseOrder.Length; i++)
                    {
                        k += purchaseOrder[i];
                    }

                    if(k == 0)
                    {
                        return;
                    }

                    var table = driver.FindElement(By.Id("game_soldiers"));

                    for (int i = 0; i < purchaseOrder.Length; i++)
                    {
                        IWebElement ele = table.FindElement(By.Name("buy_soldiers[" + amtBoxIndex[i] + "]"));
                        ele.Clear();
                        ele.SendKeys(purchaseOrder[i].ToString());
                    }

                    table.FindElement(By.Name("submit")).Click();

                    checkStats();
                }
                catch (Exception e) { Toolbox.logError(e); }
            }
        }

        private void BuyAttackSoldiers()
        {

            if (buySoldiers)
            {
                if (!this.checkLogin())
                {
                    this.login();
                }

                driver.FindElement(By.CssSelector("div#training.menu")).Click();
                Thread.Sleep(200);

                driver.FindElement(By.CssSelector("input.buy_helpers")).Click();
                Thread.Sleep(500);
                driver.FindElement(By.XPath("//*[@id=\"game_soldiers\"]/tbody/tr[9]/td/input[2]")).Click();
            }
        }

        private void TradeTurns()
        {
            if(!checkLogin())
            {
                login();
            }
            driver.Navigate().GoToUrl(TRAINING_URL);
            Thread.Sleep(200);

            int numTurns = GetOptimalTurns();

            driver.FindElement(By.XPath("//*[@id=\"content\"]/tbody/tr[7]/td[2]/table/tbody/tr[4]/td/form/table/tbody/tr[3]/td[2]/input")).SendKeys(numTurns.ToString());
            Thread.Sleep(50);
            driver.FindElement(By.Id("submitButton")).Click();
        }
        // -----------------------

        // Event Management Functions and Variables

        private bool manageEvents { get; set; }

        private string eventList;

        private bool taxFree;
        private bool studyHall;

        public void eventsToManage(string events)
        {
            eventList = events;
            if(events.Length > 0)
            {
                manageEvents = true;
            }
            else
            {
                manageEvents = false;
            }
            if(!running)
            {
                logging = false;
            }
            HandleEvents();
            if (!running)
            {
                logging = true;
            }
        }

        /// <summary>
        /// Logs in then checks events. Should be run at startup.
        /// </summary>
        private void InitEventCheck()
        {
            if (driver == null || Toolbox.isDriverDead(driver))
            {
                driverService.HideCommandPromptWindow = true;
                driver = new PhantomJSDriver(driverService);
            }
            login();

            GetEvents();
        }

        /// <summary>
        /// Finds the list of events and inserts them into the eventManager
        /// </summary>
        private void GetEvents()
        {
            if(!manageEvents || Toolbox.isDriverDead(driver))
            {
                return;
            }

            logMessage("Getting events..");
            try
            {
                driver.Navigate().GoToUrl(BASE_URL);
                Thread.Sleep(50);
                var table = driver.FindElement(By.Id("events")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));

                foreach (var ele in table)
                {
                    CreateEvent(ele);
                }
            }
            catch(Exception e) { Toolbox.logError(e); }
        }

        private void CreateEvent(IWebElement ele)
        {
            var cols = ele.FindElements(By.TagName("td"));
            IWebElement[] colArr = new IWebElement[cols.Count];
            cols.CopyTo(colArr, 0);

            string k = colArr[2].FindElement(By.TagName("span")).GetAttribute("timestamp");
            Int64 startT = Int64.Parse(k);

            eventManager.AddEvent(colArr[1].Text, startT, startT + GetEventLength(colArr[1].Text));
        }
        private int GetEventLength(string eventName)
        {
            if(eventName.Equals("Jack Rabbit") || eventName.Equals("Tax free"))
            {
                return 3600 * 24;
            }
            return 3600;
        }

        /// <summary>
        /// Cycles through the events contained in eventManager and enables the respective options.
        /// </summary>
        private void HandleEvents()
        {
            if(!manageEvents)
            {
                return;
            }
            AWEventManager.awEvent[] curEvents = eventManager.GetCurrentEvents();

            timeMultiplier = 1.0f;
            taxFree = false;
            studyHall = false;

            if (curEvents == null || !manageEvents)
            {
                logMessage("No current events.");
                return;
            }

            string message = "Current events: ";
            foreach(var evnt in curEvents)
            {
                string eventName = evnt.GetName();
                if (eventList.Contains(eventName))
                {
                    message += eventName + " ";
                    if(eventName.Equals("Gold Rush"))
                    {
                        if (eventList.Contains("Gold Rush"))
                        {
                            timeMultiplier *= (float)(2 + rand.NextDouble() * 2);
                        }
                    }
                    else if (eventName.Equals("What sentry?"))
                    {
                        if (eventList.Contains("What sentry?"))
                        {
                            
                            timeMultiplier *= (float)(1 + rand.NextDouble() * 1.5);
                        }
                    }
                    else if (eventName.Equals("Tax free"))
                    {
                        if (eventList.Contains("Tax free"))
                        {
                            taxFree = true;
                        }
                    }
                    else if (eventName.Equals("Study Hall"))
                    {
                        if (eventList.Contains("Study Hall"))
                        {
                            studyHall = true;
                        }
                    }
                }
            }
            if(message.Equals("Current events: "))
            {
                message += "none";
            }
            logMessage(message);
        }
        // -------------------------


        // Quick AW functions

        private bool login()
        {
            this.driver.Navigate().GoToUrl(LOGIN_URL);
            this.driver.FindElement(By.Name("email")).SendKeys(account.email);
            this.driver.FindElement(By.Name("password")).SendKeys(account.password);
            this.driver.FindElement(By.Name("submit")).Submit();
            if (!this.checkLogin())
            {
                return false;
            }
            return true;
        }

        private bool checkLogin()
        {
            string url = this.driver.Url;
            this.driver.Navigate().GoToUrl(ARMORY_URL);
            if (!this.driver.Url.Contains("armory"))
            {
                return false;
            }
            this.driver.Navigate().GoToUrl(url);
            return true;
        }

        private bool checkLogin(string cUrl)
        {
            this.driver.Navigate().GoToUrl(ARMORY_URL);
            if (!this.driver.Url.Contains("armory"))
            {
                return false;
            }
            this.driver.Navigate().GoToUrl(cUrl);
            return true;
        }
        
        private void checkBankStatus()
        {
            ThreadStart start = null;
            if (!this.checkLogin(BANK_URL))
            {
                this.login();
            }
            this.driver.Navigate().GoToUrl(BANK_URL);
            string text = this.driver.FindElement(By.Id("gold_bank_amount")).Text;
            this.bankAmt = text;
            if (this.bank && (Convert.ToInt64(text.Replace(",", "")) >= this.BankGoal))
            {
                if (!this.fullBank)
                {
                    if (start == null)
                    {
                        start = () => MessageBox.Show("Your bank goal of " + this.formatInt(this.BankGoal) + " gold was Reached!\nSwitching to weapon purchasing!", "Bank Goal Reached!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    new Thread(start).Start();
                }
                this.fullBank = true;
            }
            else
            {
                this.fullBank = false;
            }
        }

        private void checkClicks()
        {
            if (Toolbox.HaveClicks(driver, account))
            {
                clicksAvail = true;
                logMessage("Checking clicks: some are available!");
            }
            else
            {
                clicksAvail = false;
            }
        }

        private void checkMessages()
        {

        }

        private void checkStats()
        {
            if(!driver.Url.Contains("armory"))
            {
                driver.Navigate().GoToUrl(ARMORY_URL);
            }

            GetUserStats();
            GetWeaponStats();
            GetSoldierStats();

        }
        private void GetUserStats()
        {
            if (!driver.Url.Contains("armory"))
            {
                return;
            }
           // Elements 2 through 5 are the Userstats sa, da, spy, sentry
            var table = driver.FindElementById("player_stats").FindElements(By.TagName("tr"));
            
            foreach (var ele in table)
            {
                if (ele.Text.Contains("Strike"))
                {
                    accountStats.strikeAction = ele.FindElement(By.XPath("td[2]")).Text;
                }
                else if (ele.Text.Contains("Def"))
                {
                    accountStats.defensiveAction = ele.FindElement(By.XPath("td[2]")).Text;
                }
                else if (ele.Text.Contains("Spy"))
                {
                    accountStats.spyRating = ele.FindElement(By.XPath("td[2]")).Text;
                }
                else if (ele.Text.Contains("Sentry"))
                {
                    accountStats.sentryRating = ele.FindElement(By.XPath("td[2]")).Text;
                }
            }
        }
        private void GetWeaponStats()
        {
            if (!driver.Url.Contains("armory"))
            {
                return;
            }

            int[] wepCount = new int[4];
            string[] ids = { "player_weapons_sa", "player_weapons_da", "player_weapons_spy", "player_weapons_sentry" };

            for (int i = 0; i < ids.Length; i++)
            {
                wepCount[i] = 0;
                try
                {
                    var table = driver.FindElementById(ids[i]).FindElements(By.TagName("tr"));

                    foreach (var ele in table)
                    {
                        if (ele.Text.Contains("Stat Value"))
                        {
                            wepCount[i] += CleanUpWeaponCount(ele.FindElement(By.XPath("td[3]")).Text);
                        }
                    }
                }
                catch (Exception e) { wepCount[i] = 0; Toolbox.logError(e); }
            }
            accountStats.setWeapons(wepCount);
        }
        private int CleanUpWeaponCount(string WeaponCount)
        {
            int a = WeaponCount.Length;

            WeaponCount = WeaponCount.Substring(0, WeaponCount.IndexOf('\r')).Replace(",", "");

            if(int.TryParse(WeaponCount, out a))
            {
                return a;
            }
            return 0;

        }
        private void GetSoldierStats()
        {
            if(!driver.Url.Contains("armory") && !driver.Url.Contains("training"))
            {
                return;
            }
            int[] soldiers = new int[4];
            var table = driver.FindElementById("player_soldiers").FindElements(By.TagName("tr"));
            IWebElement[] tableArray = new IWebElement[table.Count];

            table.CopyTo(tableArray, 0);

            soldiers[0] = UnformatInt(tableArray[2].FindElement(By.XPath("td[2]")).Text);
            soldiers[1] = UnformatInt(tableArray[3].FindElement(By.XPath("td[2]")).Text);
            soldiers[2] = UnformatInt(tableArray[5].FindElement(By.XPath("td[2]")).Text);
            soldiers[3] = UnformatInt(tableArray[6].FindElement(By.XPath("td[2]")).Text);

            accountStats.setSoldiers(soldiers);
        }

        private int [] GetSoldierPurchase(stats accountStats, Int64 gold, int untrainedSoldiers)
        {
            int[] purchase = new int[4];
            int[] deficit = accountStats.getSoldierDeficit();
            int[] priority = { 3, 1, 2, 0 }; // sentry, defense, spy, attack
            int[] costs = { 2000, 2000, 3500, 3500 };


            for(int i = 0; i < 4; i++)
            {
                int k = priority[i];
                if (deficit[k] > 0 && untrainedSoldiers > 0 && gold > costs[k])
                {
                    int count = deficit[k];
                    if (untrainedSoldiers < count)
                    {
                        count = untrainedSoldiers;
                    }
                    if (count * costs[k] > gold)
                    {
                        count = int.Parse((gold / costs[k]).ToString());
                    }
                    untrainedSoldiers -= count;
                    gold -= count * costs[k];

                    purchase[k] = count;
                }
                else
                {
                    purchase[k] = 0;
                }
            }

            return purchase;

        }

        private Int64 GetCurrentGold()
        {
            if(!checkLogin())
            {
                login();
            }

            return (UnformatInt64(driver.FindElement(By.Id("gold")).Text));
        }

        /// <summary>
        /// Calculates the optimal number of turns to trade. Prevents overspending
        /// </summary>
        /// <returns>1</returns>
        private int GetOptimalTurns()
        {
            return 1;
        }

        // -----------------------

        // Properties and simple value returning functions

        public long GetBanked() => this.banked;

        public long GetPurchased() =>this.purchased;

        public bool isRunning() => this.running;

        public bool logging { get; set; }

        public long BankGoal { get; set; }

        public bool doTransactions { get; set; }

        public int timeToBuy { get; set; }

        public bool buySoldiers { get; set; }

        public bool idling { get; private set; }

        public bool doIdle { get; private set; }

        public bool clicksAvail { get; private set; }

        public bool monitorMessages { get; set; }

        public bool matchSoldiers { get; set; }

        public int captchaAttempts { get { return Toolbox.captchaAttempts; } }

        public int captchaSuccess { get { return Toolbox.captchaSuccess; } }


        // -----------------------

        // Other Functions

        public void Start()
        {
            this.CurrentGoing = false;
            if (!account.email.Contains("@") || !account.email.Contains("."))
            {
                this.running = false;
                logMessage("Error: Invalid email address.");
                MessageBox.Show("Error: Invalid email address.", "Error");
            }
            else
            {
                logMessage("Successfully started buyer.");
                this.running = true;
            }
        }

        public void Stop()
        {
            this.timeToBuy = 0;
            this.running = false;
            this.idling = false;
            firstRun = true;
            Toolbox.idling = false;
            try
            {
                this.driver.Quit();
            }
            catch (Exception e)
            {
                Toolbox.logError(e);
            }
        }

        public string formatInt(long yourInt)
        {
            string str = Convert.ToString(yourInt);
            string str2 = "";
            for (int i = 0; i < str.Length; i++)
            {
                str2 = str2 + str[i];
                if (((((str.Length - i) - 1) % 3) == 0) && (((str.Length - i) - 1) != 0))
                {
                    str2 = str2 + ',';
                }
            }
            return str2;
        }

        public void resetAll()
        {
            this.purchased = 0L;
            this.timeToBuy = 0;
            this.buyPct = 0;
            this.bankPct = 100;
            account.email = "";
            account.password = "";
            this.loginError = false;
            this.fullBank = false;
            this.bank = false;
        }

        public void setDetails(string email, string password, string start, string end)
        {
            account.email = email;
            account.password = password;
            this.lowerTime = Convert.ToInt32(start) * 0xea60;
            this.higherTime = (Convert.ToInt32(end) * 0xea60) - this.lowerTime;
        }

        public void setPercents(short buyPercent, short bankPercent)
        {
            this.bankPct = bankPercent;
            this.buyPct = buyPercent;
            if (this.bankPct == 0)
            {
                this.bank = false;
            }
            else
            {
                this.bank = true;
            }
        }

        public bool killDriver()
        {
            try
            {
                this.driver.Quit();
                return true;
            }
            catch (Exception e)
            {
                Toolbox.logError(e);
                return false;
            }
        }

        public string TimeTillPurchase()
        {
            int num = (this.timeToBuy - Environment.TickCount) / 0x3e8;
            string str = "00";
            string str2 = "";
            string str3 = "";
            if (num <= 0)
            {
                return "00:00:00";
            }
            if (num >= 0xe10)
            {
                str = "0" + Convert.ToString((int)(num / 0xe10));
                num -= (num / 0xe10) * 0xe10;
            }
            if (num > 60)
            {
                if (num < 600)
                {
                    str2 = "0";
                }
                str2 = str2 + Convert.ToString((int)(num / 60));
                num -= (num / 60) * 60;
            }
            else
            {
                str2 = "00";
            }
            if (num < 10)
            {
                str3 = str3 + "0";
            }
            str3 = str3 + Convert.ToString(num);
            return (str + ":" + str2 + ":" + str3);
        }

        /// <summary>
        /// Stops the Idle function safely.
        /// </summary>
        private void EndIdle()
        {
            if (idling)
            {
                idling = false;
                Toolbox.idling = false;
            }

            while (doIdle)
            {
                Thread.Sleep(50);
            }
        }

        private void defaultStats()
        {
            stats newAcc = new stats();
            newAcc.strikeAction = "";
            newAcc.defensiveAction = "";
            newAcc.spyRating = "";
            newAcc.sentryRating = "";
            accountStats = newAcc;
        }

        /// <summary>
        /// Removes commas and spaces from numbers.
        /// </summary>
        /// <param name="yourString">Number formatted as " ###,##,## " or some similar variation</param>
        /// <returns>An int64 #######</returns>
        private Int64 UnformatInt64(string yourString)
        {
            Int64 intOut;

            yourString = yourString.Replace(",", "").Replace(" ", "");
            if(Int64.TryParse(yourString, out intOut))
            {
                return intOut;
            }
            return -1;
        }

        /// <summary>
        /// Removes commas and spaces from numbers.
        /// </summary>
        /// <param name="yourString">Number formatted as " ###,##,## " or some similar variation</param>
        /// <returns>An int #######</returns>
        private int UnformatInt(string yourString)
        {
            int intOut;

            yourString = yourString.Replace(",", "").Replace(" ", "");
            if (int.TryParse(yourString, out intOut))
            {
                return intOut;
            }
            return -1;
        }

        private bool logMessage(string message)
        {
            if(!logging)
            {
                return true;
            }

            if (!Directory.Exists("buyerlogs"))
            {
                Directory.CreateDirectory("buyerLogs");
            }

            try
            {
                message = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: ") + message;
                using (StreamWriter sw = File.AppendText("buyerlogs/" + logName + ".log")) 
                    sw.WriteLine(message);
            }
            catch (Exception e) { Toolbox.logError(e); return false; }
            return true;
        }

        private void SetLogName()
        {
            logName = DateTime.Now.ToString("LOGyyyy-MM-dd+HH-mm");
        }
        // -----------------------

    }
}

