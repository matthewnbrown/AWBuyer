using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Chrome;
using System.Net.NetworkInformation;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using tessnet2;


namespace AWbuy.AWbuy.Res
{
    class AWTools
    {
        //private Ping pingSender;

        public bool solvingCaptcha { get; set; }
        public int captchaAttempts { get; set; }
        public int captchaSuccess { get; set; }
        public bool idling { get; set; }

        private const string AWDOMAIN = "aw.net";
        private const string AWHOME = "http://www.aw.net/index.php/index";
        private const string AWARM = "http://www.aw.net/index.php/armory";
        private const string AWCLICK = "http://aw.net/index.php/yar";
        public struct Account
        {
            public string email;
            public string password;
        }

        public AWTools()
        {
            idling = true;
        }


        public bool Login(Account account, PhantomJSDriver driver)
        {
            try
            {
                driver.Navigate().GoToUrl(AWHOME);
                driver.FindElement(By.Name("email")).SendKeys(account.email);
                driver.FindElement(By.Name("password")).SendKeys(account.password);
                driver.FindElement(By.Name("submit")).Submit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Pass current phantomjsdriver
        /// </summary>
        /// <param name="driver"></param>
        /// <returns>Whether or not driver is closed</returns>
        public bool isDriverDead(PhantomJSDriver driver)
        {
            try
            {
                if (driver.SessionId == null)
                {
                    return true;
                }
            }
            catch (NullReferenceException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pings the AW website
        /// </summary>
        /// <returns></returns>
        public bool checkConnection()
        {
            try
            {
                using (Ping pingSender = new Ping())
                { 
                    return (pingSender.Send(AWDOMAIN).Status == IPStatus.Success);
                }
            }
            catch (Exception exception)
            {
                this.logError(exception);
                return false;

            }
        }

        /// <summary>
        /// Solves the captcha on the YAR clicker page
        /// Captcha only solved if running is true
        /// </summary>
        /// <param name="driver">main driver</param>
        /// <param name="running">is the program still running?</param>
        /// <param name="solvingCaptcha">solving status</param>
        /// <param name="_clickedToday"></param>
        public void getCaptcha(ref PhantomJSDriver driver, ref bool running, ref string _clickedToday, Account account)
        {
            if (!Directory.Exists("captchas"))
            {
                Directory.CreateDirectory("captchas");
            }

            IWebElement element;
            solvingCaptcha = true;

        Label_0007:
            if(!idling)
            {
                return;
            }
            try
            {
                element = driver.FindElement(By.Id("captcha_code"));
                if (driver.FindElements(By.Id("captcha")).Count == 0)
                {
                    goto Label_0007;
                }
            }
            catch (Exception)
            {

                if(!checkLogin(account, driver))
                {
                    Login(account, driver);
                }
                driver.Navigate().GoToUrl(AWCLICK);
                Thread.Sleep(2000);
                goto Label_0007;
            }
            string str = _clickedToday;
            _clickedToday = "";

        Label_005E:
            if (running && this.checkConnection() && idling)
            {
                captchaAttempts++;
                Thread.Sleep(500);
                element = driver.FindElement(By.Id("captcha_code"));
                try
                {
                    using (Tesseract ocr = new Tesseract())
                    {
                        ocr.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz");
                        ocr.Init(@"ocr/tessdata", "eng", false);

                        Thread.Sleep(1500);

                        // Box to type
                        driver.GetScreenshot().SaveAsFile("captchaPic.png", ImageFormat.Png);

                        IWebElement CaptchaPic = driver.FindElement(By.Id("captcha_holder"));
                        // IWebElement CaptchaOverlay = this.driver.FindElement(By.Id("overlay-captcha"));


                        Thread.Sleep(50);
                        Bitmap img = (Bitmap)Image.FromFile("captchaPic.png");

                        Point pt = CaptchaPic.Location;
                        int width = CaptchaPic.Size.Width;
                        int height = CaptchaPic.Size.Height;


                        using (Bitmap bitmap2 = MakeGrayscale(this.cropImage(img, new Rectangle(new Point(0x76, 370), new Size(160, 50)))))
                        {
                            string text = "";
                            var epoch = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                            try
                            {
                                string filename = "captcha.bmp";
                                bitmap2.Save(filename);

                                var guess = ocr.DoOCR(bitmap2, Rectangle.Empty)[0];
                                if ((guess.Text.Length > 5) && guess.Text.Contains("vv"))
                                {
                                    guess.Text = guess.Text.Replace("vv", "w");
                                }

                                text = guess.Text;

                                _clickedToday = text;
                                element.SendKeys(text);
                                driver.Keyboard.PressKey(Keys.Enter);
                                img.Dispose();
                                bitmap2.Dispose();
                                
                                Thread.Sleep(500);
                                int num = 0;
                            
                                while (!this.ElementIsVisible(By.Id("overlay-captcha"), driver))
                                {
                                    num++;
                                    if (!idling)
                                    {
                                        return;
                                    }
                                    if ((!running || !this.checkConnection()) || this.ElementExists(By.Id("0"), driver))
                                    {
                                        _clickedToday = str;
                                        captchaSuccess++;
                                        return;
                                    }
                                    if (!driver.Url.Contains("yar"))
                                    {
                                        driver.Navigate().GoToUrl(AWCLICK);
                                        if (!driver.Url.Contains("yar"))
                                        {
                                            Login(account, driver);
                                        }
                                    }
                                    if (num == 150)
                                    {
                                        driver.Navigate().Refresh();
                                    }
                                    Thread.Sleep(10);
                                }

                                if (ElementExists(By.Id("0"), driver) || !ElementIsVisible(By.Id("overlay-inAbox"), driver))
                                {
                                    _clickedToday = str;
                                    captchaSuccess++;
                                    goto Label_02BC;
                                }
                            }
                            catch (AccessViolationException e) { logError(e); }
                        }

                        goto Label_005E;
                    }
                }
                catch (Exception e)
                {
                    
                    logError(e);
                    driver.Navigate().GoToUrl(AWCLICK);
                    goto Label_005E;
                }
            }
        Label_02BC:
            _clickedToday = "Solved the captcha!";
        }

        /// <summary>
        /// Checks if element exists on webpage
        /// See also ElementIsVisible for visibility instead of whether or not it exists.
        /// </summary>
        /// <param name="by"></param>
        /// <param name="driver"></param>
        /// <returns>True if element exists on webpage</returns>
        public bool ElementExists(By by, PhantomJSDriver driver)
        {
            try
            {
                if (driver.FindElements(by).Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// Checks if element's style contains 'none'
        /// See also ElementExists
        /// </summary>
        /// <param name="by"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public bool ElementIsVisible(By by, PhantomJSDriver driver)
        {
            try
            {
                if (driver.FindElement(by).GetAttribute("style").Contains("none"))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool HaveClicks(PhantomJSDriver driver, Account account)
        {
            bool garbage = true;
            string alsoGarbage = "";
            int result = -55;

            driver.Navigate().GoToUrl(AWCLICK);

            getCaptcha(ref driver, ref garbage, ref alsoGarbage,  account);

            Thread.Sleep(500);

            try
            {
                if (int.TryParse(driver.FindElement(By.Id("clicks_left")).Text, out result) && (result >= 50))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        // logs error in file given exception
        public void logError(Exception e)
        {

            string str = DateTime.Now.ToString("h:mm:ss tt");
            try
            {
                if (!Directory.Exists("errorlogs"))
                {
                    Directory.CreateDirectory("errorlogs");
                }
                var epoch = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                string[] contents = new string[5];
                contents[0] = contents[4] = "----------------------------------------";
                contents[1] = "\n\nError Occurred At: " + str;
                contents[2] = e.Message + '\n';
                contents[3] = e.Source + '\n';
                contents[4] = e.StackTrace + '\n';
                File.WriteAllLines(new StringBuilder().Append("errorlogs/").Append(contents[3].Replace('.','-').Replace("\n", "")).Append("-").Append(epoch).Append(".txt").ToString(), contents);
            }
            catch (Exception)
            {
            }
        }

        // For getting captcha
        private Bitmap cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bitmap = new Bitmap(img);
            return bitmap.Clone(cropArea, bitmap.PixelFormat);
        }
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }
        private static Bitmap MakeGrayscale(Bitmap original)
        {
            Bitmap image = new Bitmap(original.Width, original.Height);
            Graphics graphics = Graphics.FromImage(image);

            float[][] newColorMatrix = new float[5][];
            newColorMatrix[0] = new float[] { 0.3f, 0.3f, 0.3f, 0f, 0f };
            newColorMatrix[1] = new float[] { 0.59f, 0.59f, 0.59f, 0f, 0f };
            newColorMatrix[2] = new float[] { 0.11f, 0.11f, 0.11f, 0f, 0f };
            float[] numArray2 = new float[5];
            numArray2[3] = 1f;
            newColorMatrix[3] = numArray2;
            float[] numArray3 = new float[5];
            numArray3[4] = 1f;
            newColorMatrix[4] = numArray3;
            ColorMatrix matrix = new ColorMatrix(newColorMatrix);
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(matrix);
            graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttr);
            graphics.Dispose();
            original.Dispose();
            return image;
        }

        private bool checkLogin(Account account, PhantomJSDriver driver)
        {
            string url = driver.Url;
            driver.Navigate().GoToUrl(AWARM);
            if (!driver.Url.Contains("armory"))
            {
                return false;
            }
            driver.Navigate().GoToUrl(url);
            return true;
        }

    }
}
