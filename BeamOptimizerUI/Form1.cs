using BeamOptimizer;
using BeamOptimizer.FileReaders;
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

namespace BeamOptimizerUI
{
    public partial class BeamOptimizerUI : Form
    {
        public BeamOptimizerUI()
        {
            InitializeComponent();
        }

        private void BeamOptimizerUI_Load(object sender, EventArgs e)
        {
            this.TbxProjectsRoot.Text = Properties.Settings.Default.ProjectsRoot;
            this.TbxGrondstoffenBestand.Text = Properties.Settings.Default.GrondstoffenBestand;

        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            var ProjectsRoot = TbxProjectsRoot.Text;
            var GrondstoffenBestand = TbxGrondstoffenBestand.Text;

            if (!FolderExists(ProjectsRoot))
            {
                return;
            }

            if (!FilenameExists(GrondstoffenBestand, "Grondstoffen"))
            {
                return;
            }

            Properties.Settings.Default.ProjectsRoot = ProjectsRoot;
            Properties.Settings.Default.GrondstoffenBestand = GrondstoffenBestand;

            Properties.Settings.Default.Save();

            MessageBox.Show("Gegevens zijn opgeslagen");            
        }

        private bool FolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                MessageBox.Show($"Map '{path}' bestaat niet!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool FilenameExists(string path, string fileName)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"Bestand '{path}' bestaat niet!");
                return false;
            }

           var Grondstoffenextension = Path.GetExtension(path);

            if (Grondstoffenextension.ToUpper() != ".XLS")
            {
                MessageBox.Show($"Bestand '{path}' heeft niet de vereiste extentie(.XLS).");
                return false;
            }

            var filename = Path.GetFileNameWithoutExtension(path);

            if (filename != fileName)
            {
                MessageBox.Show($"Bestand '{path}' heeft niet de vereiste naam ({fileName}).");
                return false;
            }

            return true;
        }

        private void BtnRender_Click(object sender, EventArgs e)
        {
            var projectName = TbxProjectNaam.Text;
            if (!ProjectNameValid(projectName))
            {
                return;
            }


            var projectFolder = Path.Combine(TbxProjectsRoot.Text,projectName);
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
                MessageBox.Show($"Map {projectFolder} is aangemaakt, plaats stukken.xls in map");
                return;
            }

            var stukkenBestand = Path.Combine(projectFolder, "Stukken.xls");
            if (!FilenameExists(stukkenBestand, "Stukken"))
            {
                return;
            }

            // Project en stukkenbestand bestaat - Start Render
            var optimizerProcess = new BeamOptimizerProcess(projectName, TbxGrondstoffenBestand.Text, stukkenBestand,projectFolder, new GrondstofCsvReader(), new StukCsvReader());
            optimizerProcess.Run();

            MessageBox.Show("Met een beetje geluk staat er nu een zaaglijst en orderlijst klaar in de project map. En nu, hop hop aan het werk!");
        }

       private bool ProjectNameValid(string fileName)
        {
            var isValid = !string.IsNullOrEmpty(fileName) &&
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            //&& !File.Exists(Path.Combine(sourceFolder, fileName));

            if (!isValid)
            {
                MessageBox.Show($"Projectnaam {fileName} is niet geldig");
                return false;
            }
            return true;
        }
    }
}
