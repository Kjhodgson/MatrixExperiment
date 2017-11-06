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
        private const int SIZE = 200;
        private const int NUM_MATRICES = 5;

        private static int[,] array;
        private static int[,] completedArray;

        private static int numOfWorkers;
        private static Socket[] mySocketArray;


        public static void Main(string[] args)
        {
            numOfWorkers = int.Parse(args[0]);
            TcpListener myListener = new TcpListener(IPAddress.Any, 8002);
            mySocketArray = new Socket[numOfWorkers];

            myListener.Start();
            Console.WriteLine("The server is running on port 8002.....");
            Console.WriteLine("The local End Point is :" + myListener.LocalEndpoint);

            for (int i = 0; i < numOfWorkers; i++)
            {
                Console.WriteLine("Waiting for a connection..");
                mySocketArray[i] = myListener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + mySocketArray[i].RemoteEndPoint);
            }

            int count = 0;
            var totalsw = Stopwatch.StartNew();

            do
            {
                //Console.WriteLine("Creating the Array to be calculated....");

                Random rand = new Random();

                array = new int[SIZE, SIZE];
                completedArray = new int[SIZE, SIZE];
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
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
                    tasks[theWorker] = Task.Factory.StartNew((Action)(() =>
                    {
                        int rowsToCompute = Server.SIZE / numOfWorkers;
                        int rowsUpperBound = (theWorker + 1) * rowsToCompute;
                        int rowsLowerBound = theWorker * rowsToCompute;

                            //Console.WriteLine("\nSending work to the worker.");

                            // This will go through the array and send the work to the workers.
                            for (int i = rowsLowerBound; i < rowsUpperBound; i++)
                            {
                                for (int j = 0; j < Server.SIZE; j++)
                                {
                                    SendWork(theWorker, i, j);
                                }
                            }
                    }));
                }
                Task.WaitAll(tasks);

                //Console.WriteLine("The Following array is the completed array:\n");
                //PrintArray(completedArray);

                sw.Stop();
                Console.WriteLine($"\nThe calculation took {sw.Elapsed}");
                count++;

            } while (count != NUM_MATRICES);

            totalsw.Stop();
            foreach (var s in mySocketArray)
            {
                s.Disconnect(false);
                s.Close();
            }

            Console.WriteLine("\nThe program has finished.");
            Console.WriteLine($"\nThe total time for all matrices: {totalsw.Elapsed}");
            Console.WriteLine($"\nThe total Average time was: {totalsw.ElapsedMilliseconds / NUM_MATRICES}(ms)");
        }

        public static void SendWork(int worker, int row, int col)
        {
            //Console.WriteLine("Sending work to the worker(s)...");
            //Console.WriteLine("The current Row and Col are:" + row + " " + col);

            int[] ArrayRow = new int[SIZE];
            int[] ArrayCol = new int[SIZE];

            for (int i = 0; i < SIZE; i++)
            {
                // Console.WriteLine("Sending this row number to worker " +worker + ": " +  array[i, col]);
                mySocketArray[worker].Send(BitConverter.GetBytes(array[i, col]));
                //Console.WriteLine("Worker " + worker + " should have recieved: " + BitConverter.ToInt32(test, 0));
            }
            for (int j = 0; j < SIZE; j++)
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
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    Console.Write(string.Format("{0} ", array[i, j]));
                }
                Console.Write("\n");
            }
        }

    }
}
