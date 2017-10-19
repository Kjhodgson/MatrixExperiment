namespace MatrixServer
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;


    public class server
    { 
        public static void Main()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("10.20.133.186");

                TcpListener myListener = new TcpListener(ipAd, 8002);

                myListener.Start();
                Console.WriteLine("The server is running on port 8001.....");
                Console.WriteLine("The local End Point is :" + myListener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection..");

                Socket mySocket = myListener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + mySocket.RemoteEndPoint);

                Console.WriteLine("Creating the Arrays....");

                int arraySize = 100;
                Random rand = new Random();

                byte[] byteArray1 = new byte[arraySize];
                byte[] byteArray2 = new byte[arraySize];
                for (int i = 0; i < arraySize; i++)
                {                  
                    byteArray1[i] = Convert.ToByte(rand.Next(1, 3));
                    byteArray2[i] = Convert.ToByte(rand.Next(1, 3));
                }

                Console.WriteLine("Created.");
                Console.WriteLine("Printing Arrays..\n");
                Console.WriteLine("Printing Array 1:");
                for (int i = 0; i < arraySize; i++)
                {
                    Console.Write(string.Format("{0} ", byteArray1[i]));                                      
                }
                Console.Write("\nPrinting Array 2:\n");
                for (int i = 0; i < arraySize; i++)
                {
                    Console.Write(string.Format("{0} ", byteArray1[i]));
                }
                Console.WriteLine("\n");

                Console.WriteLine("\nSending the arrays to the worker.");
                mySocket.Send(byteArray1);
                mySocket.Send(byteArray2);
                Console.WriteLine("The Arrays have been sent.");

                Console.WriteLine("Waiting to recieve completed work....");

                byte[] result = new byte[1];
                int resultNumber = mySocket.Receive(result);
                //byte number = result[0];

                Console.WriteLine("Received: " + result[0] + "\n");

                Console.WriteLine("The program has finished, please press a key to exit.");
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

    }
}
