namespace AWbuy
{    
    using System;
    using System.Windows.Forms;

    internal static class AWBuyer
    {

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm mainForm = new mainForm();
            Application.Run(mainForm);
        }

        static void OnPressExit(object sender, EventArgs e)
        {

       }
    }
}

