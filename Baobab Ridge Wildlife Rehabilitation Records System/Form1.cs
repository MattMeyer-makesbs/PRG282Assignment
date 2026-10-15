using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class Form1 : Form
    {
        public string filePath = "animals.txt";
        public List<Animal> animalList = new List<Animal>();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            RefreshGrid();
        }



        private void EnsureFileExists()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    // Create the file and immediately close the file stream
                    File.Create(filePath).Close();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied while creating the animals file.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("Unable to create animals file because it is in use by another process.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while crating the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        private void LoadAnimalData()
        {
            //clear the existing list so refreshing doesn't create duplicate animal records
            animalList.Clear();
            EnsureFileExists();

            int skippedLines = 0; //counter for skipped lines due to incorrect format

            try
            {
                //Place ReadAllLines inside the try block so IO/Access exceptions are caught
                string[] info = File.ReadAllLines(filePath);

                foreach (string line in info)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|'); //split lines using |

                    if (parts.Length == 7)//Check for exactly 7 fields
                    {
                        string id = parts[0].Trim().ToUpper();
                        string name = parts[1].Trim();
                        string species = parts[2].Trim();
                        string ageText = parts[3].Trim();
                        string scoreText = parts[4].Trim();
                        string status = parts[5].Trim();
                        string house = parts[6].Trim();

                        //validate ID format (WR- + 4 digits), valid integer parsing, and range 0-100
                        if (id.Length == 7 && id.StartsWith("WR-") && int.TryParse(id.Substring(3), out _) && int.TryParse(ageText, out int age) && (age >= 0 && age <= 100) && int.TryParse(scoreText, out int score) && (score >= 0 && score <= 100))
                        {
                            //create object and add to animal list
                            animalList.Add(new Animal(id, name, species, age, score, status, house));
                        }
                        else
                        {
                            skippedLines++; //skips if ID, age, or score are malformed
                        }
                    }
                    else
                    {
                        skippedLines++; //sips if wrong nimber of fields
                    }
                }

                //notify user once after all lines are processed
                if (skippedLines > 0)
                {
                    MessageBox.Show($"{skippedLines} malformed record(s) were skipped.", "Data Load Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied when attempting to read animals.txt.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("The animals.txt file is currently open in another program.", "File In Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetupDataGridView()
        {
            dataGridView1.ColumnCount = 7;
            dataGridView1.Columns[0].Name = "ID";
            dataGridView1.Columns[1].Name = "Name";
            dataGridView1.Columns[2].Name = "Species";
            dataGridView1.Columns[3].Name = "Age";
            dataGridView1.Columns[4].Name = "Score";
            dataGridView1.Columns[5].Name = "Status";
            dataGridView1.Columns[6].Name = "House";
            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.AutoGenerateColumns = false;
        }
        public void RefreshGrid()
        {
            LoadAnimalData();
            dgvAnimals.DataSource = null;
            dgvAnimals.DataSource = animalList;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
}