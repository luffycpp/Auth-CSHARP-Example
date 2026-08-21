using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ArcLicense
{
    class Program
    {

        /*
        * 
        * WATCH THIS VIDEO TO SETUP APPLICATION: https://youtube.com/watch?v=RfDTdiBq4_o
        * 
        * READ HERE TO LEARN ABOUT ARCLICENSE FUNCTIONS
        *
        */

        public static api ArcLicenseApp = new api(
            name: "Testing", // Application Name
            ownerid: "xrfNty6okO", // Account ID
            version: "1.0" // Application version
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalFindAtom(string lpString);

        static void Main(string[] args)
        {
            try
            {
                securityChecks();

                Console.Title = "Loader";
                Console.WriteLine("\n\n Connecting..");
                ArcLicenseApp.init();

                autoUpdate();

                if (!ArcLicenseApp.response.success)
                {
                    Console.WriteLine("\n Status: " + ArcLicenseApp.response.message);
                    Thread.Sleep(1500);
                    TerminateProcess(GetCurrentProcess(), 1);
                }

            Console.Write("\n [1] Login\n [2] Register\n [3] Upgrade\n [4] License key only\n [5] Forgot password\n\n Choose option: ");

            string username, password, key, email, code;

            int option = int.Parse(Console.ReadLine());
            switch (option)
            {
                case 1:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter password: ");
                    password = Console.ReadLine();
                    Console.Write("\n\n Enter 2fa code: (not using 2fa? Just press enter) ");
                    code = Console.ReadLine();
                    ArcLicenseApp.login(username, password, code);
                    break;
                case 2:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter password: ");
                    password = Console.ReadLine();
                    Console.Write("\n\n Enter license: ");
                    key = Console.ReadLine();
                    Console.Write("\n\n Enter email (just press enter if none): ");
                    email = Console.ReadLine();
                    ArcLicenseApp.register(username, password, key, email);
                    break;
                case 3:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter license: ");
                    key = Console.ReadLine();
                    ArcLicenseApp.upgrade(username, key);
                    // don't proceed to app, user hasn't authenticated yet.
                    Console.WriteLine("\n Status: " + ArcLicenseApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break;
                case 4:
                    Console.Write("\n\n Enter license: ");
                    key = Console.ReadLine();
                    Console.Write("\n\n Enter 2fa code: (not using 2fa? Just press enter");
                    code = Console.ReadLine();
                    ArcLicenseApp.license(key, code);
                    break;
                case 5:
                    Console.Write("\n\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n\n Enter email: ");
                    email = Console.ReadLine();
                    ArcLicenseApp.forgot(username, email);
                    // don't proceed to app, user hasn't authenticated yet.
                    Console.WriteLine("\n Status: " + ArcLicenseApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break;
                default:
                    Console.WriteLine("\n\n Invalid Selection");
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break; // no point in this other than to not get error from IDE
            }

            if (!ArcLicenseApp.response.success)
            {
                Console.WriteLine("\n Status: " + ArcLicenseApp.response.message);
                Thread.Sleep(2500);
                TerminateProcess(GetCurrentProcess(), 1);
            }

            Console.WriteLine("\n Logged In!"); // at this point, the client has been authenticated. Put the code you want to run after here

            if(string.IsNullOrEmpty(ArcLicenseApp.response.message)) TerminateProcess(GetCurrentProcess(), 1);

            // user data
            Console.WriteLine("\n User data:");
            Console.WriteLine(" Username: " + ArcLicenseApp.user_data.username);
            Console.WriteLine(" License: " + ArcLicenseApp.user_data.subscriptions[0].key); // this can be used if the user used a license, username, and password for register. It'll display the license assigned to the user
            Console.WriteLine(" IP address: " + ArcLicenseApp.user_data.ip);
            Console.WriteLine(" Hardware-Id: " + ArcLicenseApp.user_data.hwid);
            Console.WriteLine(" Created at: " + UnixTimeToDateTime(long.Parse(ArcLicenseApp.user_data.createdate)));
            if (!string.IsNullOrEmpty(ArcLicenseApp.user_data.lastlogin)) // don't show last login on register since there is no last login at that point
                Console.WriteLine(" Last login at: " + UnixTimeToDateTime(long.Parse(ArcLicenseApp.user_data.lastlogin)));
            Console.WriteLine(" Your subscription(s):");
            for (var i = 0; i < ArcLicenseApp.user_data.subscriptions.Count; i++)
            {
                Console.WriteLine(" Subscription name: " + ArcLicenseApp.user_data.subscriptions[i].subscription + " - Expires at: " + UnixTimeToDateTime(long.Parse(ArcLicenseApp.user_data.subscriptions[i].expiry)) + " - Time left in seconds: " + ArcLicenseApp.user_data.subscriptions[i].timeleft);
            }

            Console.Write("\n [1] Enable 2FA\n [2] Disable 2FA\n Choose option: ");
            int tfaOptions = int.Parse(Console.ReadLine());
            switch (tfaOptions)
            {
                case 1:
                    ArcLicenseApp.enable2fa();
                    break;
                case 2:
                    Console.Write("Enter your 6 digit authorization code: ");
                    code = Console.ReadLine();
                    ArcLicenseApp.disable2fa(code);
                    break;
                default:
                    Console.WriteLine("\n\n Invalid Selection");
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break; // no point in this other than to not get error from IDE
            }

            Console.WriteLine("\n Closing in five seconds...");
            Thread.Sleep(-1);
            Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n CRASH: " + ex.ToString());
                Environment.Exit(1);
            }
        }

        public static bool SubExist(string name)
        {
            if(ArcLicenseApp.user_data.subscriptions.Exists(x => x.subscription == name))
                return true;
            return false;
        }
		
        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            try
            {
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            }
            catch
            {
                dtDateTime = DateTime.MaxValue;
            }
            return dtDateTime;
        }

        static void checkAtom()
        {
            Thread atomCheckThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(60000); // give people 1 minute to login

                    ushort foundAtom = GlobalFindAtom(ArcLicenseApp.ownerid);
                    if (foundAtom == 0)
                    {
                        TerminateProcess(GetCurrentProcess(), 1);
                    }
                }
            });

            atomCheckThread.IsBackground = true; // Ensure the thread does not block program exit
            atomCheckThread.Start();
        }

        static void securityChecks()
        {
            // check if the Loader was executed by a different program
            var frames = new StackTrace().GetFrames();
            foreach (var frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method != null && method.DeclaringType?.Assembly != Assembly.GetExecutingAssembly())
                {
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }

            // check if HarmonyLib is attempting to poison our program
            var harmonyAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "0Harmony");

            if (harmonyAssembly != null)
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }

            checkAtom();
        }

        static void autoUpdate()
        {
            if (ArcLicenseApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(ArcLicenseApp.app_data.downloadLink))
                {
                    Console.WriteLine("\n Auto update avaliable!");
                    Console.WriteLine(" Choose how you'd like to auto update:");
                    Console.WriteLine(" [1] Open file in browser");
                    Console.WriteLine(" [2] Download file directly");
                    int choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Process.Start(ArcLicenseApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case 2:
                            Console.WriteLine(" Downloading file directly..");
                            Console.WriteLine(" New file will be opened shortly..");

                            WebClient webClient = new WebClient();
                            string destFile = Application.ExecutablePath;

                            string rand = random_string();

                            destFile = destFile.Replace(".exe", $"-{rand}.exe");
                            webClient.DownloadFile(ArcLicenseApp.app_data.downloadLink, destFile);

                            Process.Start(destFile);
                            Process.Start(new ProcessStartInfo()
                            {
                                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                FileName = "cmd.exe"
                            });
                            Environment.Exit(0);

                            break;
                        default:
                            Console.WriteLine(" Invalid selection, terminating program..");
                            Thread.Sleep(1500);
                            Environment.Exit(0);
                            break;
                    }
                }
                Console.WriteLine("\n Status: Version of this program does not match the one online. Furthermore, the download link online isn't set. You will need to manually obtain the download link from the developer.");
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
        }

        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;
        }
    }
}
