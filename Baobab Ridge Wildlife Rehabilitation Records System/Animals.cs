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

        // State variables for Add/Edit mode
        private bool isEditMode = false;
        private string currentEditingId = null;
        private string currentEditingStatus = null;
        private string currentEditingHousing = null;
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
            //Ensure we are in Add Mode
            if (isEditMode) return; // Safety check

            //  Get data from form
            string id = AnimalTB.Text.Trim().ToUpper();
            string name = NameTB.Text.Trim();
            string species = SpeciesTB.Text.Trim();
            string ageText = AgeTB.Text.Trim();
            string scoreText = RCtb.Text.Trim(); 
            

            // Validation (Reusing your existing validation logic)
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(species) || string.IsNullOrEmpty(ageText))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check ID format (WR- + 4 digits)
            if (id.Length != 7 || !id.StartsWith("WR-") || !int.TryParse(id.Substring(3), out _))
            {
                MessageBox.Show("Animal ID must be in the format 'WR-0000'.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if ID already exists
            if (animalList.Any(a => a.ID.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("An animal with this ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check Age
            if (!int.TryParse(ageText, out int age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be a valid number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check Score
            if (!int.TryParse(scoreText, out int score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be a valid number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                //  Create the new line. 
                // NOTE: We must provide default values for Status and Housing because your GUI doesn't have textboxes for them.
                // You can change "Stable" and "Recovery Ward" to whatever defaults you prefer.
                string defaultStatus = "Stable";
                string defaultHousing = "Recovery Ward";

                string newLine = $"{id} | {name} | {species} | {age} | {score} | {defaultStatus} | {defaultHousing}";

                //  Append to the file
                File.AppendAllText(filePath, newLine + Environment.NewLine);

                MessageBox.Show("Animal added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  Refresh grid and clear form
                RefreshGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while adding the animal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            try
            {
                List<Animal> animals = LoadAnimalsforSummary();

                int total = animals.Count;
                double averageAge = 0;
                double averageScore = 0;

                int critical = 0, serious = 0, stable = 0, recovered = 0, releaseReady = 0;


                if (total > 0)
                {

                    double ageSum = 0, scoreSum = 0;

                    foreach (Animal a in animals)
                    {
                        ageSum += a.Age;
                        scoreSum += a.RecoveryScore;

                        switch (a.Status)
                        {
                            case
                            "Critical":
                                critical++;
                                break;
                            case
                            "Serious":
                                serious++;
                                break;
                            case
                             "Stable":
                                stable++;
                                break;
                            case
                                "Recovering":
                                recovered++;
                                break;

                            case
                                "Release Ready":
                                releaseReady++;
                                break;
                        }

                    }
                    averageAge = ageSum / total;
                    averageScore = scoreSum / total;
                }


                string agetext = total == 0 ? "N/A" : averageAge.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                string scoretext = total == 0 ? "N/A" : averageScore.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);


                label7.Text =
                    $"Total Animals: {total}" +
                    $"\nAverage Age: {agetext}" +
                    $"\nAverage Recovery Score: {scoretext}" +
                    $"\n\nStatus Counts:" +
                    $"\nCritical: {critical}" +
                    $"\nSerious: {serious}" +
                    $"\nStable: {stable}" +
                    $"\nRecovering: {recovered}" +
                    $"\nRelease Ready: {releaseReady}";

                string summaryfile = Path.Combine(Application.StartupPath, "Summary.txt");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE ANIMAL SUMMARY REPORT");
                sb.AppendLine("Generated:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:mm"));
                sb.AppendLine();
                sb.AppendLine($"Total Animals: {total}");
                sb.AppendLine($"Average Age: {agetext}");
                sb.AppendLine($"Average Recovery Score: {scoretext}");
                sb.AppendLine();
                sb.AppendLine("Animals per Status:");
                sb.AppendLine($"Critical: {critical}");
                sb.AppendLine($"Serious: {serious}");
                sb.AppendLine($"Stable: {stable}");
                sb.AppendLine($"Recovering: {recovered}");
                sb.AppendLine($"Release Ready: {releaseReady}");

                File.WriteAllText(summaryfile, sb.ToString());

                MessageBox.Show($"Report Summary generated successfully at {summaryfile}.", "Summary Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while generating the summary report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private List<Animal> LoadAnimalsforSummary()
        {


            List<Animal> result = new List<Animal>();
            EnsureFileExists();
            try
            {
                string[] info = File.ReadAllLines(filePath);
                foreach (string line in info)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split('|');
                    if (parts.Length != 7) continue;
                    {
                        string id = parts[0].Trim().ToUpper();
                        string name = parts[1].Trim();
                        string species = parts[2].Trim();
                        string ageStr = parts[3].Trim();
                        string scoreStr = parts[4].Trim();
                        string status = parts[5].Trim();
                        string housing = parts[6].Trim();


                        if (id.Length != 7 || !id.StartsWith("WR-")) continue;
                        if (!int.TryParse(ageStr, out int age)) continue;
                        if (!int.TryParse(scoreStr, out int score)) continue;

                        result.Add(new Animal(id, name, species, age, score, status, housing));
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data for summary: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        
        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = SearchIDTB.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter an Animal ID to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            Animal foundAnimal = animalList.FirstOrDefault(a => a.ID.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (foundAnimal != null)
            {
                
                foreach (DataGridViewRow row in DGVAnimals.Rows)
                {
                    if (row.DataBoundItem == foundAnimal)
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Animal ID not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        
        }


       
        private void Savebutton_Click(object sender, EventArgs e)
        {
            if (!isEditMode) return;

           
            string newName = NameTB.Text.Trim();
            string newSpecies = SpeciesTB.Text.Trim();
            string ageText = AgeTB.Text.Trim();
            string scoreText = RCtb.Text.Trim();
            
            
            

           
            if (string.IsNullOrEmpty(newName) || string.IsNullOrEmpty(newSpecies) || string.IsNullOrEmpty(ageText))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(ageText, out int age) || age < 0 || age > 100)
            {
                MessageBox.Show("Age must be a valid number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(scoreText, out int score) || score < 0 || score > 100)
            {
                MessageBox.Show("Recovery Score must be a valid number between 0 and 100.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                List<string> updatedLines = new List<string>();

               
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');
                    if (parts.Length == 7)
                    {
                        string lineId = parts[0].Trim().ToUpper();

                        
                        if (lineId == currentEditingId)
                        {
                            
                            string updatedLine = $"{currentEditingId} | {newName} | {newSpecies} | {age} | {score} | {currentEditingStatus} | {currentEditingHousing}";
                            updatedLines.Add(updatedLine);
                        }
                        else
                        {
                            updatedLines.Add(line); 
                        }
                    }
                    else
                    {
                        updatedLines.Add(line);
                    }
                }

                File.WriteAllLines(filePath, updatedLines);

                MessageBox.Show("Animal updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

               
                RefreshGrid();
                ClearForm();
                DGVAnimals.ClearSelection();

                isEditMode = false;
                currentEditingId = null;
                AnimalTB.ReadOnly = false;
                Addbutton.Enabled = true;
                Savebutton.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving changes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DGVAnimals_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            
            AnimalTB.Clear();
            NameTB.Clear();
            SpeciesTB.Clear();
            AgeTB.Clear();
            RCtb.Clear(); 

            
            
            DGVAnimals.ClearSelection();

            
            isEditMode = false;
            currentEditingId = null;
            currentEditingStatus = null;
            currentEditingHousing = null;

            
            AnimalTB.ReadOnly = false;      
            Addbutton.Enabled = true; 
            Savebutton.Enabled = false;     
        }
    }
}
    

