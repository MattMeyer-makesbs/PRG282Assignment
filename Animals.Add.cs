using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    // Add Animal feature. 
    public partial class Animals
    {
        // Handles the Add Animal button: validates, saves one new line to animals.txt, then resets the form.
        private void Addbutton_Click(object sender, EventArgs e)
        {
            ValidationResult result = AnimalValidator.Validate(
                AnimalTB.Text, NameTB.Text, SpeciesTB.Text, AgeTB.Text, RCtb.Text,
                animalList, true);

            // On any validation problem save nothing, name the field, and put the cursor back in it.
            if (!result.IsValid)
            {
                MessageBox.Show(result.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusInputField(result.Field);
                return;
            }

            // If the file could not be written, keep the user's input so they can try again.
            if (!AppendAnimalToFile(result.Animal))
            {
                return;
            }

            Animal added = result.Animal;
            RefreshGrid();
            ClearInputs();

            MessageBox.Show(
                "Animal " + added.ID + " (" + added.Name + ") was added." + Environment.NewLine + Environment.NewLine
                + "Status: " + added.Status + Environment.NewLine
                + "Housing Unit: " + added.HousingUnit,
                "Animal Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Appends one animal as a new line at the end of animals.txt. and returns true if it was saved.
        private bool AppendAnimalToFile(Animal animal)
        {
            try
            {
                EnsureFileExists();

                // If a replaced file does not end with a line break, add one first so we never glue two records together.
                string prefix = "";
                string existingText = File.ReadAllText(filePath);
                if (existingText.Length > 0 && !existingText.EndsWith("\n"))
                {
                    prefix = Environment.NewLine;
                }

                File.AppendAllText(filePath, prefix + FormatAnimalLine(animal) + Environment.NewLine, new UTF8Encoding(false));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied: the animal could not be saved to animals.txt.",
                    "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The folder that holds animals.txt could not be found.",
                    "Folder Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("animals.txt is in use by another program. Close it and try again.",
                    "File In Use", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while saving: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        // Turns an animal into one animals.txt line.
        private static string FormatAnimalLine(Animal animal)
        {
            return string.Join("|", animal.ID, animal.Name, animal.Species,
                animal.Age, animal.RecoveryScore, animal.Status, animal.HousingUnit);
        }

        // Moves the cursor into the text box that matches the field that failed validation.
        private void FocusInputField(AnimalField field)
        {
            TextBox target;
            switch (field)
            {
                case AnimalField.AnimalId: target = AnimalTB; break;
                case AnimalField.Name: target = NameTB; break;
                case AnimalField.Species: target = SpeciesTB; break;
                case AnimalField.Age: target = AgeTB; break;
                default: target = RCtb; break;
            }

            target.Focus();
            target.SelectAll();
        }

        // Empties all five input boxes and puts the cursor back in Animal ID.
        private void ClearInputs()
        {
            AnimalTB.Clear();
            NameTB.Clear();
            SpeciesTB.Clear();
            AgeTB.Clear();
            RCtb.Clear();
            AnimalTB.Focus();
        }


    }
}
