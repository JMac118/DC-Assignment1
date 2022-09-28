using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Registry_DLL;
using Newtonsoft.Json;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : Window
    {
        protected int token;
        public List<ServiceDescription> ServiceDescriptions { get; set; }
        public List<ServiceDescription> SearchServiceDescriptions { get; set; }

        private ServiceDescription curServiceDescription;



        public Services(int token)
        {
            InitializeComponent();
            this.token = token;

            TestContainer.Visibility = Visibility.Hidden;
            SearchTextBox.Visibility = Visibility.Hidden;
            SearchButton.Visibility = Visibility.Hidden;

            Task.Run(() => GetAllServices());

        }

        private void LookupButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Visibility = Visibility.Visible;
            SearchButton.Visibility = Visibility.Visible;
        }

        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Visibility = Visibility.Hidden;
            SearchButton.Visibility = Visibility.Hidden;
            SearchTextBox.Text = "";

            Task.Run(() => GetAllServices());

            //ServiceListView.ItemsSource = ServiceDescriptions;
        }

        private async void GetAllServices()
        {
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = true));

            Thread.Sleep(1000);
            RestClient restClient = new RestClient("https://localhost:44329/");
            RestRequest request = new RestRequest("api/registry/AllServices/" + token);
            RestResponse restResponse = await restClient.ExecuteGetAsync(request);

            if (restResponse.IsSuccessStatusCode)
            {
                // Do the thing with the string output
                ServiceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(restResponse.Content);
                ServiceListView.Dispatcher.Invoke(new Action(() => ServiceListView.ItemsSource = ServiceDescriptions));
            }
            else
            {
                Exception exc = new Exception(restResponse.Content);
                // Error message output
                Console.WriteLine(exc.Message);
            }
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = false));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchStr = SearchTextBox.Text;
            if (searchStr.Length != 0)
            {
                Task.Run(() => SearchAsync(searchStr));
            }
        }

        private async void SearchAsync(string searchStr)
        {
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = true));
            Thread.Sleep(1000);

            RestClient restClient = new RestClient("https://localhost:44329/");
            RestRequest request = new RestRequest("api/registry/Search/" + token + "/" + searchStr);
            RestResponse restResponse = await restClient.ExecutePostAsync(request);

            // It hated using SearchServiceDescription and didn't want to update it no matter what.
            //SearchServiceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(restResponse.Content);
            //ServiceListView.Dispatcher.Invoke(new Action(() => ServiceListView.ItemsSource = ServiceDescriptions));

            List<ServiceDescription> descriptions = new List<ServiceDescription>();
            descriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(restResponse.Content);
            ServiceListView.Dispatcher.Invoke(new Action(() => ServiceListView.ItemsSource = descriptions));

            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = false));

        }

        private void TryButton_Click(object sender, RoutedEventArgs e)
        {
            int numOp = 0;
            Button button = sender as Button;
            Button testButton = new Button();
            curServiceDescription = button.DataContext as ServiceDescription;
            var container = InputContainer;

            container.Children.Clear();
            try
            {
                numOp = int.Parse(curServiceDescription.Num_Operands);
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Invalid Num_Operands value for " + curServiceDescription.Name);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Error: " + curServiceDescription.Name + " has empty Num_Operands value");
            }

            for (int i = 0; i < numOp; i++)
            {
                var stackPanel = new StackPanel { Name= "Stack" + i, Orientation = Orientation.Horizontal, Height = 30 };
                stackPanel.Children.Add(new Label { Name= "TextBox" + i, Content = "Input " + (i + 1) });
                stackPanel.Children.Add(new TextBox { Text = "", Width = 30, Height=25});
                container.Children.Add(stackPanel);
            }

            TestContainer.Visibility = Visibility.Visible;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string reqStr = token + "/" + curServiceDescription.Name + "/";

            TextBox textbox = null;
            StackPanel parent = null;

            foreach(var control in InputContainer.Children)
            {
                if (control is StackPanel)
                {
                    parent = (StackPanel)control;
                    foreach(Control child in parent.Children)
                    {
                        if(child is TextBox)
                        {
                            textbox = (TextBox)child;
                            if (textbox != null)
                            {
                                try
                                {
                                    reqStr += int.Parse(textbox.Text) + "/";
                                }
                                catch(FormatException)
                                {
                                    MessageBox.Show("Invalid input: inputs can only be interger");
                                }
                                catch(ArgumentNullException)
                                {
                                    MessageBox.Show("No inputs entered");
                                }
                                catch(OverflowException)
                                {
                                    MessageBox.Show("Invalid input: input is too large");
                                }
                            }

                        }
                    }
                }    
                    
            }

            Task.Run(() => TestAsync(reqStr));
        }

        private async void TestAsync(string reqStr)
        {
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = true));
            Thread.Sleep(1000);

            RestClient restClient = new RestClient(curServiceDescription.API_Endpoint);
            RestRequest request = new RestRequest(reqStr);
            RestResponse restResponse = await restClient.ExecuteGetAsync(request);

            if (restResponse.IsSuccessStatusCode)
            {
                // Do the thing with the string output         
                ResultTextBlock.Dispatcher.Invoke(new Action(() =>ResultTextBlock.Text = restResponse.Content));
            }
            ProgressBar.Dispatcher.Invoke(new Action(() => ProgressBar.IsIndeterminate = false));
        }
    }
}
