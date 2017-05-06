using System;
using System.Collections.Generic;
using System.Linq;
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
using Npgsql;


// upEc_4-9Sxts9X9yUURRBw

namespace Milestone1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        table_friends tableFriends = new table_friends();
        double newRating = 0.0;
        string selectedCategory = "";
        string selectedCategoryToDelete = "";
        int globalIntOpen = 0;

        table_search_results selectedBusiness = new table_search_results();

        // Friends table object class
        public class table_friends
        {
            public string name { get; set; }
            public string avg_stars { get; set; }
            public string yelping_since { get; set; }
        }

        // Tips table object class
        public class table_tips
        {
            public string username { get; set; }
            public string business { get; set; }
            public string city { get; set; }
            public string text { get; set; }
        }
        public class table_search_results
        {
            public string BusinessName { get; set; }
            public string Address { get; set; }
            public string NumberofTips { get; set; }
            public string TotalCheckins { get; set; }
            public string selectedBusinessID { get; set; }
        }

        public class tips_results
        {
            public string Name { get; set; }
            public string Date { get; set; }
            public string Likes { get; set; }
            public string Text { get; set; }
        }

        public string namefromotherform
        {
            get { return selectedBusiness.BusinessName; }
            set { selectedBusiness.BusinessName = value; }
        }

        // Constructor
        public MainWindow()
        {
            // :O OOP
            InitializeComponent();
            findUserID();
            addColumns2Grid();
            addColumns2TipsGrid();
            addStates();
            addDaysOfWeek();
            addOpeningTimeAndClosingTime();
            addColumns2SearchResultsGrid();
        }

        private string buildConnString()
        {
            //need to double check with that
            return "Host=localhost; Username=postgres; Password=; Database=Milestone1DB";
        }

        public void findUserID()
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct userid FROM users";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserIDBox.Text = reader.GetString(0); ;//.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double buffer = 0;

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct * FROM users WHERE userid = \'" + UserIDBox.Text + "\'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name_box.Text = reader.GetString(2);
                            buffer = reader.GetDouble(1);
                            stars_box.Text = buffer.ToString();
                            fan_box.Text = reader.GetDouble(5).ToString();
                            funny_box.Text = reader.GetDouble(6).ToString();
                            cool_box.Text = reader.GetDouble(7).ToString();
                            useful_box.Text = reader.GetDouble(8).ToString();
                            yelping_box.Text = reader.GetString(4);
                        }
                    }
                }
                conn.Close();

            }

            CreateFriendsTable();

            // For the right bottom box (Tips)
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    //SELECT users.name , users.averagestars, users.yelpingsince, users.userid
                    //FROM USERS
                    //WHERE users.userid in (SELECT friends.friendid FROM users, friends where 'SHWFXq2xACjJUuoPAvztAA' = users.userid AND 'SHWFXq2xACjJUuoPAvztAA' = friends.userid)
                    cmd.Connection = conn;
                    //name of the text box
                    cmd.CommandText = "SELECT tips.date, users.name, tips.text, Businesses.city, Businesses.name FROM USERS, Businesses, Tips WHERE users.userid in (SELECT friends.friendid FROM users, friends where '" + UserIDBox.Text + "' = users.userid AND '" + UserIDBox.Text + "' = friends.userid) AND tips.userid = users.userid AND Businesses.businessid = tips.businessid Group by tips.date, users.name, tips.text, businesses.city, businesses.name Order by tips.date DESC";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipsGrid.Items.Add(new table_tips() { username = reader.GetString(1), business = reader.GetString(2), city = reader.GetString(3), text = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }
        }

        public void CreateFriendsTable()
        {
            // For the left bottom box (Friends)
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    //SELECT users.name , users.averagestars, users.yelpingsince, users.userid
                    //FROM USERS
                    //WHERE users.userid in (SELECT friends.friendid FROM users, friends where 'SHWFXq2xACjJUuoPAvztAA' = users.userid AND 'SHWFXq2xACjJUuoPAvztAA' = friends.userid)
                    cmd.Connection = conn;
                    //name of the text box
                    cmd.CommandText = "SELECT users.name, users.averagestars, users.yelpingsince, users.userid FROM USERS WHERE users.userid in (SELECT friends.friendid FROM users, friends where '" + UserIDBox.Text + "' = users.userid AND '" + UserIDBox.Text + "' = friends.userid)";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User_friends_List.Items.Add(new table_friends() { name = reader.GetString(0), avg_stars = reader.GetDouble(1).ToString(), yelping_since = reader.GetString(2) });
                        }
                    }
                }
                conn.Close();
            }
        }

        // Set up BusinessGrid
        public void addColumns2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("name");
            col1.Width = 100;
            User_friends_List.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Avg Stars";
            col2.Binding = new Binding("avg_stars");
            col2.Width = 100;
            User_friends_List.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Yelping Since";
            col3.Binding = new Binding("yelping_since");
            col3.Width = 99;
            User_friends_List.Columns.Add(col3);
        }

        // Set up TipsGrid
        public void addColumns2TipsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Username";
            col1.Binding = new Binding("username");
            col1.Width = 100;
            TipsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Business";
            col2.Binding = new Binding("business");
            col2.Width = 200;
            TipsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            col3.Width = 125;
            TipsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Text";
            col4.Binding = new Binding("text");
            col4.Width = 350;
            TipsGrid.Columns.Add(col4);
        }
        public void addColumns2SearchResultsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "BusinessName";
            col1.Binding = new Binding("BusinessName");
            col1.Width = 100;
            Search_results_Grid.Columns.Add(col1);


            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Address";
            col2.Binding = new Binding("Address");
            col2.Width = 200;
            Search_results_Grid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "#ofTips";
            col3.Binding = new Binding("NumberofTips");
            col3.Width = 125;
            Search_results_Grid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "TotalCheckins";
            col4.Binding = new Binding("TotalCheckins");
            col4.Width = 350;
            Search_results_Grid.Columns.Add(col4);
        }


        // Remove friend button click event
        private void remove_friend_button_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    var userToDeleteUID = findUser();    // Find the user because we don't have their uid on hand

                    //DELETE FROM 
                    cmd.Connection = conn;
                    //name of the text box
                    cmd.CommandText = "DELETE FROM friends WHERE friends.userid = '" + UserIDBox.Text + "' AND friends.friendid = '" + userToDeleteUID + "'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                conn.Close();
            }
            User_friends_List.Items.Clear();
            CreateFriendsTable();
        }

        // It will query Users and returns the UID of the selected friend
        public string findUser()
        {
            var uidToDelete = "";

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    //DELETE FROM 
                    cmd.Connection = conn;
                    //name of the text box                                                                                     v YES they have different names v                             v YES they have different names v
                    if (tableFriends.yelping_since != null)
                    {
                        var cleanedUpYelpingSince = tableFriends.yelping_since.Trim();
                        cmd.CommandText = "SELECT userid FROM Users WHERE Users.name = '" + tableFriends.name + "' AND Users.averagestars = '" + tableFriends.avg_stars + "' AND Users.yelpingsince = '" + cleanedUpYelpingSince + "'";
                        // hmm given "name", "avg_stars", and "yelping_since" we have to query Friends and delete a tuple. Friends has the format (userID, friendID), one of which we don't have
                        // Query Users first to find the friendID.
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uidToDelete = reader.GetString(0);
                        }
                    }
                }
                conn.Close();

                return uidToDelete;
            }
        }

        // Selected a friend from friend table (bottom left)
        private void User_friends_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tableFriends = (table_friends)User_friends_List.SelectedItem; // Select it and read info for deletion
        }

        // Rate friend button click event
        private void rate_friend_button_Click(object sender, RoutedEventArgs e)
        {
            // grab the current rating and average it with the new
            double average_stars = (Convert.ToDouble(tableFriends.avg_stars) + newRating) / 2; // Average the existing rating with the new one

            // update the entry!
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    var userToDeleteUID = findUser();

                    cmd.Connection = conn;
                    //name of the text box
                    cmd.CommandText = "UPDATE USERS set averagestars = " + average_stars + " WHERE Users.userid ='" + userToDeleteUID + "'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                conn.Close();
            }

            // Clear datagrid and re-display (update)
            User_friends_List.Items.Clear();
            CreateFriendsTable();
        }

        // Change detected in new rating for friends
        private void Rate_score_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Rate_score_box.Text != "<rate score (0-5)>")
            {
                newRating = Convert.ToDouble(Rate_score_box.Text);
            }
        }

        // Zipcode selected from zipcode table (Business Search Left Middle)
        private void ZipcodeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Adds states to state table (Business Search Top Left)
        public void addStates()
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct state FROM businesses ORDER BY state";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            StateList.Items.Add(reader.GetString(0));
                        }
                    }
                }
                conn.Close();
            }
        }

        public void addDaysOfWeek()
        {
            DaysOfWeekSelection.Items.Add("Sunday");
            DaysOfWeekSelection.Items.Add("Monday");
            DaysOfWeekSelection.Items.Add("Tuesday");
            DaysOfWeekSelection.Items.Add("Wednesday");
            DaysOfWeekSelection.Items.Add("Thursday");
            DaysOfWeekSelection.Items.Add("Friday");
            DaysOfWeekSelection.Items.Add("Saturday");
        }

        public void addOpeningTimeAndClosingTime()
        {
            FromOpenTime.Items.Add("12:00 AM");
            FromOpenTime.Items.Add("1:00 AM");
            FromOpenTime.Items.Add("2:00 AM");
            FromOpenTime.Items.Add("3:00 AM");
            FromOpenTime.Items.Add("4:00 AM");
            FromOpenTime.Items.Add("5:00 AM");
            FromOpenTime.Items.Add("6:00 AM");
            FromOpenTime.Items.Add("7:00 AM");
            FromOpenTime.Items.Add("8:00 AM");
            FromOpenTime.Items.Add("9:00 AM");
            FromOpenTime.Items.Add("10:00 AM");
            FromOpenTime.Items.Add("11:00 AM");
            FromOpenTime.Items.Add("12:00 AM");
            FromOpenTime.Items.Add("1:00 PM");
            FromOpenTime.Items.Add("2:00 PM");
            FromOpenTime.Items.Add("3:00 PM");
            FromOpenTime.Items.Add("4:00 PM");
            FromOpenTime.Items.Add("5:00 PM");
            FromOpenTime.Items.Add("6:00 PM");
            FromOpenTime.Items.Add("7:00 PM");
            FromOpenTime.Items.Add("8:00 PM");
            FromOpenTime.Items.Add("9:00 PM");
            FromOpenTime.Items.Add("10:00 PM");
            FromOpenTime.Items.Add("11:00 PM");

            ToClosingTime.Items.Add("12:00 AM");
            ToClosingTime.Items.Add("1:00 AM");
            ToClosingTime.Items.Add("2:00 AM");
            ToClosingTime.Items.Add("3:00 AM");
            ToClosingTime.Items.Add("4:00 AM");
            ToClosingTime.Items.Add("5:00 AM");
            ToClosingTime.Items.Add("6:00 AM");
            ToClosingTime.Items.Add("7:00 AM");
            ToClosingTime.Items.Add("8:00 AM");
            ToClosingTime.Items.Add("9:00 AM");
            ToClosingTime.Items.Add("10:00 AM");
            ToClosingTime.Items.Add("11:00 AM");
            ToClosingTime.Items.Add("12:00 AM");
            ToClosingTime.Items.Add("1:00 PM");
            ToClosingTime.Items.Add("2:00 PM");
            ToClosingTime.Items.Add("3:00 PM");
            ToClosingTime.Items.Add("4:00 PM");
            ToClosingTime.Items.Add("5:00 PM");
            ToClosingTime.Items.Add("6:00 PM");
            ToClosingTime.Items.Add("7:00 PM");
            ToClosingTime.Items.Add("8:00 PM");
            ToClosingTime.Items.Add("9:00 PM");
            ToClosingTime.Items.Add("10:00 PM");
            ToClosingTime.Items.Add("11:00 PM");
        }

        // State chosen from state list  (Business Search Top Left)
        private void StateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CityList.Items.Clear();
            ZipcodeList.Items.Clear();
            BusinessList.Items.Clear();
            CategoriesList.Items.Clear(); // Because certain previously selected categories may not exist in another state/zip
            //CityList.MinRowHeight = 20;
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT city FROM businesses WHERE state = '" + StateList.SelectedItem.ToString() + "' ORDER BY city";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tmp = reader.GetString(0);
                            CityList.Items.Add(tmp);
                        }
                    }
                }
                conn.Close();
            }
        }

        private void CityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZipcodeList.Items.Clear();
            if (CityList.SelectedItem != null)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT zipcode FROM businesses WHERE city = '" + CityList.SelectedItem.ToString() + "'";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ZipcodeList.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
        string[] Category_arr = new string[30000];
        private void ZipcodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityList.SelectedItem != null && ZipcodeList.SelectedItem != null)
            {
                string[] arr = new string[30000];

                string tmp = "";
                string[] arr2 = new string[30000];
                int z = 0;

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT DISTINCT category FROM Categories, Businesses WHERE zipcode = '" + ZipcodeList.SelectedItem.ToString() + "' AND businesses.businessid = categories.businessid";
                        using (var reader = cmd.ExecuteReader())
                        {
                            for (int i = 0; reader.Read(); i++)
                            {

                                tmp = reader.GetString(0);
                                arr = tmp.Split(',');
                                for (int x = 0; x < arr.Count(); x++)
                                {
                                    // Remove all extra characters
                                    arr[x] = arr[x].Trim(new Char[] { '[', '\"', '\'', ']', '`', '`' });
                                    if (!char.IsLetter(arr[x].FirstOrDefault()))
                                    {
                                        if (arr[x].Length > 0)
                                        {
                                            arr[x] = arr[x].Substring(1, arr[x].Length - 1);

                                            if (!char.IsLetter(arr[x].FirstOrDefault()))
                                            {
                                                arr[x] = arr[x].Substring(1, arr[x].Length - 1);
                                            }
                                        }
                                    }

                                    // arr2 holds ALL the categories of ALL businessses
                                    arr2[z] = arr[x];
                                    z++;
                                }
                                //arr[] = reader.GetString(0);
                            }
                        }
                    }
                    conn.Close();
                }

                string[] q = arr2.Distinct().ToArray();

                for (int j = 0; j < q.Count(); j++)
                {
                    if (q[j] != null)
                        BusinessList.Items.Add(q[j]);
                }

                string output = "";
                for (int i = 0; i < q.Count(); i++) {
                    output += q[i];
                    if (i == q.Count()) continue;

                        output += "*";
                    
                }
                Application.Current.Properties["third"] = output;
            }
        }

        public string[] getUnparsedArray()
        {
            string[] arr = new string[30000];
            string tmp = "";
            string[] arr2 = new string[30000];


            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT DISTINCT category FROM Categories, Businesses WHERE zipcode = '" + ZipcodeList.SelectedItem.ToString() + "' AND businesses.businessid = categories.businessid";
                    using (var reader = cmd.ExecuteReader())
                    {
                        for (int i = 0; reader.Read(); i++)
                        {
                            tmp = reader.GetString(0);
                            arr = tmp.Split(',');
                            for (int x = 0; x < arr.Count(); x++)
                            {
                                arr2[x] = arr[x];
                            }
                            //arr[] = reader.GetString(0);

                        }
                    }
                }
                conn.Close();
            }

            return arr2;
        }

        private void BusinessList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BusinessList.SelectedItem != null)
            {
                selectedCategory = BusinessList.SelectedItem.ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CategoriesList.Items.Add(selectedCategory);
        }

        private void CategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesList.SelectedItem != null)
            {
                selectedCategoryToDelete = CategoriesList.SelectedItem.ToString();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CategoriesList.Items.Remove(selectedCategoryToDelete);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedBusiness != null)
            {
                selectedBusiness = (table_search_results)Search_results_Grid.SelectedItem;
                if (selectedBusiness != null)
                {
                    selectedBusinessLabel.Content = selectedBusiness.BusinessName;
                    Application.Current.Properties["first"] = selectedBusiness.selectedBusinessID;

                }
            }
        }

        private void SearchBusinessesButtonClicked(object sender, RoutedEventArgs e)
        {
            Search_results_Grid.Items.Clear();
            //SELECT Categories.category, Businesses.name,Businesses.zipcode,Businesses.city, Businesses.fulladdress, count(tips.userid), Businesses.numcheckins
            //FROM Businesses, tips, users, Categories
            //WHERE 
            //Businesses.businessid = Categories.businessid AND Users.userid = Tips.userid AND Users.name = Businesses.name
            //AND '["Women`s Clothing", `Fashion`, `Shopping`, `Accessories`, `Shoe Stores`]' = Categories.category 
            //AND '85251' = Businesses.zipcode
            //AND 'Scottsdale' = Businesses.city

            //Group by Businesses.zipcode,Businesses.city,Categories.category, Businesses.name, Businesses.fulladdress,Businesses.numcheckins

            string[] arr2 = new string[30000];
            arr2 = getUnparsedArray();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount FROM  Categories , Businesses WHERE  '89005' =Businesses.zipcode AND Businesses.businessid = Categories.businessid AND Categories.category like '%Food%'
                    cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM  Categories , Businesses WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode ";
                    for (int i = 0; i < CategoriesList.Items.Count; i++)
                    {
                        cmd.CommandText += " AND Categories.category like '%";
                        cmd.CommandText += CategoriesList.Items[i].ToString();
                        cmd.CommandText += "%'";
                    }


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }


            //"SELECT Categories.category, Businesses.name,Businesses.zipcode,Businesses.city, Businesses.fulladdress, count(tips.userid), Businesses.numcheckins FROM Businesses, tips, users, Categories WHERE  Businesses.businessid = Categories.businessid AND Users.userid = Tips.userid AND Users.name = Businesses.name AND AND Categories.category like '%P%'AND Categories.category like '%a%'AND Categories.category like '%r%'AND Categories.category like '%t%'AND Categories.category like '%y%'AND Categories.category like '% %'AND Categories.category like '%&%'AND Categories.category like '% %'AND Categories.category like '%E%'AND Categories.category like '%v%'AND Categories.category like '%e%'AND Categories.category like '%n%'AND Categories.category like '%t%'AND Categories.category like '% %'AND Categories.category like '%P%'AND Categories.category like '%l%'AND Categories.category like '%a%'AND Categories.category like '%n%'AND Categories.category like '%n%'AND Categories.category like '%i%'AND Categories.category like '%n%'AND Categories.category like '%g%'AND '89015' =Businesses.zipcode  AND 'Boulder City' = Businesses.city Group by Businesses.zipcode,Businesses.city,Categories.category, Businesses.name, Businesses.fulladdress,Businesses.numcheckins"
        }

        private void AddTipButtonClicked(object sender, RoutedEventArgs e)
        {

            // SQL command to insert a new entry!
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    var userToDeleteUID = findUser();

                    cmd.Connection = conn;
                    //name of the text box
                    //INSERT INTO Tips (userid,businessid,date,likes,text) VALUES('hoThsXW1ndoytxVwc_MVLQ','b9WZJp5L1RZr4F1nxclOoQ','2013-10-17',0,'Wonderful food, generous portions.  What more  can you ask for!');

                    cmd.CommandText = "INSERT INTO Tips (userid,businessid,date,likes,text) VALUES ('" + UserIDBox.Text + "','" + selectedBusiness.selectedBusinessID + "','" + DateTime.Today.ToString("dd-MM-yyyy") + "',0, '" + TipInputBox.Text + "');";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                conn.Close();
            }
  

            // SQL command to update business table
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount FROM  Categories , Businesses WHERE  '89005' =Businesses.zipcode AND Businesses.businessid = Categories.businessid AND Categories.category like '%Food%'
                    cmd.CommandText = "UPDATE businesses SET reviewCount = t.ct FROM (SELECT count(text) AS ct, businessid FROM tips GROUP BY businessid) t INNER JOIN businesses AS B_join ON B_join.businessid = t.businessid WHERE businesses.businessid = t.businessid;";


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                conn.Close();
            }

            // SQL Command to fetch the results again, refresh results
            Search_results_Grid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount FROM  Categories , Businesses WHERE  '89005' =Businesses.zipcode AND Businesses.businessid = Categories.businessid AND Categories.category like '%Food%'
                    cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM  Categories , Businesses WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode ";
                    for (int i = 0; i < CategoriesList.Items.Count; i++)
                    {
                        cmd.CommandText += " AND Categories.category like '%";
                        cmd.CommandText += CategoriesList.Items[i].ToString();
                        cmd.CommandText += "%'";
                    }


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }
        }

        private void CheckInButtonClicked(object sender, RoutedEventArgs e)
        {
            // SQL command to insert a new entry!
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //name of the text box
                    string currentTimeOfDay = "";
                    int currentHour = DateTime.UtcNow.AddHours(-7).Hour;
                    if (currentHour >= 6 && currentHour < 12){
                        currentTimeOfDay = "monrning";
                    } else if (currentHour >= 12 && currentHour < 17) {
                        currentTimeOfDay = "afternoon";
                    } else if (currentHour >= 17 && currentHour < 23) {
                        currentTimeOfDay = "evening";
                    } else if (currentHour >= 23 && currentHour <= 24) {
                        currentTimeOfDay = "night";
                    }
                        //Utcnow.Today.ToString("hh tt");
                    //DateTime dt = new DateTime();
                    string DayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();
                    int dayOfWeekInt = 0;
                    if (DayOfWeek == "Sunday")
                    {
                        dayOfWeekInt = 0;
                    } else if (DayOfWeek == "Monday") {
                        dayOfWeekInt = 1;
                    } else if (DayOfWeek == "Tuesday") {
                        dayOfWeekInt = 2;
                    } else if (DayOfWeek == "Wednesday") {
                        dayOfWeekInt = 3;
                    } else if (DayOfWeek == "Thursday") {
                        dayOfWeekInt = 4;
                    } else if (DayOfWeek == "Friday") {
                        dayOfWeekInt = 5;
                    } else if (DayOfWeek == "Saturday") {
                        dayOfWeekInt = 6;
                    }

                    if (selectedBusiness != null)
                    {
                        // INSERT INTO CheckIns (businessid,time,day,count) VALUES ('--qeSYxyn62mMjWvznNTdg', 'morning', 0, 1), 
                        cmd.CommandText = "INSERT INTO CheckIn (businessid,time,day,count) VALUES ('" + selectedBusiness.selectedBusinessID + "', '" + currentTimeOfDay + "','" + dayOfWeekInt + "','1');";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }
                }
                conn.Close();
            }

            // SQL command to update business table
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount FROM  Categories , Businesses WHERE  '89005' =Businesses.zipcode AND Businesses.businessid = Categories.businessid AND Categories.category like '%Food%'
                    cmd.CommandText = "UPDATE businesses SET numcheckins = t.ct FROM (SELECT count(businessid) AS ct, businessid FROM checkin GROUP BY businessid) t INNER JOIN businesses AS B_join ON B_join.businessid = t.businessid WHERE businesses.businessid = t.businessid;";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                conn.Close();
            }

            Search_results_Grid.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount FROM  Categories , Businesses WHERE  '89005' =Businesses.zipcode AND Businesses.businessid = Categories.businessid AND Categories.category like '%Food%'
                    cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories , Businesses WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode";
                    for (int i = 0; i < CategoriesList.Items.Count; i++)
                    {
                        cmd.CommandText += " AND Categories.category like '%";
                        cmd.CommandText += CategoriesList.Items[i].ToString();
                        cmd.CommandText += "%'";
                    }


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (selectedBusiness != null)
            {
                Application.Current.Properties["second"] = selectedBusiness.selectedBusinessID;
            }
            else 
            {
                Application.Current.Properties["second"] = Search_results_Grid.SelectedItem;
            }
            // New Pop Up Window
            var window = new TipsWindow();
            window.Show();
            //window.selectedBusinessName = selectedBusiness.BusinessName;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (selectedBusiness != null)
            {
                Application.Current.Properties["second"] = selectedBusiness.selectedBusinessID;
            }
            else
            {
                Application.Current.Properties["second"] = Search_results_Grid.SelectedItem;
            }
            // New Pop Up Window
            var window = new CheckinWindow();
            window.Show();
            //window.selectedBusinessName = selectedBusiness.BusinessName;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //CHART: Number of Business per Category in hte Zipcode
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // New Pop Up Window
            var window = new BusinessCategoryWindow();
            window.Show();
            Application.Current.Properties["second"] = selectedBusiness.selectedBusinessID;
            Application.Current.Properties["fourth"] = ZipcodeList.SelectedItem.ToString();
            //window.selectedBusinessName = selectedBusiness.BusinessName;
        }

        private void DaysOfWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear old results
            Search_results_Grid.Items.Clear();

            // Add it to old SQL query as additional parameter
            string[] arr2 = new string[30000];
            arr2 = getUnparsedArray();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //                 SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '28012' = Businesses.zipcode AND 'Monday' = Hours.day AND Hours.businessid = Businesses.businessid AND Categories.category like '%Active Life%'
                    cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode AND Hours.day ='" + DaysOfWeekSelection.SelectedItem.ToString() + "' AND Hours.businessid = Businesses.businessid ";

                    for (int i = 0; i < CategoriesList.Items.Count; i++)
                    {
                        cmd.CommandText += " AND Categories.category like '%";
                        cmd.CommandText += CategoriesList.Items[i].ToString();
                        cmd.CommandText += "%'";
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }
        }

        private void FromOpenTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                // Clear old results
                Search_results_Grid.Items.Clear();

                // Add it to old SQL query as additional parameter
                string[] arr2 = new string[30000];
                arr2 = getUnparsedArray();

                int intOpen = 0;
                switch (FromOpenTime.SelectedIndex)
                {
                    case 0:
                        intOpen = 1200;
                        break;
                    case 1:
                        intOpen = 100;
                        break;
                    case 2:
                        intOpen = 200;
                        break;
                    case 3:
                        intOpen = 300;
                        break;
                    case 4:
                        intOpen = 400;
                        break;
                    case 5:
                        intOpen = 500;
                        break;
                    case 6:
                        intOpen = 600;
                        break;
                    case 7:
                        intOpen = 700;
                        break;
                    case 8:
                        intOpen = 800;
                        break;
                    case 9:
                        intOpen = 900;
                        break;
                    case 10:
                        intOpen = 1000;
                        break;
                    case 11:
                        intOpen = 1100;
                        break;
                    case 12:
                        intOpen = 1200;
                        break;
                    case 13:
                        intOpen = 1300;
                        break;
                    case 14:
                        intOpen = 1400;
                        break;
                    case 15:
                        intOpen = 1500;
                        break;
                    case 16:
                        intOpen = 1600;
                        break;
                    case 17:
                        intOpen = 1700;
                        break;
                    case 18:
                        intOpen = 1800;
                        break;
                    case 19:
                        intOpen = 1900;
                        break;
                    case 20:
                        intOpen = 2000;
                        break;
                    case 21:
                        intOpen = 2100;
                        break;
                    case 22:
                        intOpen = 2200;
                        break;
                    case 23:
                        intOpen = 2300;
                        break;
                    default:
                        break;
                }

                globalIntOpen = intOpen;
                    //FromOpenTime.SelectedItem;
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        //                 SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '28012' = Businesses.zipcode AND Hours.day ='Monday' AND CAST(Hours.open AS INT) < 800 AND Hours.businessid = Businesses.businessid  AND Categories.category like '%Trainers%'
                        cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode AND Hours.day ='" + DaysOfWeekSelection.SelectedItem.ToString() + "' AND CAST(Hours.open AS INT) > " + intOpen + "AND Hours.businessid = Businesses.businessid ";

                        for (int i = 0; i < CategoriesList.Items.Count; i++)
                        {
                            cmd.CommandText += " AND Categories.category like '%";
                            cmd.CommandText += CategoriesList.Items[i].ToString();
                            cmd.CommandText += "%'";
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                            }
                        }
                    }
                    conn.Close();
                }
        }

        private void ToClosingTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Clear old results
            Search_results_Grid.Items.Clear();

            // Add it to old SQL query as additional parameter
            string[] arr2 = new string[30000];
            arr2 = getUnparsedArray();

            int intClose = 0;
            switch (FromOpenTime.SelectedIndex)
            {
                case 0:
                    intClose = 1200;
                    break;
                case 1:
                    intClose = 100;
                    break;
                case 2:
                    intClose = 200;
                    break;
                case 3:
                    intClose = 300;
                    break;
                case 4:
                    intClose = 400;
                    break;
                case 5:
                    intClose = 500;
                    break;
                case 6:
                    intClose = 600;
                    break;
                case 7:
                    intClose = 700;
                    break;
                case 8:
                    intClose = 800;
                    break;
                case 9:
                    intClose = 900;
                    break;
                case 10:
                    intClose = 1000;
                    break;
                case 11:
                    intClose = 1100;
                    break;
                case 12:
                    intClose = 1200;
                    break;
                case 13:
                    intClose = 1300;
                    break;
                case 14:
                    intClose = 1400;
                    break;
                case 15:
                    intClose = 1500;
                    break;
                case 16:
                    intClose = 1600;
                    break;
                case 17:
                    intClose = 1700;
                    break;
                case 18:
                    intClose = 1800;
                    break;
                case 19:
                    intClose = 1900;
                    break;
                case 20:
                    intClose = 2000;
                    break;
                case 21:
                    intClose = 2100;
                    break;
                case 22:
                    intClose = 2200;
                    break;
                case 23:
                    intClose = 2300;
                    break;
                default:
                    break;
            }

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    //                 SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '28012' = Businesses.zipcode AND Hours.day ='Monday' AND CAST(Hours.open AS INT) < 800 AND Hours.businessid = Businesses.businessid  AND Categories.category like '%Trainers%'
                    cmd.CommandText = "SELECT DISTINCT Businesses.fulladdress, Businesses.name, Businesses.numcheckins, businesses.reviewcount, businesses.businessid FROM Categories, Businesses, Hours WHERE  '" + ZipcodeList.SelectedItem.ToString() + "' = Businesses.zipcode AND Hours.day ='" + DaysOfWeekSelection.SelectedItem.ToString() + "' AND CAST(Hours.open AS INT) > " + globalIntOpen + " AND CAST(Hours.close AS INT) < " + intClose +" AND Hours.businessid = Businesses.businessid ";

                    for (int i = 0; i < CategoriesList.Items.Count; i++)
                    {
                        cmd.CommandText += " AND Categories.category like '%";
                        cmd.CommandText += CategoriesList.Items[i].ToString();
                        cmd.CommandText += "%'";
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Search_results_Grid.Items.Add(new table_search_results() { BusinessName = reader.GetString(1), Address = reader.GetString(0), NumberofTips = reader.GetString(3), TotalCheckins = reader.GetString(2), selectedBusinessID = reader.GetString(4) });
                        }
                    }
                }
                conn.Close();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["fourth"] = ZipcodeList.SelectedItem.ToString();

            // New Pop Up Window
            var window = new AverageStarsWindow();
            window.Show();


        }
    }
}
