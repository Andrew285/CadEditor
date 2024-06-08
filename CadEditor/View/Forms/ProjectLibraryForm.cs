using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor.View.Forms
{
    public partial class ProjectLibraryForm : Form
    {
        private List<SaveData> _saves;
        private SaveData _selectedSave;
        private int _projectCounter = 0;
        private FlowLayoutPanel _selectedProjectPanel;
        private ToolTip _toolTip;

        public ProjectLibraryForm(List<SaveData> saves)
        {
            this._saves = saves;
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set up the form
            this.Text = "Project Library";
            this.Size = new Size(800, 600);
            this.TopMost = true;

            // Create a TabControl
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // Create two TabPages
            TabPage tabPage1 = new TabPage("Tab 1");
            TabPage tabPage2 = new TabPage("Tab 2");

            // Add TabPages to TabControl
            tabControl.TabPages.Add(tabPage1);
            tabControl.TabPages.Add(tabPage2);

            // Add TabControl to the form
            this.Controls.Add(tabControl);

            // Initialize tabs with project data
            InitializeTabPage(tabPage1);
            InitializeTabPage(tabPage2);

            // Create buttons
            Button addCurrentButton = new Button();
            addCurrentButton.Text = "Add";
            addCurrentButton.Click += OpenButton_Click;

            Button openNewButton = new Button();
            openNewButton.Text = "Open as New";
            openNewButton.AutoSize = true;
            openNewButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            openNewButton.AutoEllipsis = false;
            openNewButton.Click += OpenButton_Click;

            Button deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Click += DeleteButton_Click;

            // Create a panel for buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.Controls.Add(addCurrentButton);
            buttonPanel.Controls.Add(openNewButton);
            buttonPanel.Controls.Add(deleteButton);

            // Add button panel to the form
            this.Controls.Add(buttonPanel);

            // Initialize ToolTip control
            _toolTip = new ToolTip();
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 1000;
            _toolTip.ReshowDelay = 500;
            _toolTip.ShowAlways = true;

            // Set tooltips for buttons
            _toolTip.SetToolTip(addCurrentButton, "Add selected project to current opened scene");
            _toolTip.SetToolTip(openNewButton, "Open selected project in new scene");
            _toolTip.SetToolTip(deleteButton, "Delete selected project from library");
        }

        private void InitializeTabPage(TabPage tabPage)
        {
            // Create a TableLayoutPanel
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.ColumnCount = 6;
            tableLayoutPanel.RowCount = 0;
            tableLayoutPanel.AutoSize = true;

            // Add projects to the TableLayoutPanel
            foreach (var save in _saves)
            {
                AddProjectRow(tableLayoutPanel, save.GetPicture(), save.GetTitle(), save.GetDate());
            }

            // Add TableLayoutPanel to the TabPage
            tabPage.Controls.Add(tableLayoutPanel);
        }

        private void AddProjectRow(TableLayoutPanel tableLayoutPanel, Bitmap image, string name, DateTime date)
        {
            // Create controls for the project
            PictureBox pictureBox = new PictureBox();
            //pictureBox.Image = Image.FromFile(imagePath);
            pictureBox.BackgroundImage = image;
            //pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox.Height = 100;
            pictureBox.Width = 100;

            Label nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.Font = new Font(Label.DefaultFont, FontStyle.Bold);
            nameLabel.AutoSize = true;

            Label dateLabel = new Label();
            dateLabel.Text = date.ToString("g");
            dateLabel.AutoSize = true;

            // Create a FlowLayoutPanel for each project row
            FlowLayoutPanel projectPanel = new FlowLayoutPanel();
            projectPanel.FlowDirection = FlowDirection.TopDown;
            projectPanel.AutoSize = true;
            projectPanel.Margin = new Padding(5);
            projectPanel.Padding = new Padding(5);
            projectPanel.BorderStyle = BorderStyle.FixedSingle;
            projectPanel.Tag = _projectCounter++;
            projectPanel.Click += (s, e) => ProjectPanel_Click(projectPanel);
            pictureBox.Click += (s, e) => ProjectPanel_Click(projectPanel);

            // Add controls to the FlowLayoutPanel
            projectPanel.Controls.Add(pictureBox);
            projectPanel.Controls.Add(nameLabel);
            projectPanel.Controls.Add(dateLabel);

            // Add the FlowLayoutPanel to the TableLayoutPanel
            tableLayoutPanel.Controls.Add(projectPanel);
            tableLayoutPanel.RowCount++;
        }

        public SaveData GetSelectedProject()
        {
            return _selectedSave;
        }

        private void ProjectPanel_Click(FlowLayoutPanel projectPanel)
        {
            if (_selectedProjectPanel != null)
            {
                _selectedProjectPanel.BackColor = Color.Transparent;
            }

            _selectedProjectPanel = projectPanel;
            _selectedSave = _saves[(int)_selectedProjectPanel.Tag];
            _selectedProjectPanel.BackColor = Color.Red;
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (_selectedProjectPanel == null)
            {
                Output.ShowMessageBox("Warning", "Please select a project to delete.");
            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_selectedProjectPanel != null)
            {
                DialogResult result = Output.ShowError("Confirm Deletion",
                                                       "Are you sure you want to delete this project?");
                if (result == DialogResult.Yes)
                {
                    _saves.Remove(_selectedSave);
                }
            }
            else
            {
                Output.ShowMessageBox("Warning", "Please select a project to delete.");
            }
        }
    }
}
