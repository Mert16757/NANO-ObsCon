using ASCOM.Utilities;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ASCOM.NANO.ObservingConditions
{
    [ComVisible(false)] // Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        const string NO_PORTS_MESSAGE = "No COM ports found";
        TraceLogger tl; // Holder for a reference to the driver's trace logger

        public SetupDialogForm(TraceLogger tlDriver)
        {
            InitializeComponent();

            // Save the provided trace logger for use within the setup dialogue
            tl = tlDriver;

            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void CmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here and update the state variables with results from the dialogue

            tl.Enabled = chkTrace.Checked;

            // Update the COM port variable if one has been selected
            if (comboBoxComPort.SelectedItem is null) // No COM port selected
            {
                tl.LogMessage("Setup OK", $"New configuration values - COM Port: Not selected");
            }
            else if (comboBoxComPort.SelectedItem.ToString() == NO_PORTS_MESSAGE)
            {
                tl.LogMessage("Setup OK", $"New configuration values - NO COM ports detected on this PC.");
            }
            else if ( FWHMtextBox1.Text == "" )
            {
                FWHMtextBox1.Text = ObservingConditionsHardware.FWHMpathDefault;
            } else if ( BatchtextBox1.Text == "")
            {
                BatchtextBox1.Text = ObservingConditionsHardware.BatchfilepathDefault;
            } else if ( Batchfile.Text == "")
            {
                Batchfile.Text = ObservingConditionsHardware.BatchfileDefault;
            } 
            // A valid COM port has been selected and a path to save the FWHM images and logfile have been set.
            {
                ObservingConditionsHardware.comPort = (string)comboBoxComPort.SelectedItem;
                ObservingConditionsHardware.FWHMpathDefault = (string)FWHMtextBox1.Text;
                ObservingConditionsHardware.BatchfilepathDefault = (string)BatchtextBox1.Text;
                ObservingConditionsHardware.SensorLogfileDefault = (string)LogfiletextBox1.Text;
                ObservingConditionsHardware.CorrFactorScopeDefault = (string)CorrtextBox1.Text;
                tl.LogMessage("Setup OK", $"New configuration values - COM Port: {comboBoxComPort.SelectedItem}");
            }
        }

        private void CmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("https://ascom-standards.org/");
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {

            // Set the trace checkbox
            chkTrace.Checked = tl.Enabled;
            //    LogfilecheckBox1.Checked = Convert.ToBoolean(ObservingConditionsHardware.LogFileStateDefault);
            //    LogfilecheckBox1.Checked = ObservingConditionsHardware.SensorLogEnable;
            //    MessageBox.Show(Convert.ToString(ObservingConditionsHardware.SensorLogEnable));  Lo hace bien, lee cambia etc... pero no pone el check
            // set the list of COM ports to those that are currently available
            comboBoxComPort.Items.Clear(); // Clear any existing entries
            using (Serial serial = new Serial()) // User the Se5rial component to get an extended list of COM ports
            {
                comboBoxComPort.Items.AddRange(serial.AvailableCOMPorts);
            }

            // If no ports are found include a message to this effect
            if (comboBoxComPort.Items.Count == 0)
            {
                comboBoxComPort.Items.Add(NO_PORTS_MESSAGE);
                comboBoxComPort.SelectedItem = NO_PORTS_MESSAGE;
            }

            // select the current port if possible
            if (comboBoxComPort.Items.Contains(ObservingConditionsHardware.comPort))
            {
                comboBoxComPort.SelectedItem = ObservingConditionsHardware.comPort;
            } 
            if ( FWHMtextBox1.Text == "" )
            {
                FWHMtextBox1.Text = ObservingConditionsHardware.FWHMpathDefault;
            } 
            if ( BatchtextBox1.Text == "" )
            {
                BatchtextBox1.Text = ObservingConditionsHardware.BatchfileDefault;
            }
           

            FWHMtextBox1.Text = ObservingConditionsHardware.FWHMpathDefault;
            BatchtextBox1.Text = ObservingConditionsHardware.BatchfilepathDefault;
            Batchfile.Text = ObservingConditionsHardware.BatchfileDefault;
            LogfiletextBox1.Text = ObservingConditionsHardware.SensorLogfileDefault;
            LogfilecheckBox1.Checked = Convert.ToBoolean(ObservingConditionsHardware.SensorLogEnable);
            UpdatetextBox1.Text = ObservingConditionsHardware.UpdateSampleTimeDefault;
            CorrtextBox1.Text = ObservingConditionsHardware.CorrFactorScopeDefault;
            tl.LogMessage("InitUI", $"Set UI controls to Trace: {chkTrace.Checked}, COM Port: {comboBoxComPort.SelectedItem}");
        }

        private void SetupDialogForm_Load(object sender, EventArgs e)
        {
            // Bring the setup dialogue to the front of the screen
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            {
                TopMost = true;
                Focus();
                BringToFront();
                TopMost = false;
            }
        }

        private void comboBoxComPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
        private void folderBrowserDialog1_HelpRequest_1(object sender, EventArgs e)
        {

        }
        private void FWHMtextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void chkTrace_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LogfilecheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            //  MessageBox.Show("Logfile enable selected!");    OK  funciona!
            if (LogfilecheckBox1.Checked == true)
            {
                ObservingConditionsHardware.SensorLogEnable = "True";
            } else
            {
                ObservingConditionsHardware.SensorLogEnable = "False";
            }
        }

        private void UpdatetextBox1_TextChanged(object sender, EventArgs e)
        {
            ObservingConditionsHardware.UpdateSampleTimeDefault = UpdatetextBox1.Text;
        }

        private void CorrtextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}