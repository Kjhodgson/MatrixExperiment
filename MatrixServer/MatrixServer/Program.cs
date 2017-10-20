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
        private static int NUMROWS = 100;
        private static int NUMCOLS = 100;
        private static byte[,] array;
        private static byte[,] completedArray;

        private static int numOfWorkers = 2;
        private static Socket[] mySocketArray = new Socket[numOfWorkers];

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
                
                for (int i = 0; i < numOfWorkers; i++)
                {
                    mySocketArray[i] = myListener.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + mySocketArray[i].RemoteEndPoint);
                    Console.WriteLine("Waiting for another connection...");
                }               

                Console.WriteLine("Creating the Array to be calculated....");

                Random rand = new Random();
                
                array = new byte[NUMROWS, NUMCOLS];
                completedArray = new byte[NUMROWS, NUMCOLS];
                for (int i = 0; i < NUMROWS; i++)
                {           
                    for (int j = 0; j < NUMCOLS; j++)
                    {
                        array[i,j] = Convert.ToByte(rand.Next(1, 3));
                    }
                    
                }

                Console.WriteLine("Created.");
                Console.WriteLine("Printing Array..\n");
                PrintArray(array);
                var sw = new Stopwatch();
                Task[] tasks = new Task[numOfWorkers];

                //sw.Start();
               
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

                PrintArray(completedArray);

                //sw.Stop();
                //Console.WriteLine("\nThe calculation took {0} (ms)", sw.ElapsedMilliseconds);
                //decimal timeSeconds = decimal.Divide((decimal)sw.ElapsedMilliseconds, (decimal)1000);
                //Console.WriteLine("Time in seconds: {0}", timeSeconds);

                Console.WriteLine("\nThe program has finished, please press a key to exit.");
                Console.ReadKey();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public static void SendWork(int worker, int Row, int Col)
        {
            Console.WriteLine("Sending work to the worker(s)...");
            Console.WriteLine("The current Row and Col are:" + Row + " " + Col);
            byte[] byteArrayRow = new byte[NUMROWS];
            byte[] byteArrayCol = new byte[NUMCOLS];
            
            for (int i = 0; i < NUMROWS; i++)
            {
                byteArrayCol[i] = array[i, Col];
            }
            for (int j = 0; j < NUMCOLS; j++)
            {
                byteArrayRow[j] = array[Row, j];
            }
            Console.WriteLine("Did I get here....");
            mySocketArray[worker].Send(byteArrayRow);
            mySocketArray[worker].Send(byteArrayCol);

            Console.WriteLine("Waiting to recieve completed work....");

            byte[] result = new byte[1];
            mySocketArray[worker].Receive(result);
            Console.WriteLine("Received: " + result[0] + "\n");

            completedArray[Row, Col] = result[0];

            Console.WriteLine("This unit of work has been completed.\n");

            //timer to see if array is being populated properly.
            //System.Threading.Thread.Sleep(5000);
        }

        private static void PrintArray(byte[,] array)
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
