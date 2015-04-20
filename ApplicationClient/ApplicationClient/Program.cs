namespace ApplicationClient
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The client.
    /// </summary>
    class Client
    {
        /// <summary>
        /// The _client.
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The _stream reader.
        /// </summary>
        private StreamReader _streamReader;

        /// <summary>
        /// The _network stream.
        /// </summary>
        private NetworkStream _networkStream;

        /// <summary>
        /// The _game field.
        /// </summary>
        private string[,] _gameField = new string[3, 3];

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public Client(string port)
        {
            this._client = new TcpClient("127.0.0.1", Convert.ToInt32(port));

            this._streamReader = new StreamReader(this._client.GetStream());

            this._networkStream = this._client.GetStream();
        }

        /// <summary>
        /// The make move.
        /// </summary>
        private void MakeMove()
        {
            string massege;
            do
            {
                Console.WriteLine("делай ход:");
                massege = Console.ReadLine();
            }
            while (massege == null || this.InvalidMassege(this.ConvertStringToInt(massege.Split(' '))));

            int[] coordinate = this.ConvertStringToInt(massege.Split(' '));

            this.SaveMyProgress(coordinate);
            _networkStream.Write(Encoding.UTF8.GetBytes(massege), 0, massege.Length);

            Console.WriteLine(this._gameField);
        }

        /// <summary>
        /// The check massege.
        /// </summary>
        /// <param name="coordinate">
        /// The coordinate.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InvalidMassege(int[] coordinate)
        {
            return (coordinate.Count() > 2);
        }

        /// <summary>
        /// The save my progress.
        /// </summary>
        /// <param name="coordinate">
        /// The coordinate.
        /// </param>
        private void SaveMyProgress(int[] coordinate)
        {
            this._gameField[coordinate[0], coordinate[1]] = "X";
        }

        /// <summary>
        /// The convert string to int.
        /// </summary>
        /// <param name="coordinate">
        /// The coordinate.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        private int[] ConvertStringToInt(string[] coordinate)
        {
            var intCoordinate = new int[] { Convert.ToInt32(coordinate[0]), Convert.ToInt32(coordinate[1]) };

            return intCoordinate;
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes("I'm ready to play!" + "\r\n");
                this._networkStream.Write(data, 0, data.Length);

                while (true)
                {
                    var masseg = this._streamReader.ReadLine();

                    if (masseg == "go")
                    {
                        this.MakeMove();
                    }
                    if (masseg == "wait!")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                
                this._client.Close();
            }
            catch (Exception exp)
            {
                // вывод сообщения в случае возникновения оцибки
                Console.WriteLine("Исключение:" + exp);
            }
            
        }
    }

    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var client = new Client(args[0]);

            client.Run();

            Console.ReadLine();
        }
    }

}
