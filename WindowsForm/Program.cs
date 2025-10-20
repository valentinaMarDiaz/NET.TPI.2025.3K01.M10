using System;
using System.Windows.Forms;
using DTOs;

namespace WindowsForm
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new AppController());
        }
    }

    public class AppController : ApplicationContext
    {
        public AppController()
        {
            ShowInicio();
        }

        private void ShowInicio()
        {
            var f = new FormInicio();
            f.FormClosed += (_, __) =>
            {
                if (f.WasCancelled)
                {
                    ExitThread(); // cierra la app
                }
                else if (f.AuthUser != null)
                {
                    ShowMenu(f.AuthUser);
                }
                f.Dispose();
            };
            f.Show();
        }

        private void ShowMenu(LoginResponseDTO user)
        {
            var m = new FormMenu(user);
            m.FormClosed += (_, __) =>
            {
                if (m.LogoutRequested || m.DialogResult == DialogResult.None)
                    ShowInicio();
                m.Dispose();
            };
            m.Show();
        }
    }
}
