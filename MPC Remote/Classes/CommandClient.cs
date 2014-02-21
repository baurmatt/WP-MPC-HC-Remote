using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MPC_Remote
{
    public class CommandClient
    {
        // Cached Socket object that will be used by each call for the lifetime of this class
        static Socket _Socket = null;

        // Signaling object used to notify when an asynchronous operation is completed
        static ManualResetEvent _ClientDone = new ManualResetEvent(false);

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandClient()
        {
            
        }

        /// <summary>
        /// Execute a command on the given PC
        /// </summary>
        /// <param name="ConnectionProfile">ConnectionProfile which is used to connected to the client</param>
        /// <param name="Command">Command which should executed</param>
        public static void ExecuteCommand(ConnectionProfile ConnectionProfile, string Command)
        {
            try
            {
                string RequestURL = "http://" + ConnectionProfile.Hostname + ":" + ConnectionProfile.Port + "/command.html?wm_command=" + Command;
                HttpWebRequest.CreateHttp(new Uri(RequestURL, UriKind.Absolute)).BeginGetResponse((AsyncCallback)(result => { }), null);
            }
            catch { }
        }

        /// <summary>
        /// Attempt a TCP socket connection to the given host over the given port
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        /// <param name="portNumber">The port number to connect</param>
        /// <returns>A string representing the result of this connection attempt</returns>
        internal static string Connect(string hostName, int portNumber)
        {
            string result = string.Empty;

            // Create DnsEndPoint. The hostName and port are passed in to this method.
            DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);

            // Create a stream-based, TCP socket using the InterNetwork Address Family. 
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Create a SocketAsyncEventArgs object to be used in the connection request
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;

            // Inline event handler for the Completed event.
            // Note: This event handler was implemented inline in order to make this method self-contained.
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                // Retrieve the result of this request
                result = e.SocketError.ToString();

                // Signal that the request is complete, unblocking the UI thread
                _ClientDone.Set();
            });

            // Make an asynchronous Connect request over the socket
            _Socket.ConnectAsync(socketEventArg);

            System.Threading.Thread.Sleep(200);

            return result;
        }

        public static bool IsConnectionAvailable(string Hostname, int Port)
        {
            if (IsNetworkAvailable && CommandClient.Connect(Hostname, Port) == "Success")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNetworkAvailable
        {
            get
            {
            return Microsoft.Phone.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            }
        }
    }
}
