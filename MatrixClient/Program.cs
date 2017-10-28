namespace MatrixClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Xml.Serialization;

    public class Client
    {
        public static void Main()
        {

            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect("10.20.143.110", 8002);
                Stream stm = tcpclnt.GetStream();


                byte[] byteArray1;
                byte[] byteArray2;
                int[] intArray1;
                int[] intArray2;
                int result;
                int i = 0;
                // Do-while loop used for the continous flow of work and calculations.
                do
                {
                    Console.WriteLine("Connected");
                    Console.Write("Waiting for Arrays to work on.. ");

                    byteArray1 = new byte[4];
                    byteArray2 = new byte[4];
                    intArray1 = new int[800];
                    intArray2 = new int[800];

                    for (i = 0; i < intArray1.Length; i++)
                    {
                        stm.Read(byteArray1, 0, byteArray1.Length);
                        intArray1[i] = BitConverter.ToInt32(byteArray1, 0);
                    }
                    for (i = 0; i < intArray1.Length; i++)
                    {
                        stm.Read(byteArray2, 0, 4);
                        intArray2[i] = BitConverter.ToInt32(byteArray2, 0);
                    }
                    Console.WriteLine("Received array.. beginning calculations...");

                    //Console.WriteLine("\nThe first array is as follows: \n");
                    //for (i = 0; i < intArray1.Length; i++)
                    //{
                        //Console.Write(" " + intArray1[i]);
                    //}

                    //Console.WriteLine("\nThe Second array is as follows: \n");
                    //for (i = 0; i < intArray1.Length; i++)
                    //{
                        //Console.Write(" " + intArray2[i]);
                    //}

                    //Console.WriteLine("\n\nNow beginning matrix calculation with the two give arrays..");
                    result = matrixCalculation(intArray1, intArray2);
                    //Console.WriteLine("The calculation has finished.");
                    //Console.WriteLine("\nNow transmitting results back to master...");

                    stm.Write(BitConverter.GetBytes(result),0,4);                   
                } while (true); //For now the worker will work until death

                tcpclnt.Close();
                Console.WriteLine("The program has finished, please press a key to exit.");
                Console.ReadKey();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private static int matrixCalculation(int[] array1, int[] array2)
        {
            int sum1 = 0;
            int sum2 = 0;

            for(int i = 0; i < array1.Length; i++)
            {
                sum1 += array1[i];
                sum2 += array2[i];
            }
            //Console.WriteLine("Calculated: " + (byte)(sum1 * sum2));
            //Console.WriteLine("The result is: " + (sum1 * sum2));
            return (sum1 * sum2);
        }
    }
}
