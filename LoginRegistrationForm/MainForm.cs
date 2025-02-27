using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LoginRegistrationForm
{
    public partial class MainForm : Form
    {
        // Database for oxygen tanks
        private List<OxygenTank> tanks = new List<OxygenTank>();

        // File path for saving data
        private string filePath = "oxygen_tanks.csv";

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadTanksFromFile();
            RefreshTanksGrid();
        }

        private void InitializeCustomComponents()
        {
            // Main form properties
            this.Text = "Oxygen Tank Tracking System";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create title label
            Label titleLabel = new Label
            {
                Text = "Oxygen Tank Tracking System",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);

            // Create serial number input
            Label serialLabel = new Label
            {
                Text = "Serial Number:",
                Location = new Point(20, 70),
                Size = new Size(100, 20)
            };
            this.Controls.Add(serialLabel);

            TextBox serialTextBox = new TextBox
            {
                Name = "serialTextBox",
                Location = new Point(130, 70),
                Size = new Size(150, 20)
            };
            this.Controls.Add(serialTextBox);

            // Create bulk serial input
            Label bulkSerialLabel = new Label
            {
                Text = "Bulk Serial Numbers (comma separated):",
                Location = new Point(20, 100),
                Size = new Size(260, 20)
            };
            this.Controls.Add(bulkSerialLabel);

            TextBox bulkSerialTextBox = new TextBox
            {
                Name = "bulkSerialTextBox",
                Location = new Point(20, 130),
                Size = new Size(260, 60),
                Multiline = true
            };
            this.Controls.Add(bulkSerialTextBox);

            // Create delivery button
            Button deliveryButton = new Button
            {
                Text = "Record Delivery",
                Location = new Point(300, 70),
                Size = new Size(120, 30)
            };
            deliveryButton.Click += DeliveryButton_Click;
            this.Controls.Add(deliveryButton);

            // Create bulk delivery button
            Button bulkDeliveryButton = new Button
            {
                Text = "Bulk Delivery",
                Location = new Point(300, 130),
                Size = new Size(120, 30)
            };
            bulkDeliveryButton.Click += BulkDeliveryButton_Click;
            this.Controls.Add(bulkDeliveryButton);

            // Create return button
            Button returnButton = new Button
            {
                Text = "Record Return",
                Location = new Point(430, 70),
                Size = new Size(120, 30)
            };
            returnButton.Click += ReturnButton_Click;
            this.Controls.Add(returnButton);

            // Create bulk return button
            Button bulkReturnButton = new Button
            {
                Text = "Bulk Return",
                Location = new Point(430, 130),
                Size = new Size(120, 30)
            };
            bulkReturnButton.Click += BulkReturnButton_Click;
            this.Controls.Add(bulkReturnButton);

            // Create DataGridView
            DataGridView tanksGrid = new DataGridView
            {
                Name = "tanksGrid",
                Location = new Point(20, 200),
                Size = new Size(940, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tanksGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            this.Controls.Add(tanksGrid);

            // Create button panel
            Panel buttonPanel = new Panel
            {
                Location = new Point(20, 510),
                Size = new Size(940, 40)
            };
            this.Controls.Add(buttonPanel);

            // Create delete button
            Button deleteButton = new Button
            {
                Text = "Delete Selected Tank",
                Location = new Point(0, 5),
                Size = new Size(150, 30)
            };
            deleteButton.Click += DeleteButton_Click;
            buttonPanel.Controls.Add(deleteButton);

            // Create update button
            Button updateButton = new Button
            {
                Text = "Update Destination",
                Location = new Point(160, 5),
                Size = new Size(150, 30)
            };
            updateButton.Click += UpdateButton_Click;
            buttonPanel.Controls.Add(updateButton);

            // Create refresh button
            Button refreshButton = new Button
            {
                Text = "Refresh Data",
                Location = new Point(320, 5),
                Size = new Size(150, 30)
            };
            refreshButton.Click += RefreshButton_Click;
            buttonPanel.Controls.Add(refreshButton);

            // Create export button
            Button exportButton = new Button
            {
                Text = "Export to CSV",
                Location = new Point(480, 5),
                Size = new Size(150, 30)
            };
            exportButton.Click += ExportButton_Click;
            buttonPanel.Controls.Add(exportButton);

            // Create search box
            Label searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(560, 70),
                Size = new Size(60, 20)
            };
            this.Controls.Add(searchLabel);

            TextBox searchTextBox = new TextBox
            {
                Name = "searchTextBox",
                Location = new Point(620, 70),
                Size = new Size(150, 20)
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(searchTextBox);

            // Create destination input
            Label destinationLabel = new Label
            {
                Text = "Destination:",
                Location = new Point(560, 100),
                Size = new Size(100, 20)
            };
            this.Controls.Add(destinationLabel);

            TextBox destinationTextBox = new TextBox
            {
                Name = "destinationTextBox",
                Location = new Point(620, 100),
                Size = new Size(150, 20)
            };
            this.Controls.Add(destinationTextBox);

            // Status label
            Label statusLabel = new Label
            {
                Name = "statusLabel",
                Text = "Ready",
                Location = new Point(20, 170),
                Size = new Size(940, 20),
                ForeColor = Color.Blue
            };
            this.Controls.Add(statusLabel);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Already handled in constructor
        }

        private void DeliveryButton_Click(object sender, EventArgs e)
        {
            string serialNumber = GetControlByName("serialTextBox") is TextBox tb ? tb.Text.Trim() : "";
            string destination = GetControlByName("destinationTextBox") is TextBox dtb ? dtb.Text.Trim() : "";

            if (string.IsNullOrEmpty(serialNumber))
            {
                UpdateStatus("Serial number cannot be empty", Color.Red);
                return;
            }

            RecordTankDelivery(serialNumber, destination);
            GetControlByName("serialTextBox").Text = "";
        }

        private void BulkDeliveryButton_Click(object sender, EventArgs e)
        {
            string bulkSerials = GetControlByName("bulkSerialTextBox") is TextBox tb ? tb.Text.Trim() : "";
            string destination = GetControlByName("destinationTextBox") is TextBox dtb ? dtb.Text.Trim() : "";

            if (string.IsNullOrEmpty(bulkSerials))
            {
                UpdateStatus("Bulk serial numbers cannot be empty", Color.Red);
                return;
            }

            string[] serials = bulkSerials.Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;

            foreach (string serial in serials)
            {
                string trimmedSerial = serial.Trim();
                if (!string.IsNullOrEmpty(trimmedSerial))
                {
                    RecordTankDelivery(trimmedSerial, destination);
                    count++;
                }
            }

            UpdateStatus($"Processed {count} tanks for delivery", Color.Green);
            GetControlByName("bulkSerialTextBox").Text = "";
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            string serialNumber = GetControlByName("serialTextBox") is TextBox tb ? tb.Text.Trim() : "";

            if (string.IsNullOrEmpty(serialNumber))
            {
                UpdateStatus("Serial number cannot be empty", Color.Red);
                return;
            }

            RecordTankReturn(serialNumber);
            GetControlByName("serialTextBox").Text = "";
        }

        private void BulkReturnButton_Click(object sender, EventArgs e)
        {
            string bulkSerials = GetControlByName("bulkSerialTextBox") is TextBox tb ? tb.Text.Trim() : "";

            if (string.IsNullOrEmpty(bulkSerials))
            {
                UpdateStatus("Bulk serial numbers cannot be empty", Color.Red);
                return;
            }

            string[] serials = bulkSerials.Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;

            foreach (string serial in serials)
            {
                string trimmedSerial = serial.Trim();
                if (!string.IsNullOrEmpty(trimmedSerial))
                {
                    RecordTankReturn(trimmedSerial);
                    count++;
                }
            }

            UpdateStatus($"Processed {count} tanks for return", Color.Green);
            GetControlByName("bulkSerialTextBox").Text = "";
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (GetControlByName("tanksGrid") is DataGridView grid && grid.SelectedRows.Count > 0)
            {
                string serialNumber = grid.SelectedRows[0].Cells["SerialNumber"].Value.ToString();

                // Remove tank from collection
                tanks.RemoveAll(t => t.SerialNumber == serialNumber);

                // Refresh grid
                RefreshTanksGrid();

                // Save changes
                SaveTanksToFile();

                UpdateStatus($"Tank {serialNumber} deleted successfully", Color.Green);
            }
            else
            {
                UpdateStatus("Please select a tank to delete", Color.Red);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (GetControlByName("tanksGrid") is DataGridView grid && grid.SelectedRows.Count > 0)
            {
                string serialNumber = grid.SelectedRows[0].Cells["SerialNumber"].Value.ToString();
                string newDestination = GetControlByName("destinationTextBox") is TextBox tb ? tb.Text.Trim() : "";

                if (string.IsNullOrEmpty(newDestination))
                {
                    UpdateStatus("Please enter a destination", Color.Red);
                    return;
                }

                // Find and update tank
                OxygenTank tank = tanks.FirstOrDefault(t => t.SerialNumber == serialNumber);
                if (tank != null)
                {
                    tank.Destination = newDestination;

                    // Refresh grid
                    RefreshTanksGrid();

                    // Save changes
                    SaveTanksToFile();

                    UpdateStatus($"Destination updated for tank {serialNumber}", Color.Green);
                }
            }
            else
            {
                UpdateStatus("Please select a tank to update", Color.Red);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadTanksFromFile();
            RefreshTanksGrid();
            UpdateStatus("Data refreshed", Color.Green);
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "CSV files (*.csv)|*.csv";
                    saveDialog.Title = "Export Tanks Data";
                    saveDialog.FileName = "oxygen_tanks_export.csv";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                        {
                            // Write header
                            writer.WriteLine("SerialNumber,Status,LastDeliveryDate,LastReturnDate,Destination");

                            // Write tank data
                            foreach (OxygenTank tank in tanks)
                            {
                                writer.WriteLine($"{tank.SerialNumber},{tank.Status},{tank.LastDeliveryDate},{tank.LastReturnDate},{tank.Destination}");
                            }
                        }

                        UpdateStatus($"Data exported to {saveDialog.FileName}", Color.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Export error: {ex.Message}", Color.Red);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = GetControlByName("searchTextBox") is TextBox tb ? tb.Text.Trim() : "";

            if (string.IsNullOrEmpty(searchText))
            {
                RefreshTanksGrid(); // Show all tanks
                return;
            }

            // Filter tanks by serial number or destination
            List<OxygenTank> filteredTanks = tanks.Where(t =>
                t.SerialNumber.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                t.Destination.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            DisplayTanks(filteredTanks);
        }

        private Control GetControlByName(string name)
        {
            return Controls.Find(name, true).FirstOrDefault();
        }

        private void RecordTankDelivery(string serialNumber, string destination)
        {
            // Check if tank exists
            OxygenTank existingTank = tanks.FirstOrDefault(t => t.SerialNumber == serialNumber);

            if (existingTank != null)
            {
                // Tank exists, check status
                if (existingTank.Status == TankStatus.Delivered)
                {
                    UpdateStatus($"Warning: Tank {serialNumber} is already out for delivery! Last delivery: {existingTank.LastDeliveryDate}", Color.Orange);
                }
                else
                {
                    // Update tank for delivery
                    existingTank.Status = TankStatus.Delivered;
                    existingTank.LastDeliveryDate = DateTime.Now;
                    existingTank.Destination = destination;
                    UpdateStatus($"Tank {serialNumber} marked as delivered", Color.Green);
                }
            }
            else
            {
                // New tank, add to collection
                OxygenTank newTank = new OxygenTank
                {
                    SerialNumber = serialNumber,
                    Status = TankStatus.Delivered,
                    LastDeliveryDate = DateTime.Now,
                    Destination = destination
                };

                tanks.Add(newTank);
                UpdateStatus($"New tank {serialNumber} added and marked as delivered", Color.Green);
            }

            // Refresh grid and save data
            RefreshTanksGrid();
            SaveTanksToFile();
        }

        private void RecordTankReturn(string serialNumber)
        {
            // Find tank
            OxygenTank tank = tanks.FirstOrDefault(t => t.SerialNumber == serialNumber);

            if (tank != null)
            {
                // Update tank status
                tank.Status = TankStatus.Available;
                tank.LastReturnDate = DateTime.Now;
                UpdateStatus($"Tank {serialNumber} marked as returned", Color.Green);

                // Refresh grid and save data
                RefreshTanksGrid();
                SaveTanksToFile();
            }
            else
            {
                UpdateStatus($"Error: Tank {serialNumber} not found in database", Color.Red);
            }
        }

        private void RefreshTanksGrid()
        {
            DisplayTanks(tanks);
        }

        private void DisplayTanks(List<OxygenTank> tanksToDisplay)
        {
            if (GetControlByName("tanksGrid") is DataGridView grid)
            {
                // Create data table
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("SerialNumber", typeof(string));
                dataTable.Columns.Add("Status", typeof(string));
                dataTable.Columns.Add("LastDeliveryDate", typeof(string));
                dataTable.Columns.Add("LastReturnDate", typeof(string));
                dataTable.Columns.Add("Destination", typeof(string));

                // Add rows
                foreach (OxygenTank tank in tanksToDisplay.OrderBy(t => t.SerialNumber))
                {
                    dataTable.Rows.Add(
                        tank.SerialNumber,
                        tank.Status,
                        tank.LastDeliveryDate == DateTime.MinValue ? "-" : tank.LastDeliveryDate.ToString(),
                        tank.LastReturnDate == DateTime.MinValue ? "-" : tank.LastReturnDate.ToString(),
                        tank.Destination
                    );
                }

                // Set data source
                grid.DataSource = dataTable;

                // Color coding
                grid.CellFormatting += (s, e) => {
                    if (e.RowIndex >= 0 && e.ColumnIndex == 1) // Status column
                    {
                        if (e.Value.ToString() == "Delivered")
                        {
                            e.CellStyle.BackColor = Color.LightCoral;
                        }
                        else if (e.Value.ToString() == "Available")
                        {
                            e.CellStyle.BackColor = Color.LightGreen;
                        }
                    }
                };
            }
        }

        private void LoadTanksFromFile()
        {
            tanks.Clear();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    // Skip header
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(',');
                        if (parts.Length >= 5)
                        {
                            OxygenTank tank = new OxygenTank
                            {
                                SerialNumber = parts[0],
                                Status = parts[1] == "Delivered" ? TankStatus.Delivered : TankStatus.Available,
                                LastDeliveryDate = DateTime.TryParse(parts[2], out DateTime deliveryDate) ? deliveryDate : DateTime.MinValue,
                                LastReturnDate = DateTime.TryParse(parts[3], out DateTime returnDate) ? returnDate : DateTime.MinValue,
                                Destination = parts[4]
                            };

                            tanks.Add(tank);
                        }
                    }

                    UpdateStatus($"Loaded {tanks.Count} tanks from database", Color.Green);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading data: {ex.Message}", Color.Red);
            }
        }

        private void SaveTanksToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("SerialNumber,Status,LastDeliveryDate,LastReturnDate,Destination");

                    // Write tank data
                    foreach (OxygenTank tank in tanks)
                    {
                        writer.WriteLine($"{tank.SerialNumber},{tank.Status},{tank.LastDeliveryDate},{tank.LastReturnDate},{tank.Destination}");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error saving data: {ex.Message}", Color.Red);
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (GetControlByName("statusLabel") is Label label)
            {
                label.Text = message;
                label.ForeColor = color;
            }
        }
    }

    public enum TankStatus
    {
        Available,
        Delivered
    }

    public class OxygenTank
    {
        public string SerialNumber { get; set; }
        public TankStatus Status { get; set; }
        public DateTime LastDeliveryDate { get; set; }
        public DateTime LastReturnDate { get; set; }
        public string Destination { get; set; }

        public OxygenTank()
        {
            SerialNumber = "";
            Status = TankStatus.Available;
            LastDeliveryDate = DateTime.MinValue;
            LastReturnDate = DateTime.MinValue;
            Destination = "";
        }
    }
}