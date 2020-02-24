using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace wpf.services
{
    [Serializable]
    class UserPass
    {
        public string User { get; set; }
        public string Pass { get; set; }

        public void Load()
        {
            if (File.Exists(@"conf.cnf"))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(@"conf.cnf", FileMode.Open, FileAccess.Read);
                    UserPass userPass = (UserPass)formatter.Deserialize(stream);
                    this.User = userPass.User;
                    this.Pass = userPass.Pass;
                    stream.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }
        }
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"conf.cnf", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, this);
            stream.Close();
        }
    }
}
