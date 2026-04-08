namespace BarcodeBartenderApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        ///  Always shows the Login form first — no auto-login.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            DatabaseHelper.Initialize();

            // Show login form on startup — user must authenticate
            var login = new LoginForm();
            if (login.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && !string.IsNullOrEmpty(login.LoggedUser))
            {
                System.Windows.Forms.Application.Run(new Form1(login.LoggedUser));
            }
        }
    }
}
