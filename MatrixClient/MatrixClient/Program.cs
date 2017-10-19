namespace MatrixClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Xml.Serialization;

    public class client
    {
        public static void Main()
        {

            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect("10.20.133.186", 8002);
                Stream stm = tcpclnt.GetStream();


                byte[] byteArray1;
                byte[] byteArray2;
                byte[] result = new byte[1];
                // Do-while loop used for the continous flow of work and calculations.
                do
                {
                    Console.WriteLine("Connected");
                    Console.Write("Waiting for Arrays to work on.. ");

                    byteArray1 = new byte[100];
                    byteArray2 = new byte[100];
                    stm.Read(byteArray1, 0, 100);
                    stm.Read(byteArray2, 0, 100);
                    Console.WriteLine("Received array.. beginning calculations...");

                    Console.WriteLine("\nThe first array is as follows: \n");
                    for (int i = 0; i < byteArray1.Length; i++)
                    {
                        Console.Write(" " + byteArray1[i]);
                    }

                    Console.WriteLine("\nThe Second array is as follows: \n");
                    for (int i = 0; i < byteArray1.Length; i++)
                    {
                        Console.Write(" " + byteArray2[i]);
                    }

                    Console.WriteLine("\n\nNow beginning matrix calculation with the two give arrays..");
                    result[0] = matrixCalculation(byteArray1, byteArray2);
                    Console.WriteLine("The calculation has finished.");
                    Console.WriteLine("\nNow transmitting results back to master...");

                    stm.Write(result,0,1);                   
                } while (true);

                tcpclnt.Close();
                Console.WriteLine("The program has finished, please press a key to exit.");
                Console.ReadKey();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private static byte matrixCalculation(byte[] byteArray1, byte[] byteArray2)
        {
            int sum1 = 0;
            int sum2 = 0;

            for(int i = 0; i < byteArray1.Length; i++)
            {
                sum1 += byteArray1[i];
                sum2 += byteArray2[i];
            }
            return (byte)(sum1 * sum2);
        }
    }
}
