using System;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace MPC_Remote
{
    public class Status : INotifyPropertyChanged
    {
        private WebClient _WebClient = new WebClient();

        private string _filename;
        private string _filepatharg;
        private string _filepath;
        private string _filedirarg;
        private string _filedir;
        private int _state;
        private string _statestring;
        private double _position;
        private string _positionstring;
        private double _duration;
        private string _durationstring;
        private string _volumelevel;
        private string _muted;
        private string _playbackrate;
        private string _reloadtime;

        public string filename
        {
            get
            {
                return (string.IsNullOrEmpty(_filename)) ? "Title Info" : _filename;
            }
            set
            {
                if (this._filename == value)
                    return;

                _filename = value;
                PropChanged("filename");
            }
        }
        public string filepatharg
        {
            get { return _filepatharg; }
            set
            {
                if (this._filepatharg == value)
                    return;

                _filepatharg = value;
                PropChanged("filepatharg");
            }
        }
        public string filepath
        {
            get { return _filepath; }
            set
            {
                if (this._filepath == value)
                    return;

                _filepath = value;
                PropChanged("filepath");
            }
        }
        public string filedirarg
        {
            get { return _filedirarg; }
            set
            {
                if (this._filedirarg == value)
                    return;

                _filedirarg = value;
                PropChanged("filedirarg");
            }
        }
        public string filedir
        {
            get { return _filedir; }
            set
            {
                if (this._filedir == value)
                    return;

                _filedir = value;
                PropChanged("filedir");
            }
        }
        public int state
        {
            get { return _state; }
            set
            {
                if (this._state == value)
                    return;

                this._state = value;
                PropChanged("state");
            }
        }
        public string statestring
        {
            get { return _statestring; }
            set
            {
                if (this._statestring == value)
                    return;

                _statestring = value;
                PropChanged("statestring");
            }
        }
        public double position
        {
            get { return _position; }
            set
            {
                if (this._position == value)
                    return;

                _position = value;
                PropChanged("position");
            }
        }
        public string positionstring
        {
            get { return _positionstring; }
            set
            {
                if (this._positionstring == value)
                    return;

                _positionstring = value;
                PropChanged("PositionDurationFormated");
                PropChanged("positionstring");
            }
        }
        public double duration
        {
            get { return _duration; }
            set
            {
                if (this._duration == value)
                    return;

                _duration = value;
                PropChanged("duration");
            }
        }
        public string durationstring
        {
            get { return _durationstring; }
            set
            {
                if (this._durationstring == value)
                    return;

                _durationstring = value;
                PropChanged("PositionDurationFormated");
                PropChanged("durationstring");
            }
        }
        public string volumelevel
        {
            get { return _volumelevel; }
            set
            {
                if (this._volumelevel == value)
                    return;

                _volumelevel = value;
                PropChanged("volumelevel");

            }
        }
        public string muted
        {
            get { return _muted; }
            set
            {
                if (this._muted == value)
                    return;

                _muted = value;
                PropChanged("muted");
            }
        }
        public string playbackrate
        {
            get { return _playbackrate; }
            set
            {
                if (this._playbackrate == value)
                    return;

                _playbackrate = value;
                PropChanged("playbackrate");
            }
        }
        public string reloadtime
        {
            get { return _reloadtime; }
            set
            {
                if (this._reloadtime == value)
                    return;

                _reloadtime = value;
                PropChanged("reloadtime");
            }
        }
        public string PositionDurationFormated
        {
            get
            {
                return (string.IsNullOrEmpty(positionstring) || string.IsNullOrEmpty(durationstring)) ? "00:00:00/00:00:00" : positionstring + "/" + durationstring;
            }
        }

        public Status()
        {
            _WebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(_WebClient_DownloadStringCompleted);
        }

        public void GetStatusData(ConnectionProfile ConnectionProfile)
        {
            try
            {
                if (!string.IsNullOrEmpty(ConnectionProfile.Hostname) && !string.IsNullOrEmpty(ConnectionProfile.Port) && !_WebClient.IsBusy)
                    _WebClient.DownloadStringAsync(new Uri("http://" + ConnectionProfile.Hostname + ":" + ConnectionProfile.Port + "/variables.html"), UriKind.Absolute);
            }
            catch (UriFormatException ex) { }
        }

        private void _WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                // Prüft ob Error von 'Webclient' kam oder MPC-HC kein File geladen hat
                // --> Bricht ab wenn zutrifft
                if ((Regex.Match(e.Result, @"OnStatus\('Media\sPlayer\sClassic\sHome\sCinema")).Success)
                    return;

                SplitStatusData(e.Result);
            }
            catch
            {
            }
        }

        private void SplitStatusData(string RawStatusData)
        {
            // initialize regex class and match the 'variables.html' page
            string pattern = ".*<p\\sid=\"(.*)\">(.*)</p>";
            Regex SplitRegex = new Regex(pattern);
            MatchCollection matches = SplitRegex.Matches(RawStatusData);

            // save the data in the foreseen 'Status' class variables 
            foreach (Match match in matches)
            {
                switch (match.Groups[1].Value)
                {
                    case "filepatharg":
                        this.filepatharg = match.Groups[2].Value.ToString();
                        break;
                    case "filepath":
                        this.filepath = match.Groups[2].Value.ToString();
                        break;
                    case "filedirarg":
                        this.filedirarg = match.Groups[2].Value.ToString();
                        break;
                    case "filedir":
                        this.filedir = match.Groups[2].Value.ToString();
                        break;
                    case "state":
                        this.state = Convert.ToInt32(match.Groups[2].Value);
                        break;
                    case "statestring":
                        this.statestring = match.Groups[2].Value.ToString();
                        break;
                    case "position":
                        this.position = Convert.ToDouble(match.Groups[2].Value);
                        break;
                    case "positionstring":
                        this.positionstring = match.Groups[2].Value.ToString();
                        break;
                    case "duration":
                        this.duration = Convert.ToDouble(match.Groups[2].Value);
                        break;
                    case "durationstring":
                        this.durationstring = match.Groups[2].Value.ToString();
                        break;
                    case "volumelevel":
                        this.volumelevel = match.Groups[2].Value.ToString();
                        break;
                    case "muted":
                        this.muted = match.Groups[2].Value.ToString();
                        break;
                    case "playbackrate":
                        this.playbackrate = match.Groups[2].Value.ToString();
                        break;
                    case "reloadtime":
                        this.reloadtime = match.Groups[2].Value.ToString();
                        break;
                }
            }

            //get video name by last item in split array of filepath (\) or (/)
            string[] tempvariable = this.filepath.Split('\\');
            if (tempvariable.Length == 1)
            {
                tempvariable = this.filepath.Split('/');
                this.filename = tempvariable[tempvariable.Length - 1];
            }
            else
            {
                this.filename = tempvariable[tempvariable.Length - 1];
            }
        }

        public static string GetValidTimeStringFromMilliseconds(int milliseconds)
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(milliseconds);
            return interval.ToString(@"hh\:mm\:ss"); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
