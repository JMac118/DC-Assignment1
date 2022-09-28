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
       


        public Services(int token)
        {
            InitializeComponent();
            this.token = token;

            SearchTextBox.Visibility = Visibility.Hidden;
            SearchButton.Visibility = Visibility.Hidden;

            RestClient restClient = new RestClient("https://localhost:44329/");
            RestRequest request = new RestRequest("api/registry/AllServices/" + token);
            RestResponse restResponse = restClient.ExecuteGet(request);

            if (restResponse.IsSuccessStatusCode)
            {
                // Do the thing with the string output
               ServiceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(restResponse.Content);
            }
            else
            {
                Exception exc = new Exception(restResponse.Content);
                // Error message output
                // Console.WriteLine(exc.Message);
            }

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
            ServiceListView.ItemsSource = ServiceDescriptions;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchStr = SearchTextBox.Text;
            RestClient restClient = new RestClient("https://localhost:44329/");
            RestRequest request = new RestRequest("Search/" + token + "/" + searchStr);
            RestResponse restResponse = restClient.ExecutePost(request);

            SearchServiceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(restResponse.Content); 
            ServiceListView.ItemsSource = SearchServiceDescriptions;
        }
    }
}
