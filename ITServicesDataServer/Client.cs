﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using Remote_Content_Show_Protocol;
using System.Net.Http;

namespace ITServicesDataServer
{
    public class Client
    {
        private TcpClient client;
        private Thread thread;
        private SqlConnection connetion = new SqlConnection("Data Source=datafhdbF1;Initial Catalog=FHDB;Persist Security Info=True;User ID=www_stuplan;Password=Export_stuplan");

        //-http://code.tutsplus.com/tutorials/http-headers-for-dummies--net-8039

        public event EventHandler<RequestHandler> OnRequest;

        public Client(TcpClient client)
        {
            this.client = client;
            this.thread = new Thread(new ThreadStart(Work));
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        protected void FireOnRequest(string request, IPAddress ip)
        {
            if (this.OnRequest != null)
            {
                this.OnRequest(this, new RequestHandler(request, ip));
            }
        }

        private void Work()
        {
            NetworkStream stream = this.client.GetStream();
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] input = line.Split(' ');
                if (input.Length == 3)
                {
                    if (input[0] == "GET" && input[1].StartsWith("/room="))
                    {
                        this.FireOnRequest(line, IPAddress.Parse(((IPEndPoint)this.client.Client.RemoteEndPoint).Address.ToString()));
                        string paramether = input[1].Remove(0, 6);
                        //SqlCommand command = new SqlCommand("SELECT TOP(3) * FROM v_stuplan WHERE SAAL = '" + paramether + "' ORDER BY DATUM, VON", connetion);
                        SqlCommand command = new SqlCommand("SELECT TOP(3) * FROM v_stuplan WHERE SAAL = '" + paramether + "' ORDER BY DATUM, VON", connetion);
                        this.connetion.Open();
                        SqlDataReader sqlResult = command.ExecuteReader();
                        List<ReslutItem> erg = new List<ReslutItem>();
                        while(sqlResult.Read())
                        {
                            ReslutItem ri = new ReslutItem();
                            ri.Datum = sqlResult.GetDateTime(0);
                            ri.Von = sqlResult.GetTimeSpan(1);
                            ri.Bis = sqlResult.GetTimeSpan(2);
                            ri.Saal = sqlResult.GetString(3);
                            ri.Bezeichnung = sqlResult.GetString(4);
                            ri.LVArt = sqlResult.GetString(5);
                            ri.LVBezeichnung = sqlResult.GetString(6);
                            erg.Add(ri);
                        }

                        List<byte> response = new List<byte>();
                        response.AddRange(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n"));
                        response.AddRange(Encoding.UTF8.GetBytes("Content-Type: application/json; charset=UTF-8\r\n\r\n"));
                        response.AddRange(Remote_Content_Show_MessageGenerator.GetMessageAsByte(erg));
                        byte[] data = response.ToArray();
                        stream.Write(data, 0, data.Length);
                        //response.Content = Remote_Content_Show_MessageGenerator.toJason(erg);



                        this.connetion.Close();
                    }
                }
            }

            reader.Close();
            stream.Close();
            this.client.Close();
        }
    }
}
