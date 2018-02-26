using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Lantronix_Netzwerk_Tool
{
    public partial class Form1 : Form
    {
        //Formatierung der Textbox
        String IPundMAC = "{0,-30}{1, 24}";
        String IPundMAC2 = "{0,-30}{1, 15}";
        public Form1()
        {
            InitializeComponent();

            //Textbox zu Beginn füllen
            listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Vollständige Suche
            textBox2.Clear();
            Cursor.Current = Cursors.WaitCursor;
            progressBar1.Increment(-100000);
            listBox1.Items.Clear();
            listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));

            Byte[] data = { 0x00, 0x00, 0x00, 0xF6 };

            int port = 30718;

            // Socket definieren
            Socket bcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //bcSocket.Connect(new IPEndPoint(IPAddress.Broadcast, port));

            // EndPoint definieren bzw. Ziel des Broadcastes
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);

            // Optionen auf den Socket binden
            bcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            int Timeout = 2500;

            if(checkBox1.Checked)
            {
                Timeout = 30000;
            }
            bcSocket.ReceiveTimeout = Timeout;

            byte[] test = new byte[1024];

            IPEndPoint _sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)_sender;

            int durchlauf = 0;
            int anzahlg = 0;

            //Broadcasten!
            while (durchlauf <= 100000)
            {
                int ausf = 1;
                bcSocket.SendTo(data, iep);
                try
                {
                    bcSocket.ReceiveFrom(test, ref senderRemote); //IP steht in senderRemote!
                }
                catch
                {
                    ausf = 0;
                    durchlauf = 100000;
                    listBox1.Items.Clear();
                    listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("FEHLER #01");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Kein Gerät gefunden.");
                    listBox1.Items.Add("Bitte Verbindung zu Lantronix-Gerät prüfen.");
                    listBox1.Items.Add("- Timeout erhöhen -");

                }
                if (ausf == 1)
                {
                    string IP1 = senderRemote.ToString();
                    int stelle = IP1.IndexOf(":");

                    string NewIP1 = IP1.Substring(0, stelle);

                    //Mac aus Datenpaket filtern
                    string MAC1 = BitConverter.ToString(test);
                    string NewMac1 = MAC1.Substring(72, 17);

                    //Schauen ob Mac bereits in Liste
                    var item2 = listBox1.FindString(NewIP1);

                    if (item2 == -1)
                    {
                        listBox1.Items.Add(String.Format(IPundMAC2, NewIP1, NewMac1));
                        this.Refresh();
                        anzahlg++;
                    }
                }

                durchlauf++;
                this.progressBar1.Increment(1);
            }
            bcSocket.Close();
            Cursor.Current = Cursors.Default;
            textBox2.Text = anzahlg.ToString();
        }
        







        private void button3_Click(object sender, EventArgs e)
        {
            //Hier nur nach MAC suchen
            this.progressBar1.Increment(-100000);
            textBox2.Clear();

            Byte[] data = { 0x00, 0x00, 0x00, 0xF6 };

            int port = 30718;
            //string Antwort;


            // Socket definieren
            Socket bcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //bcSocket.Connect(new IPEndPoint(IPAddress.Broadcast, port));

            // EndPoint definieren bzw. Ziel des Broadcastes
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);

            // Optionen auf den Socket binden
            bcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            int Timeout = 2500;

            if (checkBox1.Checked)
            {
                Timeout = 30000;
            }
            bcSocket.ReceiveTimeout = Timeout;

            byte[] test = new byte[1024];

            IPEndPoint _sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)_sender;

            //Gesuchte MAC einlesen
            string sMAC = textBox1.Text;

            //Abfrage ob Text geändert wurde
            if (sMAC == "00-80-A3-BE-2F-")
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("FEHLER #02");
                listBox1.Items.Add("");
                listBox1.Items.Add("Keine MAC-Adresse erkannt.");
                listBox1.Items.Add("Bitte die letzten zwei Ziffern angeben.");
                listBox1.Items.Add("- Timeout erhöhen -");

            }
            else
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                Cursor.Current = Cursors.WaitCursor;
                int gefunden = 0;
                int abbruch = 0;

                //Suche solange bis MAC gefunden wurde
                while (gefunden == 0)
                {
                    int ausf2 = 1;
                    bcSocket.SendTo(data, iep);
                    try
                    {
                        bcSocket.ReceiveFrom(test, ref senderRemote); //IP steht in senderRemote!
                    }
                    catch
                    {
                        ausf2 = 0;
                        gefunden = 1;
                        listBox1.Items.Clear();
                        listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("FEHLER #03");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("Kein Gerät mit der angegeben MAC gefunden.");
                        listBox1.Items.Add("Bitte Verbindung und MAC prüfen.");
                        listBox1.Items.Add("(Groß- und Kleinschreibung beachten!)");
                    }

                    if (ausf2 == 1)
                    {
                        string IP2 = senderRemote.ToString();

                        //Port abtrennen nach :
                        int stelle = IP2.IndexOf(":");
                        string NewIP2 = IP2.Substring(0, stelle);

                        //Port anzeigen wie folgt:
                        //int port = Integer.parseInt(s.substring(s.indexOf(':') + 1));

                        //Mac aus Datenpaket filtern
                        string MAC2 = BitConverter.ToString(test);
                        string NewMac2 = MAC2.Substring(72, 17);

                        if (sMAC == NewMac2)
                        {
                            listBox1.Items.Add(String.Format(IPundMAC2, NewIP2, NewMac2));
                            gefunden = 1;
                            this.progressBar1.Increment(100000);
                            textBox2.Text = "1";
                        }

                        //Direkt das erste wählen (geht nicht richtig)
                        //listBox1.SetSelected(1, true);
                    }
                    abbruch++;
                    if(abbruch >= 100000)
                    {
                        //Prüfen ob Abbruch nicht besser geht (z.B über länge der Mac)
                        //Auserdem vorher kleinbuchstaben zu groß wandeln
                        gefunden = 1;
                        Cursor.Current = Cursors.Default;
                        //Vorgabe Text zurücksetzen
                        textBox1.Text = "00-80-A3-BE-2F-";
                        listBox1.Items.Clear();
                        listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("FEHLER #05");
                        listBox1.Items.Add("");
                        listBox1.Items.Add("Zeitüberschreitung.");
                        listBox1.Items.Add("MAC ist nicht im Netzwerk vorhanden.");
                        listBox1.Items.Add("Bitte Verbindung und MAC prüfen.");
                        listBox1.Items.Add("(Groß- und Kleinschreibung beachten!)");
                    }

                }
                // Socket schliessen, nach erfolgreichem Senden des Broadcastes
                bcSocket.Close();
                Cursor.Current = Cursors.Default;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Schauen welche IP ausgewählt wurde und Verbinden
            string ausw = listBox1.GetItemText(listBox1.SelectedItem);
                int stelle2 = ausw.IndexOf(" ");
            if (stelle2 == -1)
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("");
                listBox1.Items.Add("FEHLER #04");
                listBox1.Items.Add("");
                listBox1.Items.Add("Keine Auswahl erkannt.");
                listBox1.Items.Add("Bitte IP auswählen und erneut Verbinden.");
            }
            else
            {
                string Auswahl = ausw.Substring(0, stelle2);
                string http = Auswahl.Insert(0, "http://");

                //Mit IP verbinden über Browser
                System.Diagnostics.Process.Start(http);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Liste löschen
            listBox1.Items.Clear();
            listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
            textBox2.Clear();
            this.progressBar1.Increment(-100000);
            textBox1.Text = ("00-80-A3-BE-2F-");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Einfache Suche
            textBox2.Clear();
            Cursor.Current = Cursors.WaitCursor;
            progressBar1.Increment(-100000);
            listBox1.Items.Clear();
            listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));

            Byte[] data = { 0x00, 0x00, 0x00, 0xF6 };

            int port = 30718;

            // Socket definieren
            Socket bcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // EndPoint definieren bzw. Ziel des Broadcastes
            IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, port);

            // Optionen auf den Socket binden
            bcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            int Timeout = 2500;

            if (checkBox1.Checked)
            {
                Timeout = 30000;
            }
            bcSocket.ReceiveTimeout = Timeout;

            byte[] test = new byte[1024];

            IPEndPoint _sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)_sender;

            int durchlauf = 0;
            int anzahlg2 = 0;

            while (durchlauf <= 100)
            {
                int ausf = 1;
                bcSocket.SendTo(data, iep);
                try
                {
                    bcSocket.ReceiveFrom(test, ref senderRemote); //IP steht in senderRemote!
                }
                catch
                {
                    ausf = 0;
                    durchlauf = 100;
                    listBox1.Items.Clear();
                    listBox1.Items.Add(String.Format(IPundMAC, "IP-Adresse", "Hardware-Adresse (MAC)"));
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("FEHLER #01");
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Kein Gerät gefunden.");
                    listBox1.Items.Add("Bitte Verbindung zu Lantronix-Gerät prüfen.");
                    listBox1.Items.Add("- Timeout erhöhen -");                }
                if (ausf == 1)
                {
                    string IP1 = senderRemote.ToString();
                    int stelle = IP1.IndexOf(":");

                    string NewIP1 = IP1.Substring(0, stelle);

                    //Mac aus Datenpaket filtern
                    string MAC1 = BitConverter.ToString(test);
                    string NewMac1 = MAC1.Substring(72, 17);

                    //Schauen ob Mac bereits in Liste
                    var item2 = listBox1.FindString(NewIP1);

                    this.progressBar1.Increment(10000);

                    if (item2 == -1)
                    {
                        listBox1.Items.Add(String.Format(IPundMAC2, NewIP1, NewMac1));
                        this.Refresh();
                        anzahlg2++;
                    }
                }

                durchlauf++;
            }
            bcSocket.Close();
            Cursor.Current = Cursors.Default;
            textBox2.Text = anzahlg2.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
