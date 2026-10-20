# Baobab Ridge Wildlife Rehabilitation Records System

## Team Members
* **Matthew Meyer** - Project Setup, Viewing, Create Form
* **Galaletsang Mahlahlane** -  Add Animal , Validation and Classification
* **Vusumuzi Hlongwa** -  Search Update
* **Tshepo Matsepe** - Delete , Summary 

## How to Build and Run
1. Clone the repository to your local machine using Visual Studio.
2. Open the solution file (requires .NET 8.0 or newer).
3. Select **Build > Rebuild Solution** to compile the project and ensure the starter data file is copied to the debug folder.
4. Press  **Start** to run the application.

## List of Features
* **Feature 1 - Add an animal:** Validates all fields, calculates the correct status and housing unit based on the recovery score, and appends the new record.
* **Feature 2 - View all animals:** Displays all valid records in a strictly ordered, read-only grid on startup, safely skipping any malformed data.
* **Feature 3 - Update an animal:** Allows searching by ID or grid selection to safely edit and recalculate an existing record without altering other lines.
* **Feature 4 - Delete an animal:** Removes a single selected record from the file after a Yes/No user confirmation prompt.
* **Feature 5 - Summary report:** Calculates totals, averages, and per-status counts, displaying them on the form and generating an external report file.

## File Formats
* **`animals.txt`**: The main data storage file. It uses a pipe (`|`) delimiter to separate 7 specific fields: `ID | Name | Species | Age | Score | Status | Housing Unit`. 
* **`summary.txt`**: A generated report file. It is overwritten every time the summary button is clicked and contains a specific timestamped layout of the current animal statistics.

