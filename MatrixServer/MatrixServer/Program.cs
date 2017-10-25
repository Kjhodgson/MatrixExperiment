namespace MatrixServer
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Threading;

    public class Server
    {
        private static int NUMROWS = 800;
        private static int NUMCOLS = 800;
        private static int[,] array;
        private static int[,] completedArray;

        private static int numOfWorkers = 2;
        private static Socket[] mySocketArray = new Socket[numOfWorkers];

        public static void Main()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse("10.20.143.110");

                TcpListener myListener = new TcpListener(ipAd, 8002);                

                myListener.Start();
                Console.WriteLine("The server is running on port 8001.....");
                Console.WriteLine("The local End Point is :" + myListener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection..");
                
                for (int i = 0; i < numOfWorkers; i++)
                {
                    mySocketArray[i] = myListener.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + mySocketArray[i].RemoteEndPoint);
                    Console.WriteLine("Waiting for another connection...");
                }

                // Will calculate 100 arrays.
                int count = 0;
                do
                {
                    Console.WriteLine("Creating the Array to be calculated....");

                    Random rand = new Random();

                    array = new int[NUMROWS, NUMCOLS];
                    completedArray = new int[NUMROWS, NUMCOLS];
                    for (int i = 0; i < NUMROWS; i++)
                    {
                        for (int j = 0; j < NUMCOLS; j++)
                        {
                            array[i, j] = rand.Next(1, 4);
                        }

                    }

                    //Console.WriteLine("Created.");
                    //Console.WriteLine("Printing Array..\n");
                    //PrintArray(array);
                    var sw = new Stopwatch();
                    Task[] tasks = new Task[numOfWorkers];

                    sw.Start();

                    for (int worker = 0; worker < numOfWorkers; worker++)
                    {
                        int theWorker = worker;
                        tasks[theWorker] = Task.Factory.StartNew(() =>
                        {
                            int rowsToCompute = NUMROWS / numOfWorkers;
                            int rowsUpperBound = (theWorker + 1) * rowsToCompute;
                            int rowsLowerBound = theWorker * rowsToCompute;

                            Console.WriteLine("\nSending work to the worker.");

                            // This will go through the array and send the work to the workers.
                            for (int i = rowsLowerBound; i < rowsUpperBound; i++)
                            {
                                for (int j = 0; j < NUMCOLS; j++)
                                {
                                    SendWork(theWorker, i, j);
                                }
                            }
                        });
                    }
                    Task.WaitAll(tasks);
                    Console.WriteLine("The Following array is the completed array:\n");

                    //PrintArray(completedArray);

                    sw.Stop();
                    Console.WriteLine("\nThe calculation took {0} (ms)", sw.ElapsedMilliseconds);
                    decimal timeSeconds = decimal.Divide((decimal)sw.ElapsedMilliseconds, (decimal)1000);
                    Console.WriteLine("Time in seconds: {0}", timeSeconds);
                    count++;
                    
                } while (count != 100);
                Console.WriteLine("\nThe program has finished, please press a key to exit.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public static void SendWork(int worker, int row, int col)
        {
            //Console.WriteLine("Sending work to the worker(s)...");
            //Console.WriteLine("The current Row and Col are:" + row + " " + col);

            int[] ArrayRow = new int[NUMROWS];
            int[] ArrayCol = new int[NUMCOLS];
            
            for (int i = 0; i < NUMROWS; i++)
            {
               // Console.WriteLine("Sending this row number to worker " +worker + ": " +  array[i, col]);
                mySocketArray[worker].Send(BitConverter.GetBytes(array[i, col]));
                //Console.WriteLine("Worker " + worker + " should have recieved: " + BitConverter.ToInt32(test, 0));
            }
            for (int j = 0; j < NUMCOLS; j++)
            {
                mySocketArray[worker].Send(BitConverter.GetBytes(array[row, j]));
            }

            //Console.WriteLine("Waiting to recieve completed work....");

            byte[] byteResult = new byte[4];
            mySocketArray[worker].Receive(byteResult);
            int result = BitConverter.ToInt32(byteResult, 0);
            //Console.WriteLine("Received: " + result + "\n");

            completedArray[row, col] = result;

            //Console.WriteLine("This unit of work has been completed.\n");
        }

        private static void PrintArray(int[,] array)
        {
            //prints the array 
            for (int i = 0; i < NUMROWS; i++)
            {
                for (int j = 0; j < NUMCOLS; j++)
                {
                    Console.Write(string.Format("{0} ", array[i, j]));
                }
                Console.Write("\n");
            }
        }

    }
}
