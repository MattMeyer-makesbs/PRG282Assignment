using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class Animals : Form
    {
        public string filePath = "animals.txt";
        private List<Animal> animalList = new List<Animal>();
        public Animals()
        {
            InitializeComponent();
            SetupDataGridView();
        }
        private void Animals_Load_1(object sender, EventArgs e)
        {

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
        }

            private void ClearForm()
        { 
        AnimalTB.Clear();
            NameTB.Clear();
            SpeciesTB.Clear();
            AgeTB.Clear();
          
  
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
            DGVAnimals.ReadOnly = true;
            DGVAnimals.AllowUserToAddRows = false;
            DGVAnimals.AllowUserToDeleteRows = false;
            DGVAnimals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVAnimals.MultiSelect = false;
            DGVAnimals.AutoGenerateColumns = false;

            DGVAnimals.Columns.Clear();
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "Animal ID", Name = "colID" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name", Name = "colName" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Species", HeaderText = "Species", Name = "colSpecies" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Age", HeaderText = "Age", Name = "colAge" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RecoveryScore", HeaderText = "Recovery Score", Name = "colScore" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Name = "colStatus" });
            DGVAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HousingUnit", HeaderText = "Housing Unit", Name = "colHouse" });
        }
        public void RefreshGrid()
        {
            LoadAnimalData();
            DGVAnimals.DataSource = null;
            DGVAnimals.DataSource = animalList;
        }





        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {


            if (DGVAnimals.CurrentRow == null)
            {

                MessageBox.Show("Please select an animal to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            Animal selected = DGVAnimals.CurrentRow.DataBoundItem as Animal;
            if (selected == null)
            {
                MessageBox.Show("Could not identify the selected animal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string IDtoDelete = selected.ID;
            string nameToDelete = selected.Name;


            DialogResult result = MessageBox.Show($"Are you sure you want to delete {nameToDelete} (ID: {IDtoDelete})? This action cannot be undone.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;
            {
                try
                {
                    string[] alllines = File.ReadAllLines(filePath);
                    List<string> KeptLines = new List<string>();

                    foreach (string line in alllines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        {
                            string[] parts = line.Split('|');

                            if (parts.Length < 1) continue;
                            {
                                string lineID = parts[0].Trim().ToUpper();
                                if (lineID != IDtoDelete.ToUpper())
                                {
                                    KeptLines.Add(line);
                                }
                            }

                            File.WriteAllLines(filePath, KeptLines);

                            RefreshGrid();

                            if (AnimalTB.Text.Trim().ToUpper() == IDtoDelete.ToUpper())
                            {
                                ClearForm();

                            }

                            MessageBox.Show($"{nameToDelete} (ID: {IDtoDelete}) has been deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the animal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }



        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {





        }
    }

}
    

