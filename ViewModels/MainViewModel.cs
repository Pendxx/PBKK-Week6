using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using StudentRegistrationApp.Data;
using StudentRegistrationApp.Models;

namespace StudentRegistrationApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IStudentRepository _repository;

        // Navigation
        private string _currentView = "Dashboard"; // Dashboard, Mahasiswa, MataKuliah, Laporan, Pengaturan
        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // Dark Mode
        private bool _isDarkMode = true; // Level 3 shows a sleek dark theme SISMA
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (SetProperty(ref _isDarkMode, value))
                {
                    OnPropertyChanged(nameof(ThemeBackground));
                    OnPropertyChanged(nameof(ThemeSidebar));
                    OnPropertyChanged(nameof(ThemeCardBackground));
                    OnPropertyChanged(nameof(ThemeTextPrimary));
                    OnPropertyChanged(nameof(ThemeTextSecondary));
                    OnPropertyChanged(nameof(ThemeBorder));
                    OnPropertyChanged(nameof(ThemeDataGridHeader));
                    OnPropertyChanged(nameof(ThemeDataGridRowAlt));
                    OnPropertyChanged(nameof(ThemeInputBackground));
                }
            }
        }

        #region Dynamic Theme Colors for Binding
        public string ThemeBackground => IsDarkMode ? "#0F172A" : "#F1F5F9";
        public string ThemeSidebar => IsDarkMode ? "#1E293B" : "#FFFFFF";
        public string ThemeCardBackground => IsDarkMode ? "#1E293B" : "#FFFFFF";
        public string ThemeTextPrimary => IsDarkMode ? "#F8FAFC" : "#0F172A";
        public string ThemeTextSecondary => IsDarkMode ? "#94A3B8" : "#64748B";
        public string ThemeBorder => IsDarkMode ? "#334155" : "#E2E8F0";
        public string ThemeDataGridHeader => IsDarkMode ? "#334155" : "#F8FAFC";
        public string ThemeDataGridRowAlt => IsDarkMode ? "#192231" : "#F8FAFC";
        public string ThemeInputBackground => IsDarkMode ? "#0F172A" : "#FFFFFF";
        #endregion

        // Statistics Cards (Level 3)
        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            set => SetProperty(ref _totalStudents, value);
        }

        private int _totalPrograms = 5;
        public int TotalPrograms
        {
            get => _totalPrograms;
            set => SetProperty(ref _totalPrograms, value);
        }

        private int _totalLecturers = 8;
        public int TotalLecturers
        {
            get => _totalLecturers;
            set => SetProperty(ref _totalLecturers, value);
        }

        private int _totalBatches = 3;
        public int TotalBatches
        {
            get => _totalBatches;
            set => SetProperty(ref _totalBatches, value);
        }

        // Search & Data
        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    _ = FilterOrSearchStudentsAsync();
                }
            }
        }

        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<ProgramItem> Programs { get; } = new();
        public ObservableCollection<ProgramStatistic> ProgramStats { get; } = new();
        public ObservableCollection<ActivityLog> RecentActivities { get; } = new();

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set => SetProperty(ref _selectedStudent, value);
        }

        // Database Status & Configuration
        private string _databaseStatus = string.Empty;
        public string DatabaseStatus
        {
            get => _databaseStatus;
            set => SetProperty(ref _databaseStatus, value);
        }

        private string _sqlConnectionString = "Server=localhost;Database=StudentRegistrationDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public string SqlConnectionString
        {
            get => _sqlConnectionString;
            set => SetProperty(ref _sqlConnectionString, value);
        }

        // Form Modal State (Level 1 Dialog)
        private bool _isFormOpen;
        public bool IsFormOpen
        {
            get => _isFormOpen;
            set => SetProperty(ref _isFormOpen, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    OnPropertyChanged(nameof(FormTitle));
                }
            }
        }

        public string FormTitle => IsEditMode ? "Edit Data Mahasiswa" : "Form Registrasi Mahasiswa";

        private int _editingStudentId;
        private string _formNIM = string.Empty;
        public string FormNIM
        {
            get => _formNIM;
            set => SetProperty(ref _formNIM, value);
        }

        private string _formName = string.Empty;
        public string FormName
        {
            get => _formName;
            set => SetProperty(ref _formName, value);
        }

        private ProgramItem? _formSelectedProgram;
        public ProgramItem? FormSelectedProgram
        {
            get => _formSelectedProgram;
            set => SetProperty(ref _formSelectedProgram, value);
        }

        private DateTime? _formBirthDate = new DateTime(2003, 1, 1);
        public DateTime? FormBirthDate
        {
            get => _formBirthDate;
            set => SetProperty(ref _formBirthDate, value);
        }

        private string? _formAddress = string.Empty;
        public string? FormAddress
        {
            get => _formAddress;
            set => SetProperty(ref _formAddress, value);
        }

        private string? _formPhoneNumber = string.Empty;
        public string? FormPhoneNumber
        {
            get => _formPhoneNumber;
            set => SetProperty(ref _formPhoneNumber, value);
        }

        private string _formValidationMessage = string.Empty;
        public string FormValidationMessage
        {
            get => _formValidationMessage;
            set => SetProperty(ref _formValidationMessage, value);
        }

        // Delete Confirm Modal State (Level 2 Modal)
        private bool _isDeleteConfirmOpen;
        public bool IsDeleteConfirmOpen
        {
            get => _isDeleteConfirmOpen;
            set => SetProperty(ref _isDeleteConfirmOpen, value);
        }

        private Student? _studentPendingDelete;
        public Student? StudentPendingDelete
        {
            get => _studentPendingDelete;
            set => SetProperty(ref _studentPendingDelete, value);
        }

        // Commands
        public ICommand NavigateCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenAddFormCommand { get; }
        public ICommand OpenEditFormCommand { get; }
        public ICommand CloseFormCommand { get; }
        public ICommand ResetFormCommand { get; }
        public ICommand SaveStudentCommand { get; }
        public ICommand PromptDeleteCommand { get; }
        public ICommand CancelDeleteCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public MainViewModel() : this(new StudentRepository())
        {
        }

        public MainViewModel(IStudentRepository repository)
        {
            _repository = repository;
            DatabaseStatus = _repository.CurrentProviderName;
            SqlConnectionString = _repository.CurrentConnectionString;

            // Initialize commands
            NavigateCommand = new RelayCommand(param =>
            {
                if (param is string viewName)
                {
                    CurrentView = viewName;
                }
            });

            ToggleDarkModeCommand = new RelayCommand(() => IsDarkMode = !IsDarkMode);

            SearchCommand = new AsyncRelayCommand(FilterOrSearchStudentsAsync);
            RefreshCommand = new AsyncRelayCommand(LoadAllDataAsync);

            OpenAddFormCommand = new RelayCommand(OpenAddForm);
            OpenEditFormCommand = new RelayCommand(param =>
            {
                if (param is Student student) OpenEditForm(student);
                else if (SelectedStudent != null) OpenEditForm(SelectedStudent);
            });
            CloseFormCommand = new RelayCommand(() => IsFormOpen = false);
            ResetFormCommand = new RelayCommand(ResetForm);
            SaveStudentCommand = new AsyncRelayCommand(SaveStudentAsync);

            PromptDeleteCommand = new RelayCommand(param =>
            {
                if (param is Student s)
                {
                    StudentPendingDelete = s;
                    IsDeleteConfirmOpen = true;
                }
                else if (SelectedStudent != null)
                {
                    StudentPendingDelete = SelectedStudent;
                    IsDeleteConfirmOpen = true;
                }
            });
            CancelDeleteCommand = new RelayCommand(() =>
            {
                StudentPendingDelete = null;
                IsDeleteConfirmOpen = false;
            });
            ConfirmDeleteCommand = new AsyncRelayCommand(ExecuteDeleteStudentAsync);

            TestConnectionCommand = new AsyncRelayCommand(TestAndConnectDatabaseAsync);

            // Populate sample activity logs from image 2
            RecentActivities.Add(new ActivityLog { Title = "Ahmad Rizki ditambahkan", TimeAgo = "2 jam yang lalu", ActionType = "Add" });
            RecentActivities.Add(new ActivityLog { Title = "Data Siti Aisyah diperbarui", TimeAgo = "5 jam yang lalu", ActionType = "Update" });
            RecentActivities.Add(new ActivityLog { Title = "Budi Santoso dihapus", TimeAgo = "1 hari yang lalu", ActionType = "Delete" });

            // Initial load
            _ = LoadAllDataAsync();
        }

        public async Task LoadAllDataAsync()
        {
            try
            {
                // Load Programs
                var programs = await _repository.GetAllProgramsAsync();
                Programs.Clear();
                foreach (var p in programs) Programs.Add(p);
                TotalPrograms = Programs.Count;

                // Load Students
                await FilterOrSearchStudentsAsync();

                // Load Stats & Charts
                await LoadStatsAndChartsAsync();

                DatabaseStatus = _repository.CurrentProviderName;
            }
            catch (Exception ex)
            {
                DatabaseStatus = $"Error: {ex.Message}";
            }
        }

        private async Task FilterOrSearchStudentsAsync()
        {
            var students = await _repository.SearchStudentsAsync(SearchKeyword);
            Students.Clear();
            int index = 1;
            foreach (var s in students)
            {
                s.RowNumber = index++;
                Students.Add(s);
            }
            TotalStudents = Students.Count;
        }

        private async Task LoadStatsAndChartsAsync()
        {
            var stats = await _repository.GetStudentStatisticsByProgramAsync();
            ProgramStats.Clear();
            foreach (var st in stats)
            {
                ProgramStats.Add(st);
            }

            int count = await _repository.GetTotalStudentsCountAsync();
            TotalStudents = count;
        }

        private void OpenAddForm()
        {
            IsEditMode = false;
            _editingStudentId = 0;
            FormNIM = string.Empty;
            FormName = string.Empty;
            FormSelectedProgram = Programs.FirstOrDefault();
            FormBirthDate = new DateTime(2003, 1, 1);
            FormAddress = string.Empty;
            FormPhoneNumber = string.Empty;
            FormValidationMessage = string.Empty;
            IsFormOpen = true;
        }

        private void OpenEditForm(Student student)
        {
            IsEditMode = true;
            _editingStudentId = student.StudentId;
            FormNIM = student.NIM;
            FormName = student.Name;
            FormSelectedProgram = Programs.FirstOrDefault(p => p.ProgramId == student.ProgramId) 
                                 ?? Programs.FirstOrDefault(p => p.ProgramCode == student.ProgramCode)
                                 ?? Programs.FirstOrDefault();
            FormBirthDate = student.BirthDate;
            FormAddress = student.Address;
            FormPhoneNumber = student.PhoneNumber;
            FormValidationMessage = string.Empty;
            IsFormOpen = true;
        }

        private void ResetForm()
        {
            if (IsEditMode && SelectedStudent != null)
            {
                OpenEditForm(SelectedStudent);
            }
            else
            {
                FormNIM = string.Empty;
                FormName = string.Empty;
                FormSelectedProgram = Programs.FirstOrDefault();
                FormBirthDate = new DateTime(2003, 1, 1);
                FormAddress = string.Empty;
                FormPhoneNumber = string.Empty;
                FormValidationMessage = string.Empty;
            }
        }

        private async Task SaveStudentAsync()
        {
            FormValidationMessage = string.Empty;

            // Validations matching Level 1
            if (string.IsNullOrWhiteSpace(FormName))
            {
                FormValidationMessage = "Nama Mahasiswa wajib diisi!";
                return;
            }
            if (string.IsNullOrWhiteSpace(FormNIM))
            {
                FormValidationMessage = "NIM wajib diisi!";
                return;
            }
            if (FormSelectedProgram == null)
            {
                FormValidationMessage = "Pilih Program Studi!";
                return;
            }

            var student = new Student
            {
                StudentId = _editingStudentId,
                NIM = FormNIM.Trim(),
                Name = FormName.Trim(),
                ProgramId = FormSelectedProgram.ProgramId,
                BirthDate = FormBirthDate,
                Address = FormAddress?.Trim(),
                PhoneNumber = FormPhoneNumber?.Trim()
            };

            bool success;
            if (IsEditMode)
            {
                success = await _repository.UpdateStudentAsync(student);
                if (success)
                {
                    RecentActivities.Insert(0, new ActivityLog
                    {
                        Title = $"Data {student.Name} diperbarui",
                        TimeAgo = "Baru saja",
                        ActionType = "Update"
                    });
                }
            }
            else
            {
                success = await _repository.AddStudentAsync(student);
                if (success)
                {
                    RecentActivities.Insert(0, new ActivityLog
                    {
                        Title = $"{student.Name} ditambahkan",
                        TimeAgo = "Baru saja",
                        ActionType = "Add"
                    });
                }
            }

            if (success)
            {
                IsFormOpen = false;
                await LoadAllDataAsync();
            }
            else
            {
                FormValidationMessage = "Gagal menyimpan data! Pastikan NIM unik.";
            }
        }

        private async Task ExecuteDeleteStudentAsync()
        {
            if (StudentPendingDelete == null) return;

            string deletedName = StudentPendingDelete.Name;
            bool success = await _repository.DeleteStudentAsync(StudentPendingDelete.StudentId);
            IsDeleteConfirmOpen = false;

            if (success)
            {
                RecentActivities.Insert(0, new ActivityLog
                {
                    Title = $"{deletedName} dihapus",
                    TimeAgo = "Baru saja",
                    ActionType = "Delete"
                });
                await LoadAllDataAsync();
            }
        }

        private async Task TestAndConnectDatabaseAsync()
        {
            DatabaseStatus = "Menghubungkan ke SQL Server...";
            bool connected = await _repository.TryConnectSqlServerAsync(SqlConnectionString);
            if (connected)
            {
                DatabaseStatus = "Terhubung ke SQL Server (StudentRegistrationDB)";
                await LoadAllDataAsync();
            }
            else
            {
                DatabaseStatus = "Koneksi SQL Server gagal. Menggunakan SQLite (Local Fallback).";
            }
        }
    }
}
