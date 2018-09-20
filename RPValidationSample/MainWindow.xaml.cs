using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reactive.Bindings;

namespace RPValidationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly DummyModel _model = new DummyModel();

        public event PropertyChangedEventHandler PropertyChanged;

        [Required(ErrorMessage = "Input is required.")]
        public ReactiveProperty<string> Input { get; }

        public ReadOnlyReactivePropertySlim<string> ErrorMessage { get; }

        public ReactiveCommand SubmitCommand { get; }

        public MainWindowViewModel()
        {
            Input = new ReactiveProperty<string>()
                .SetValidateAttribute(() => Input)
                .SetValidateNotifyError(_ => _model.Input.GetErrors(nameof(ReactiveProperty<string>.Value)));
            _model.Input.ObserveErrorChanged.Subscribe(_ => Input.ForceValidate());

            ErrorMessage = Input.ObserveErrorChanged
                .Select(x => x?.OfType<string>().FirstOrDefault())
                .ToReadOnlyReactivePropertySlim();

            SubmitCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    _model.Input.Value = Input.Value;
                });
        }
    }

    public class DummyModel
    {
        [Range(0, 10, ErrorMessage = "0-10")]
        public ReactiveProperty<string> Input { get; }

        public DummyModel()
        {
            Input = new ReactiveProperty<string>()
                .SetValidateAttribute(() => Input);
        }
    }
}
