using System.Windows.Forms;
using System.Data.SqlClient;

namespace Validation {
    public partial class Form1 : Form
    {
        bool valid = true;
        Validator v = new Validator();
        public Form1()
        {
            InitializeComponent();
        }

        private int GetAge(DateTime birthdate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthdate.Year;


            if (birthdate.Date > today.AddYears(-age)) 
            {
                age--;
            } 

            return age;
        }

        

       

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void BTNREGISTER_Click(object sender, EventArgs e)
        {
            if (!v.IsValidEmail(TBEMAIL.Text))
            {
                MessageBox.Show("Invalid Email");
                valid = false;
            }

            if(TBUSERNAME.Text == TBPASSWORD.Text)
            {
                MessageBox.Show("User and Password should not be the same");
                valid = false;
            }

            if (TBUSERNAME.Text == "")
            {
                MessageBox.Show("No Username");
                valid = false;
            }

            if (!v.IsValidPassword(TBPASSWORD.Text))
            {
                MessageBox.Show("Invalid Password");
                valid = false;
            }
            String gender = "";
            if(RBFEMALE.Checked || RBMALE.Checked)
            {
                if (RBMALE.Checked)
                {
                    gender = RBMALE.Text;
                }

                if (RBFEMALE.Checked)
                {
                    gender = RBFEMALE.Text;
                }
            }
            else
            {
                MessageBox.Show("Choose gender");
                valid = false;
            }

            DateTime selectedDate = DATETIME.Value;
            int age = GetAge(selectedDate);

            if (age < 16)
            {
                MessageBox.Show("Age must be at least 16 years old.", "Invalid Age", MessageBoxButtons.OK, MessageBoxIcon.Error);
                valid = false;  
                DATETIME.Value = DateTime.Today.AddYears(-16);
            }

            try
            {
                if (valid)
                {
                    using (SqlConnection conn = new SqlConnection("Data Source=PC00014\\SQLEXPRESS;Initial Catalog=FORM;Integrated Security=True;TrustServerCertificate=True"))
                    {
                        conn.Open();

                        
                        string checkUsernameQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                        using (SqlCommand cmd = new SqlCommand(checkUsernameQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", TBUSERNAME.Text);
                            int count = (int)cmd.ExecuteScalar();
                            if (count > 0)
                            {
                                MessageBox.Show("Username already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        
                        string insertQuery = "INSERT INTO USERS (Username, Password, Email, Gender, Birthdate) VALUES (@Username, @Password, @Email, @Gender, @Birthdate)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", TBUSERNAME.Text);
                            cmd.Parameters.AddWithValue("@Password", TBPASSWORD.Text);
                            cmd.Parameters.AddWithValue("@Email", TBEMAIL.Text);
                            cmd.Parameters.AddWithValue("@Gender", gender);
                            cmd.Parameters.AddWithValue("@Birthdate", DATETIME.Value);

                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    Person p = new Person
                    {
                        Username = TBUSERNAME.Text,
                        Password = TBPASSWORD.Text,
                        Email = TBEMAIL.Text,
                        Gender = gender,
                        Birthdate = DATETIME.Value
                    };

                    Console.WriteLine("You are now registered");

                    Display D = new Display(p);
                    D.ShowDialog();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
