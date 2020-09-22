using Hardcodet.Wpf.TaskbarNotification;
using Interop.QBXMLRP2;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using wpf.Control;
using wpf.Models;
using wpf.services;


namespace wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection HubConnection;
        ConectionConstrol conectionConstrol;

        RequestProcessor2 rp = null;
        string ticket = null;
        string response = null;
        TaskbarIcon tbi = new TaskbarIcon();
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            LabelAssambly.Content = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            #region iconNotification
            tbi.Icon = Resource1.file_qbm;
            tbi.ContextMenu = new ContextMenu();
            tbi.MenuActivation = PopupActivationMode.LeftOrRightClick;
            tbi.TrayMouseDoubleClick += new RoutedEventHandler(MenuItem_Click2);

            MenuItem menuItem1 = new MenuItem()
            {
                Header = "Close",
                IsCheckable = false,
                Icon = new Image() { Source = Resource1.close.ToImageSource(), Width = 15, Height = 15 }
            };
            menuItem1.Click += MenuItem_Click1;

            MenuItem menuItem2 = new MenuItem()
            {
                Header = "Show",
                IsCheckable = false,
                Icon = new Image() { Source = Resource1.maximise.ToImageSource(), Width = 15, Height = 15 }
            };
            menuItem2.Click += MenuItem_Click2;

            tbi.ContextMenu.Items.Add(menuItem2);
            tbi.ContextMenu.Items.Add(menuItem1);
            #endregion

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 10, 0);
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Start();

            UserPass userPass = new UserPass();
            userPass.Load();
            userTextBox.Text = userPass.User;
            passwordUser.Password = userPass.Pass;

            conectionConstrol = new ConectionConstrol();
            conectionConstrol.AssamblyChequin();

            HubConnection = new HubConnectionBuilder()
                .WithUrl(conectionConstrol.baseUrl + "servicehub")
                .WithAutomaticReconnect(new RetryPolicy())
                .Build();

            HubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                try
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        var newMessage = $"{user} : {message}";
                        messagesList.Items.Insert(0, newMessage);
                    });
                }
                catch (Exception)
                {

                }

            });

            HubConnection.On<string, string, string>("runQuery", (message, token, funcion) =>
             {
                 this.Dispatcher.Invoke(() =>
                 {
                     TimeSpan stop;
                     TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

                     try
                     {
                         response = rp.ProcessRequest(ticket, message);
                     }
                     catch (System.Runtime.InteropServices.COMException ex)
                     {
                         string filePath = "log.txt";
                         FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                         StreamReader sr = new StreamReader(fs);
                         StreamWriter sw = new StreamWriter(fs);
                         string newString = sr.ReadToEnd() + Environment.NewLine + DateTime.Now + ": " + message;
                         sw.Write(newString);
                         sw.Flush(); //HERE
                         fs.Close();
                         messagesList.Items.Insert(0, DateTime.Now.ToString() + " " + ex.Message);
                         return;
                     }

                     stop = new TimeSpan(DateTime.Now.Ticks);
                     messagesList.Items.Insert(0, DateTime.Now.ToString() + ": " + Math.Round(stop.Subtract(start).TotalMilliseconds));
                     string dat = conectionConstrol.RunQueryReturn(response, token, funcion).GetAwaiter().GetResult();
                     if (dat != "")
                         messagesList.Items.Insert(0, dat);
                     if (messagesList.Items.Count > 400)
                         messagesList.Items.RemoveAt(messagesList.Items.Count - 1);
                 });
             });

            HubConnection.On("Login", () =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Login();
                    });
                }
                catch (Exception)
                {
                }
            });

            if (userPass.User != null && userPass.Pass != null)
                LoginButton();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            conectionConstrol.AssamblyChequin();
        }

        private void Login()
        {
            string resul = conectionConstrol.login(HubConnection.ConnectionId, userTextBox.Text, passwordUser.Password);
            if (resul == "OK")
            {
                if (HubConnection.State == HubConnectionState.Connected)
                {
                    if (ticket != null)
                    {
                        rp.EndSession(ticket);
                    }
                    if (rp != null)
                    {
                        rp.CloseConnection();
                    }
                }

                rp = new RequestProcessor2();
                rp.OpenConnection("", "IDN CustomerAdd C# sample");
                ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);

                btn1C.Content = "Close";
                messagesList.Items.Insert(0, "Login");

                UserPass userPass = new UserPass();
                userPass.User = userTextBox.Text;
                userPass.Pass = passwordUser.Password;
                userPass.Save();
            }
            else
            {
                messagesList.Items.Insert(0, resul);
            }
        }
        private async void LoginButton()
        {
            try
            {
                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                    messagesList.Items.Insert(0, "Connection started");
                }
                else
                {
                    if (conectionConstrol.token == null && HubConnection.State == HubConnectionState.Connected)
                        Login();
                    else
                    {
                        await HubConnection.StopAsync();
                        messagesList.Items.Insert(0, "Close");
                        btn1C.Content = "Login";
                    }
                }
            }
            catch (Exception ex)
            {
                messagesList.Items.Insert(0, ex.Message);
            }
        }

        //Login Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoginButton();
        }
        //status
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            messagesList.Items.Insert(0, HubConnection.ConnectionId + " -> " + HubConnection.State.ToString());
        }
        //clear listbox
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            messagesList.Items.Clear();
        }
        //move Windows
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //minimizar
        private void CloseAPP(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
        //menu1 click
        private void MenuItem_Click1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        //menu2 click
        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
            this.Show();
            this.Activate();
        }

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    messagesList.Items.Insert(0,"---->");
        //    Thread thread;
        //    for (int i = 0; i < 1; i++)
        //    {
        //        int a = i;
        //        thread = new Thread(x=>prueba(a));
        //        thread.IsBackground = true;
        //        thread.Start();
        //    }
        //    messagesList.Items.Insert(0,"<---");
        //}
        //private void prueba(int i)
        //{ 
        //    Random a = new Random();
        //    int valor = a.Next(0000, 2000);
        //    //Thread.Sleep(valor);
        //    string message = $"<?xml version=\"1.0\"?><?qbxml version=\"2.0\"?><QBXML><QBXMLMsgsRq onError = \"continueOnError\"><CustomerQueryRq requestID = \"2\" /></QBXMLMsgsRq ></QBXML>";
        //    TimeSpan stop;
        //    TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

        //    //step3: do the qbXMLRP request
        //    RequestProcessor2 rp = null;
        //    string ticket = null;
        //    string response = null;
        //    try
        //    {
        //        //Interop.QBXMLRP2
        //        rp = new RequestProcessor2();
        //        rp.OpenConnection("", "IDN CustomerAdd C# sample");
        //        ticket = rp.BeginSession("", QBFileMode.qbFileOpenDoNotCare);
        //        response = rp.ProcessRequest(ticket, message);

        //    }
        //    catch (System.Runtime.InteropServices.COMException ex)
        //    {
        //        MessageBox.Show("COM Error Description = " + ex.Message, "COM error");
        //        return;
        //    }
        //    finally
        //    {
        //        if (ticket != null)
        //        {
        //            rp.EndSession(ticket);
        //        }
        //        if (rp != null)
        //        {
        //            rp.CloseConnection();
        //        }
        //    };
        //    stop = new TimeSpan(DateTime.Now.Ticks);
        //    this.Dispatcher.Invoke(() =>
        //    {
        //       messagesList.Items.Insert(0,i + "--> " + stop.Subtract(start).TotalMilliseconds + " " + DateTime.Now.ToString());
        //        //messagesList.Items.Insert(0,i+"->" +valor);
        //    });
        //}
    }
}
