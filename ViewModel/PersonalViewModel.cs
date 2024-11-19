using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module07dataaccess.Model;
using Module07dataaccess.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mysqlx.Crud;


namespace Module07dataaccess.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;
        public ObservableCollection<Personal> PersonalList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private Personal _selectedPersonal;
        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalName = _selectedPersonal.Name;
                    NewPersonalAddress = _selectedPersonal.Address;
                    NewPersonalEmail = _selectedPersonal.email;
                    NewPersonalContactNo = _selectedPersonal.ContactNo;
                    IsPersonSelected = true;
                }
                else
                {
                    IsPersonSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
            }
        }

        // New Personal entry for name, gender, contact no
        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalAddress;
        public string NewPersonalAddress
        {
            get => _newPersonalAddress;
            set
            {
                _newPersonalAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalEmail;
        public string NewPersonalEmail
        {
            get => _newPersonalEmail;
            set
            {
                _newPersonalEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand SelectedPersonalCommand { get; }
        public ICommand DeletePersonalCommand { get; }
        public ICommand UpdatePersonalCommand { get; }
        public ICommand SearchCommand { get; }

        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();

            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async () => await AddPerson());
            SelectedPersonalCommand = new Command<Personal>(person => SelectedPersonal = person);
            DeletePersonalCommand = new Command(async () => await DeletePersonal(), () => SelectedPersonal != null);
            UpdatePersonalCommand = new Command(async () => await UpdatePersonal(), () => SelectedPersonal != null);
            SearchCommand = new Command(async () => await SearchPersonals());

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading Personal Data...";
            try
            {
                var personals = await _personalService.GetAllPersonalsAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Data loaded successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddPerson()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalAddress) ||
                string.IsNullOrWhiteSpace(NewPersonalEmail) || string.IsNullOrWhiteSpace(NewPersonalContactNo))
            {
                StatusMessage = "Please fill in all fields before adding.";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new employee...";

            try
            {
                var newPerson = new Personal
                {
                    Name = NewPersonalName,
                    Address = NewPersonalAddress,
                    email = NewPersonalEmail,
                    ContactNo = NewPersonalContactNo
                };
                var isSuccess = await _personalService.AddPersonalAsync(newPerson);
                if (isSuccess)
                {
                    NewPersonalName = string.Empty;
                    NewPersonalAddress = string.Empty;
                    NewPersonalEmail = string.Empty;
                    NewPersonalContactNo = string.Empty;
                    StatusMessage = "New Person added successfully.";
                    await LoadData();
                }
                else
                {
                    StatusMessage = "Failed to add the new person.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add the new person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedPersonal.Name}?", "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting Employee...";
            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.EmployeeID);
                StatusMessage = success ? "Employee removed successfully." : "Failed to remove employee.";
                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to remove employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdatePersonal()
        {
            if (SelectedPersonal == null) return;

            IsBusy = true;
            StatusMessage = "Updating Employee...";
            try
            {
                var updatedPerson = new Personal
                {
                    EmployeeID = SelectedPersonal.EmployeeID,
                    Name = NewPersonalName,
                    Address = NewPersonalAddress,
                    email = NewPersonalEmail,
                    ContactNo = NewPersonalContactNo
                };
                var success = await _personalService.UpdatePersonalAsync(updatedPerson);
                StatusMessage = success ? "Employee updated successfully." : "Failed to update employee.";
                if (success)
                {
                    await LoadData();
                    SelectedPersonal = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to update employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchPersonals()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Searching...";
            try
            {
                var results = await _personalService.SearchPersonalsAsync(SearchTerm);
                PersonalList.Clear();
                foreach (var person in results)
                {
                    PersonalList.Add(person);
                }
                StatusMessage = "Search completed.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to search: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
