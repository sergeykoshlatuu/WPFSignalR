using ChatClient.Models;
using ChatClient.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace ChatClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructors
        public MainWindowViewModel(IChatService chatService)
        {
            _chatService = chatService;
            _chatService.SendMessage += SendMessage;
            _chatService.AddMessageToGroup += AddMessageToGroup;
            _chatService.ParticipantLoggedIn += ParticipantLogin;
            _chatService.ParticipantLoggedOut += ParticipantDisconnection;
            _chatService.ParticipantDisconnected += ParticipantDisconnection;
            _chatService.ParticipantReconnected += ParticipantReconnection;
            _chatService.ParticipantTyping += ParticipantTyping;
            Chatter = new ObservableCollection<ChatMessage>() { new ChatMessage() { Author = "Admin", Message = "Welcome to SignalR Chat" } };

            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            IsGroupChatVisible = false;
            IsUserChatVisible = false;
        }
        #endregion

        #region variable and property
        private TaskFactory ctxTaskFactory;

        private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> Participants
        {
            get { return _participants; }
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        private Participant _selectedParticipant;
        public Participant SelectedParticipant
        {
            get { return _selectedParticipant; }
            set
            {
                _selectedParticipant = value;
                _selectedGroup = null;
                IsUserChatVisible = true;
                IsGroupChatVisible = false;
                if (SelectedParticipant.HasSentNewMessage) SelectedParticipant.HasSentNewMessage = false;
                OnPropertyChanged();
                OnPropertyChanged("SelectedGroup");
            }
        }

        private string _title = "SignarR Chat";
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set
            {
                _groupName = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private UserModes _userMode;
        public UserModes UserMode
        {
            get { return _userMode; }
            set
            {
                _userMode = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ChatMessage> _chatter;
        public ObservableCollection<ChatMessage> Chatter
        {
            get { return _chatter; }
            set
            {
                _chatter = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Group> _groups=new ObservableCollection<Group>();
        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }

        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                _selectedParticipant = null;
                IsGroupChatVisible = true;
                IsUserChatVisible = false;
                if (SelectedGroup.HasSentNewMessage) SelectedGroup.HasSentNewMessage = false;
                OnPropertyChanged();
                OnPropertyChanged("SelectedParticipant");
            }
        }

        private bool _isGroupChatVisible;
        public bool IsGroupChatVisible
        {
            get { return _isGroupChatVisible; }
            set
            {
                _isGroupChatVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isUserChatVisible;
        public bool IsUserChatVisible
        {
            get { return _isUserChatVisible; }
            set
            {
                _isUserChatVisible = value;
                OnPropertyChanged();
            }
        }

        private IChatService _chatService;

        #endregion

        #region commands
        private DelegateCommand _sendMessage;
        public DelegateCommand SendMessageCommand =>
            _sendMessage ?? (_sendMessage = new DelegateCommand(ExecuteSendMessage));

        private DelegateCommand _login;
        public DelegateCommand Login =>
            _login ?? (_login = new DelegateCommand(ExecuteLogin));

        private DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand =>
             _connectCommand ?? (_connectCommand = new DelegateCommand(ExecuteConnectCommand));

        private DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand =>
            _logoutCommand ?? (_logoutCommand = new DelegateCommand(ExecuteLogoutCommand));

        #endregion

        #region methods
        private void ExecuteSendMessage()
        {
            if (Message != string.Empty && Message!=""&&Message!=null)
            {
                if (IsUserChatVisible)
                {
                    try
                    {
                        var recepient = _selectedParticipant.Name;
                        _chatService.SendMessageAsync(recepient, UserName, Message);
                        ChatMessage msg = new ChatMessage
                        {
                            Author = UserName,
                            Message = Message,
                            Time = DateTime.Now,
                            IsOriginMaster = true
                        };
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            SelectedParticipant.Chatter.Add(msg);
                        });
                        Message = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (IsGroupChatVisible)
                {
                    try
                    {
                        var recepient = SelectedGroup.Name;
                        _chatService.SendMessageToGroupAsync(recepient, UserName, Message);
                        ChatMessage msg = new ChatMessage
                        {
                            Author = UserName,
                            Message = Message,
                            Time = DateTime.Now,
                            IsOriginMaster = true
                        };
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            SelectedGroup.Chatter.Add(msg);
                        });
                        Message = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            }

        private void SendMessage(string name, string senders, string message)
        {
            ChatMessage cm = new ChatMessage { Author = name, Message = message, Time = DateTime.Now };
            var sender = _participants.Where((u) => string.Equals(u.Name, name)).FirstOrDefault();
            ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

            if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
            {
                ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
            }
        }

        private void AddMessageToGroup(string roomname, string senders, string message)
        {
            if (senders != UserName)
            {
                ChatMessage cm = new ChatMessage { Author = senders, Message = message, Time = DateTime.Now };
                var sender = _groups.Where((u) => string.Equals(u.Name, roomname)).FirstOrDefault();
                ctxTaskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

                if (!(SelectedGroup != null && sender.Name.Equals(SelectedGroup.Name)))
                {
                    ctxTaskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
                }
            }
        }

       

        private async void ExecuteLogin()
        {
            try
            {
                List<User> users = new List<User>();
                users = await _chatService.LoginAsync(_userName);


                if (users != null&& CanLogin())
                {
                    Participants = new ObservableCollection<Participant>();
                    users.ForEach(u => Participants.Add(new Participant { Name = u.Name, IsLoggedIn = u.IsLogedIn }));
                    UserMode = UserModes.Chat;
                    IsLoggedIn = true;
                    
                    List<string> groups = new List<string>();
                    groups = await _chatService.JoinRooms();
                    if (groups != null)
                    {
                        Groups = new ObservableCollection<Group>();
                        for (int i = 0; i < groups.Count; i++)
                        {
                            Groups.Add(new Group { Name = groups[i] });
                            Groups[i].Chatter.Add(new ChatMessage()
                            { Author="Admin", Time=DateTime.Now,
                                Message = String.Format("Its chat for {0}", groups[i]) });
                        }
                    }
                }

               

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName) && UserName.Length >= 2 && IsConnected;
        }

       

        private async void ExecuteConnectCommand()
        {
            try
            {
                await _chatService.ConnectAsync();
                IsConnected = true;
            }
            catch (Exception) {  }
        }

       

      private async void ExecuteLogoutCommand()
        {
            try
            {
                await _chatService.LogoutAsync();
                UserMode = UserModes.Login;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ParticipantReconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null) person.IsLoggedIn = true;
        }

        private void ParticipantDisconnection(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null) person.IsLoggedIn = false;
        }

        private void ParticipantLogin(User u)
        {
            var ptp = Participants.FirstOrDefault(p => string.Equals(p.Name, u.Name));
            if (_isLoggedIn && ptp == null)
            {
                ctxTaskFactory.StartNew(() => Participants.Add(new Participant
                {
                    Name = u.Name
                })).Wait();
                return;
            }
            Participants.FirstOrDefault(p => string.Equals(p.Name, u.Name)).IsLoggedIn = true;
        }

        private void ParticipantTyping(string name)
        {
            var person = Participants.Where((p) => string.Equals(p.Name, name)).FirstOrDefault();
            if (person != null && !person.IsTyping)
            {
                person.IsTyping = true;
                Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(t => person.IsTyping = false);
            }
        }
        #endregion
    }
}
