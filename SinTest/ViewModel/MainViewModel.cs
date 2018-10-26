using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyoSensor.Model;
using MyoSensor.Services;

namespace MyoSensor
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;

    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    class MainViewModel : INotifyPropertyChanged, IDisposable
    {

        #region Variables
        public string TextMy { get; set; } = "Мио";
        private const int UpdateInterval = 33;
        private int maxNumberOfPoints = 1000;
        private ObservableCollection<ProfileModel> profileList { get; set; }
        private ProfileModel selectedProfile = null;
        private SessionModel selectedSession;
        private ObservableCollection<SessionModel> sessionList { get; set; }
        #endregion

        #region Object Variables
        private DataModel SignalGen = new DataModel();
        private bool disposed;
        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();
        private int numberOfSeries;
        #endregion


        #region Setters
        public PlotModel PlotModel { get; private set; }
        public int TotalNumberOfPoints { get; private set; }
        public int MaxTimeOnGraph
        {
            get
            {
                return maxNumberOfPoints;
            }
            set
            {
                maxNumberOfPoints = value;
            }
        }

        public string SelectedProfile
        {
            get
            {
                {
                    if (selectedProfile == null)
                        return "Не авторизирован";
                    else
                        return selectedProfile.FullName;
                }
            }
            set
            {
                foreach (var profile in profileList)
                {
                    if (profile.FullName == value)
                    {
                        selectedProfile = profile;
                        string date = DateTime.Now.ToString();
                        sessionList = SessionModel.GetSessions(profile.Id);
                        RaisePropertyChanged("SessionsItem");
                    }
                }
            }
        }

        public string SelectedSession
        {
            get { return selectedSession.Id.ToString(); }
            set {
                if (selectedProfile != null)
                {
                    ulong idSession = Convert.ToUInt64(value);
                    foreach (var session in sessionList)
                    {
                        if (session.Id == idSession)
                        {
                            selectedSession = session;
                            LoadSession();
                        }
                    }
                }

            }
        }

        public ObservableCollection<string> ComPorts
        {
            get { return SignalGen.ComPorts; }
        }

        public bool CheckedConnected
        {
            get { return SignalGen.StateConnected; }
            set { SignalGen.StateConnected = value; }
        }

        public string NamePort
        {
            get { return SignalGen.NamePort; }
            set { SignalGen.NamePort = value; }
        }

        public ObservableCollection<string> ProfilesItem
        {
            get
            {

                ObservableCollection<string> profilesFullName = new ObservableCollection<string>();
                foreach (var item in profileList)
                {
                    profilesFullName.Add(item.FullName);
                }
                return profilesFullName;
            }
            private set { }
        }

        public ObservableCollection<string> SessionsItem
        {
            get
            {
                if (sessionList != null)
                {
                    ObservableCollection<string> sessionsItemId = new ObservableCollection<string>();
                    foreach (var item in sessionList)
                    {
                        sessionsItemId.Add(item.Id.ToString());
                    }
                    return sessionsItemId;
                }
                else
                {
                    return null;
                }
            }
            private set { }
        }

        #endregion

        #region Constructors
        public MainViewModel()
        {
            this.timer = new Timer(OnTimerElapsed);
            SetupModel();

            profileList = new ObservableCollection<ProfileModel>();
            foreach (var item in ProfileModel.GetProfiles())
            {
                profileList.Add(item);
            }
        }
        #endregion

        #region Events
        private void StartDraw()
        {
            SignalGen.StartReceive();
            SetupModel();
            this.timer.Change(0, UpdateInterval);
        }

        private void StopDraw()
        {
            SignalGen.StopReceive();
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.Update();
        }

        private void SaveSession()
        {
            if (selectedProfile == null)
                return;

            DateTime date = DateTime.Now;
            ulong idSession = ulong.Parse(date.ToString("yyMMddHHmmss"));

            List<DataPoint> points = ((LineSeries)PlotModel.Series[0]).Points;
            List<double> dataOnGraph = new List<double>();
            foreach (DataPoint point in points)
            {
                dataOnGraph.Add(point.Y);
            }
            while (SignalGen.StateReceive) ;
            SignalGen.Data = dataOnGraph;
            LoaderModel.SaveSession(selectedProfile.Id, idSession, SignalGen.Data);
            // sessionList = SessionModel.GetSessions(selectedProfile.Id);
            RaisePropertyChanged("SessionsItem");
            SessionModel newSession = new SessionModel
            {
                Id = idSession,
                DateSession = date.ToString(),
                Сomment = ""
            };
            SessionModel.SaveSession(selectedProfile.Id , newSession);
            selectedSession = newSession;
            sessionList = SessionModel.GetSessions(selectedProfile.Id);
            // sessionList.Add(newSession);
            RaisePropertyChanged("SessionsItem");
        }

        private void CreatProfile()
        {
            LoginWindow loginWIndow = new LoginWindow();
            var result = loginWIndow.ShowDialog();
            if (result == true)
            {
                string FullName = loginWIndow.FullName;

                int id = GetNewId();
                ProfileModel newProfile = new ProfileModel();
                newProfile.Id = id;
                newProfile.FullName = FullName;
                ProfileModel.SaveProfile(newProfile);
                profileList.Add(newProfile);
                selectedProfile = newProfile;
                selectedSession = null;
                if (sessionList != null)
                    sessionList.Clear();
                RaisePropertyChanged("SessionsItem");
                RaisePropertyChanged("ProfilesItem");
                RaisePropertyChanged("SelectedProfile");
            }

        }

        #endregion

        #region Commands
        public ICommand StopCommand
        {
            get { return new CommandHandler(() => StopDraw(), true); }
        }

        public ICommand StartCommand
        {
            get { return new CommandHandler(() => StartDraw(), true); }
        }

        public ICommand CreatProfileCommand
        {
            get { return new CommandHandler(() => CreatProfile(), true); }
        }

        public ICommand SaveProfileCommand
        {
            get { return new CommandHandler(() => SaveSession(), true); }
        }
        
        #endregion

        #region GraphicInit
        private void SetupModel()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 3000 });

            this.numberOfSeries = 1;

            for (int i = 0; i < this.numberOfSeries; i++)
            {
                PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid });
            }

            this.watch.Start();

            this.RaisePropertyChanged("PlotModel");


        }

        public event PropertyChangedEventHandler PropertyChanged;
        // public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        protected void RaisePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Draw
        private void OnTimerElapsed(object state)
        {
            lock (this.PlotModel.SyncRoot)
            {
                this.Update();
            }

            this.PlotModel.InvalidatePlot(true);
        }

        private void Update()
        {
            double t = this.watch.ElapsedMilliseconds * 0.001;
            int n = 0;
            for (int i = 0; i < PlotModel.Series.Count; i++)
            {
                var s = (LineSeries)PlotModel.Series[i];
                List<double> data;
                if (SignalGen.StateReceive == true)
                {
                    data = SignalGen.GetNewData();
                }
                else
                {
                    data = SignalGen.Data;
                    s.Points.Clear();
                }
                for (int j = 0; j < data.Count(); j++)
                {
                    double x = s.Points.Count > 0 ? s.Points[s.Points.Count - 1].X + 1 : 0;

                    if (SignalGen.StateReceive == true)
                    {
                        if (s.Points.Count >= maxNumberOfPoints)
                            s.Points.RemoveAt(0);
                    }
                    double y = data[j];
                    s.Points.Add(new DataPoint(x, y));
                }
                n += s.Points.Count;
            }

            if (SignalGen.StateReceive == false)
            {
                try
                {
                    PlotModel.Axes[0].Reset();
                    PlotModel.Axes[1].Reset();

                    PlotModel.Axes[0].Minimum = 0;
                    PlotModel.Axes[0].Maximum = 3000;

                    PlotModel.Axes[1].Minimum = 0;
                    PlotModel.Axes[1].Maximum = n;
                    PlotModel.InvalidatePlot(true);
                }
                catch
                {

                }
            }

            if (this.TotalNumberOfPoints != n)
            {
                this.TotalNumberOfPoints = n;
                this.RaisePropertyChanged("TotalNumberOfPoints");
            }

        }
        #endregion

        #region Other

        private int GetNewId()
        {
            int newId = 0;
            int maxId = 0;

            for (int i = 0; i < profileList.Count; i++)
            {
                if (profileList[i].Id > maxId)
                    maxId = profileList[i].Id;
            }

            for (int i = 1; i < maxId; i++)
            {
                bool state_search = false;
                for (int j = 0; j < profileList.Count; j++)
                {
                    if (profileList[j].Id == i)
                    {
                        state_search = true;
                        break;
                    }
                }

                if (state_search == false)
                {
                    newId = i;
                    return newId;
                }

            }

            return maxId + 1;
        }

        private void LoadSession()
        {

            SignalGen.Data = LoaderModel.LoadSession(selectedProfile.Id, selectedSession.Id);
            Update();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.timer.Dispose();
                }
            }

            this.disposed = true;
        }
        #endregion

        #region CommandHandler
        public class CommandHandler : ICommand
        {
            private Action _action;
            private bool _canExecute;
            public CommandHandler(Action action, bool canExecute)
            {
                _action = action;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                _action();
            }
        }
        #endregion

    }
}
